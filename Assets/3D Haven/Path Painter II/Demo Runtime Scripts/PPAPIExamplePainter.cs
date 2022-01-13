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
    public class PPAPIExamplePainter : MonoBehaviour
    {
        #region Public, Inspector Memebers

        [Header("Viewport")]
        [Tooltip("Drag and drop the camera users will use as their viewport.")]
        public Camera Camera;

        [Header("Sizes")]
        [Tooltip("Drag and drop the dropdown from the GUI.")]
        public Dropdown PathSizeDropdown;
        [Tooltip("Drag and drop the dropdown from the GUI.")]
        public Dropdown EmbankmentSizeDropdown;
        [Tooltip("Avaliable path sizes.")]
        public LabeledFloat[] PathSizes =
        {
            new LabeledFloat("XS", 2f),
            new LabeledFloat("S", 4f),
            new LabeledFloat("M", 6f),
            new LabeledFloat("L", 12f),
            new LabeledFloat("XL", 24f)
        };
        [Tooltip("Avaliable embankment size ratios (i.e. size is multiplied by this number).")]
        public LabeledFloat[] EmbankmentSizes =
        {
            new LabeledFloat("XS", 1.5f),
            new LabeledFloat("S", 2f),
            new LabeledFloat("M", 4f),
            new LabeledFloat("L", 8f),
            new LabeledFloat("XL", 12f)
        };

        [Header("Layers")]
        [Tooltip("Drag and drop the dropdown from the GUI.")]
        public Dropdown PathLayerDropdown;
        [Tooltip("Drag and drop the dropdown from the GUI.")]
        public Dropdown EmbankmentLayerDropdown;
        [Tooltip("Drag and drop layers here.")]
        public TerrainLayer[] Layers;

        [Header("Other settings")]
        [Tooltip("Drag and drop the ramp slider from the GUI.")]
        public Slider RampSlider;

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

        private bool m_painting = false;
        private int m_pathSizeIx = 3;
        private int m_embankmentSizeIx = 3;
        private float m_pathSize = 6f;
        private float m_embankmentRatio = 24f;
        private TerrainLayer m_pathLayer = null;
        private TerrainLayer m_embankmentLayer = null;

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
        /// Start is called before the first frame update
        /// </summary>
        private void Start()
        {
            if (!ValidateSetup())
            {
                return;
            }
            InitUndo();
            InitDropdowns();
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
            if (PathSizeDropdown == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Path size dropdown missing. Drag and drop it from the GUI.", gameObject.name);
            }
            if (EmbankmentSizeDropdown == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Embankment size dropdown missing. Drag and drop it from the GUI.", gameObject.name);
            }
            if (PathLayerDropdown == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Path layer dropdown missing. Drag and drop it from the GUI.", gameObject.name);
            }
            if (EmbankmentLayerDropdown == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Embankment layer dropdown missing. Drag and drop it from the GUI.", gameObject.name);
            }
            if (RampSlider == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Ramp slider missing. Drag and drop it from the GUI.", gameObject.name);
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
        /// Initialises the dropdowns
        /// </summary>
        private void InitDropdowns()
        {
            Dropdown.OptionDataList ddList = new Dropdown.OptionDataList();
            ddList.options.Add(new Dropdown.OptionData("Size"));
            for (int i = 0; i < PathSizes.Length; i++)
            {
                ddList.options.Add(new Dropdown.OptionData(PathSizes[i].Label));
            }
            PathSizeDropdown.options = ddList.options;

            ddList = new Dropdown.OptionDataList();
            ddList.options.Add(new Dropdown.OptionData("Size"));
            for (int i = 0; i < EmbankmentSizes.Length; i++)
            {
                ddList.options.Add(new Dropdown.OptionData(EmbankmentSizes[i].Label));
            }
            EmbankmentSizeDropdown.options = ddList.options;

            ddList = new Dropdown.OptionDataList();
            ddList.options.Add(new Dropdown.OptionData("Material"));
            if (Layers != null && Layers.Length > 0)
            {
                Vector2 mid = new Vector2(0.5f, 0.5f);
                for (int i = 0; i < Layers.Length; i++)
                {
                    Texture2D tx = Layers[i].diffuseTexture;
                    ddList.options.Add(new Dropdown.OptionData(Sprite.Create(tx, new Rect(0f, 0f, tx.width, tx.height), mid)));
                }
            }
            PathLayerDropdown.options = ddList.options;
            EmbankmentLayerDropdown.options = ddList.options;

            // Set the default values
            PathSizeDropdown.value = m_pathSizeIx;
            EmbankmentSizeDropdown.value = m_embankmentSizeIx;
            PathLayerDropdown.value = 1;
            EmbankmentLayerDropdown.value = 2;
            SetPathSize(m_pathSizeIx);
            SetEmbankmentSize(m_embankmentSizeIx);
            SetPathLayer(1);
            SetEmbankmentLayer(2);
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
        /// Used to swtich painting on/off
        /// </summary>
        public void PaintingSwitch(bool val)
        {
            m_painting = val;
        }

        /// <summary>
        /// Called by the dropdown to set the value
        /// </summary>
        public void SetPathSize(int i)
        {
            // Label size doesn't make sence, so revert
            if (i < 1)
            {
                PathSizeDropdown.value = m_pathSizeIx;
                return;
            }
            m_pathSizeIx = PathSizeDropdown.value;
            m_pathSize = PathSizes[Mathf.Clamp(m_pathSizeIx - 1, 0, PathSizes.Length)].Value;
            m_flowRate = m_pathSize * FLOW_RATE_MULTIPLIER;
        }

        /// <summary>
        /// Called by the dropdown to set the value
        /// </summary>
        public void SetEmbankmentSize(int i)
        {
            // Label size doesn't make sence, so revert
            if (i < 1)
            {
                EmbankmentSizeDropdown.value = m_pathSizeIx;
                return;
            }
            m_embankmentSizeIx = EmbankmentSizeDropdown.value;
            m_embankmentRatio = EmbankmentSizes[Mathf.Clamp(m_embankmentSizeIx - 1, 0, EmbankmentSizes.Length)].Value;
        }

        /// <summary>
        /// Called by the dropdown to set the value
        /// </summary>
        public void SetPathLayer(int i)
        {
            if (i < 1)
            {
                m_pathLayer = null;
                return;
            }
            m_pathLayer = Layers[Mathf.Clamp(PathLayerDropdown.value - 1, 0, Layers.Length)];
        }

        /// <summary>
        /// Called by the dropdown to set the value
        /// </summary>
        public void SetEmbankmentLayer(int i)
        {
            if (i < 1)
            {
                m_embankmentLayer = null;
                return;
            }
            m_embankmentLayer = Layers[Mathf.Clamp(EmbankmentLayerDropdown.value - 1, 0, Layers.Length)];
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
            if (!m_painting)
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
                m_painter.NewLine(hit.point, Painter.Size(m_pathSize), Painter.EmbankmentSize(m_pathSize * m_embankmentRatio),
                    Painter.Texture(m_pathLayer), Painter.EmbankmentTexture(m_embankmentLayer), Painter.EvenRamp(RampSlider.value));
            }
            else
            {
                m_painter.AddToLine(hit.point);
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
            m_painter.AddToLine(hit.point);
            m_activeLine.Add(hit.point);
        }

        /// <summary>
        /// Called during the frame the user released the mouse button.
        /// </summary>
        private void MouseUp()
        {
            if (m_mouseDown)
            {
                m_painter.CompleteLine();
            }
            m_mouseDown = false;
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
            if (!ValidateUndoStack())
            {
                return;
            }
            ApplyUndo(m_undoStack.Peek());
            m_painter.Paint(m_activeLine, Painter.Size(m_pathSize), Painter.EmbankmentSize(m_pathSize * m_embankmentRatio),
                    Painter.Texture(m_pathLayer), Painter.EmbankmentTexture(m_embankmentLayer), Painter.EvenRamp(RampSlider.value));
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
