using System.Collections.Generic;
using UnityEngine;
using Haven.API.PathPainter2;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Haven.Demo
{
    /// <summary>
    /// A crude Path Painter API example that features a rudimentary undo system.
    /// </summary>
    public class PPAPIExamplePainter2 : MonoBehaviour
    {
        #region Public, Inspector Memebers

        [Header("Viewport")]
        [Tooltip("Drag and drop the camera users will use as their viewport.")]
        public Camera Camera;

        [Header("Main Contols")]
        [Tooltip("Drag and drop the SelectionGrid the user will use to select what to paint.")]
        public SelectionGrid PresetSelectionGrid;

        [Tooltip("Match these to the items in the selection grid.")]
        public PaintPreset[] Presets;

        [Header("Undo/Refresh")]
        [Tooltip("Undo and refresh is fairly calculation intensive.\n" +
            "This will improve with a built-in Undo system.\n" +
            "Disable for best performance")]
        public bool UndoEnabled = true;
        public Button UndoButton;
        public Button RefreshButton;

        #endregion

        #region Private Members

        private Painter m_painter;

        private bool m_validPresetSelected = false;
        private float m_pathSize = 6f;
        private float m_embankmentSize = 24f;
        private TerrainLayer m_pathLayer = null;
        private TerrainLayer m_embankmentLayer = null;
        private float m_slopeLimit = 20f;
        private float m_ramp = 0f;

        private bool m_mouseDown = false;
        private const float FLOW_RATE_MULTIPLIER = 0.3f;
        private float m_flowRate = 0.5f;
        private List<Vector3> m_activeLine;
        private Vector2 m_lastHitColumn;

        private bool m_undoEnabled;
        private bool m_optionalButtonsProvided;
        private Dictionary<TerrainData, TerrainSnapshot> m_undoSnapshot;
        private Stack<IDictionary<TerrainData, TerrainSnapshot>> m_undoStack;

        private bool m_validSetup = false;

        #endregion

        #region Initialisation

        /// <summary>
        /// Awake is called before Start, which is called before the first frame update
        /// </summary>
        private void Awake()
        {
            if (!ValidateSetup())
            {
                return;
            }
            InitGUI();
            InitUndo();
            m_painter = new Painter(RecordSnapshot);
        }

        /// <summary>
        /// Validates that everything is in order.
        /// </summary>
        private bool ValidateSetup()
        {
            m_validSetup = true;
            m_optionalButtonsProvided = true;
            if (Camera == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Camera missing. Drag and drop the camera users will use as their viewport.", gameObject.name);
            }
            if (PresetSelectionGrid == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Selection grid missing. Drag and drop it from the GUI.", gameObject.name);
            }
            if (UndoButton == null)
            {
                m_validSetup = m_optionalButtonsProvided = UndoEnabled = false;
                Debug.LogErrorFormat("[{0}]: Undo button missing. Drag and drop it from the GUI.", gameObject.name);
            }
            if (RefreshButton == null)
            {
                m_validSetup = m_optionalButtonsProvided = UndoEnabled = false;
                Debug.LogErrorFormat("[{0}]: Refresh button missing. Drag and drop it from the GUI.", gameObject.name);
            }
            return m_validSetup;
        }

        /// <summary>
        /// Initialises GUI
        /// </summary>
        private void InitGUI()
        {
            PresetSelectionGrid.OnValueChange = SelectionChange;
            PresetSelectionGrid.MaxValue = Presets.Length - 1;
            if (UndoEnabled)
            {
                UndoButton.onClick.AddListener(Undo);
                RefreshButton.onClick.AddListener(Refresh);
            }
        }

        /// <summary>
        /// Initialises undo if enabled
        /// </summary>
        private void InitUndo()
        {
            m_undoEnabled = UndoEnabled;
            if (m_undoEnabled)
            {
                m_undoStack = new Stack<IDictionary<TerrainData, TerrainSnapshot>>();
                UndoButton.interactable = false;
                RefreshButton.interactable = false;
            }
            else
            {
                UndoButton.gameObject.SetActive(false);
                RefreshButton.gameObject.SetActive(false);
            }
        }

        #endregion

        #region GUI Interface

        /// <summary>
        /// Handles selection change
        /// </summary>
        public void SelectionChange(int val)
        {
            if (val < 0)
            {
                m_validPresetSelected = false;
                return;
            }
            m_validPresetSelected = true;
            m_pathSize = Presets[val].PathSize;
            m_embankmentSize = Presets[val].EmbankmentSize;
            m_ramp = Presets[val].Ramp;
            m_slopeLimit = Presets[val].SlopeLimit;
            m_pathLayer = Presets[val].PathLayer;
            m_embankmentLayer = Presets[val].EmbankmentLayer;
        }

        #endregion

        #region Paint Mechanism

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            if (!m_validSetup)
            {
                return;
            }
            // Disable/enable undo and refresgh
            if (m_undoEnabled != UndoEnabled)
            {
                if (m_optionalButtonsProvided)
                {
                    m_undoEnabled = UndoEnabled;
                }
                else
                {
                    UndoEnabled = false;
                }
            }
            if (!m_validPresetSelected)
            {
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                MouseUp();
                return;
            }
            RaycastHit hit;
            if (!Raycast(out hit))
            {
                return;
            }
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                MouseDown(hit);
                return;
            }
            if (m_mouseDown)
            {
                MouseDrag(hit);
            }
        }

        /// <summary>
        /// Figures out where the user is pointing.
        /// </summary>
        private bool Raycast(out RaycastHit hit)
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called during the frame the user pressed the mouse button.
        /// </summary>
        private void MouseDown(RaycastHit hit)
        {
            m_mouseDown = true;
            m_lastHitColumn = new Vector2(hit.point.x, hit.point.z);

            // If shift is down, the user is continuing an existing line
            if (m_activeLine == null || !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                m_activeLine = new List<Vector3>();
                if (m_undoEnabled)
                {
                    m_undoSnapshot = new Dictionary<TerrainData, TerrainSnapshot>();
                    m_undoStack.Push(m_undoSnapshot);
                }
                PaintStart(hit.point);
            }
            else
            {
                PaintAdd(hit.point);
            }
            m_activeLine.Add(hit.point);
        }

        /// <summary>
        /// Called when mouse is dragging.
        /// </summary>
        private void MouseDrag(RaycastHit hit)
        {
            Vector2 hitColumn = new Vector2(hit.point.x, hit.point.z);
            // Do nothing for tiny mouse movements.
            if (Vector2.Distance(m_lastHitColumn, hitColumn) < m_flowRate)
            {
                return;
            }
            m_lastHitColumn = hitColumn;
            PaintAdd(hit.point);
            m_activeLine.Add(hit.point);
        }

        /// <summary>
        /// Called during the frame the user released the mouse button.
        /// </summary>
        private void MouseUp()
        {
            if (m_mouseDown)
            {
                PaintEnd();
            }
            m_mouseDown = false;
        }

        #endregion

        #region Paint Actions

        /// <summary>
        /// Used when painting paths/ramps
        /// </summary>
        private void PaintStart(Vector3 point)
        {
            m_painter.NewLine(point, Painter.Size(m_pathSize), Painter.EmbankmentSize(m_embankmentSize),
                Painter.SlopeLimit(m_slopeLimit), Painter.EvenRamp(m_ramp), 
                Painter.Texture(m_pathLayer), Painter.EmbankmentTexture(m_embankmentLayer));
        }

        /// <summary>
        /// Used when painting paths/ramps
        /// </summary>
        private void PaintAdd(Vector3 point)
        {
            m_painter.AddToLine(point);
        }

        /// <summary>
        /// Used when painting paths/ramps
        /// </summary>
        private void PaintEnd()
        {
            m_painter.CompleteLine();
        }

        #endregion

        #region Undo System

        /// <summary>
        /// This will record snapshots of terrains to undo painting
        /// </summary>
        public void RecordSnapshot(Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;
            // Make sure we don't add the same terrain twice to the same snapshot
            if (m_undoSnapshot.ContainsKey(terrainData))
            {
                return;
            }
            UndoButton.interactable = true;
            RefreshButton.interactable = true;
            TerrainSnapshot terrainSnapshot = new TerrainSnapshot();
            terrainSnapshot.Heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
            terrainSnapshot.Splats = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution);
            int[][,] details = new int[terrainData.detailPrototypes.Length][,];
            for (int i = 0; i < details.Length; i++)
            {
                details[i] = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i);
            }
            terrainSnapshot.Details = details;
            terrainSnapshot.Trees = terrainData.treeInstances;
            m_undoSnapshot[terrainData] = terrainSnapshot;
        }

        /// <summary>
        /// Resets the terrain
        /// </summary>
        public void Undo()
        {
            if (!ValidateUndoStack())
            {
                return;
            }
            ApplyUndo(m_undoStack.Pop());
            if (m_undoStack.Count < 1)
            {
                UndoButton.interactable = false;
                RefreshButton.interactable = false;
            }
        }

        /// <summary>
        /// Refreshes the last painted line. Good to use after settings changes.
        /// </summary>
        public void Refresh()
        {
            if (!m_validPresetSelected || !ValidateUndoStack())
            {
                return;
            }
            ApplyUndo(m_undoStack.Peek());
            m_painter.Paint(m_activeLine, Painter.Size(m_pathSize), Painter.EmbankmentSize(m_embankmentSize),
                Painter.SlopeLimit(m_slopeLimit), Painter.EvenRamp(m_ramp),
                Painter.Texture(m_pathLayer), Painter.EmbankmentTexture(m_embankmentLayer));
        }

        /// <summary>
        /// Handles if snapshots miracleously disappear. Returns false if the stack is empty.
        /// </summary>
        private bool ValidateUndoStack()
        {
            if (m_undoStack.Count < 1)
            {
                Debug.LogErrorFormat("Nothing to undo here.");
                UndoButton.interactable = false;
                RefreshButton.interactable = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Applies a snapshot to the terrain.
        /// </summary>
        private void ApplyUndo(IDictionary<TerrainData, TerrainSnapshot> snapshot)
        {
            foreach (KeyValuePair<TerrainData, TerrainSnapshot> item in snapshot)
            {
                item.Key.SetHeights(0, 0, item.Value.Heights);
                item.Key.SetAlphamaps(0, 0, item.Value.Splats);
                if (item.Key.detailPrototypes.Length == item.Value.Details.Length)
                {
                    for (int i = 0; i < item.Value.Details.Length; i++)
                    {
                        item.Key.SetDetailLayer(0, 0, i, item.Value.Details[i]);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("[{0}]: Warning! The number of detail layers of terrain {1} changed ({2} vs {3}). Aborting detail undo...",
                        gameObject.name, item.Key.name, item.Key.detailPrototypes.Length, item.Value.Details.Length);
                }
                item.Key.treeInstances = item.Value.Trees;
            }
        }

        /// <summary>
        /// Terrain snapshot. A crude way to record undo.
        /// </summary>
        private struct TerrainSnapshot
        {
            public float[,] Heights;
            public float[,,] Splats;
            public int[][,] Details;
            public TreeInstance[] Trees;
        }

        #endregion
    } 
}
