//#define DLD_BEZ_SOL
using System.Collections.Generic;
using UnityEngine;
using Haven.API.PathPainter2;
using UnityEngine.UI;
#if DLD_BEZ_SOL
using BezierSolution; 
#endif
// Used one of the free spline tools in this example. You can import that tool, uncomment the first 
// line (#define DLD_BEZ_SOL) and see how one could use a spline tool to drive Path Painter.
// The tool that was used in this example is called Bezier Solution by yasirkula.
// Get it here: http://bit.ly/3DHFreeSpln
// You can also easily port to the spline tool of your choice.
// Have fun!

namespace Haven.Demo
{
    /// <summary>
    /// A crude Path Painter API example that features a rudimentary undo system.
    /// </summary>
    public class PPAPIExampleSpline : MonoBehaviour
    {
        #region Public, Inspector Memebers

        [Header("Input")]
        [Tooltip("Drag and drop one of the terrains which belongs to the group of terrains to paint on.\n" +
            "Alternatively drag & drop anything that's directly above or below the terrains to be selected." +
            "(Some projects may use more than one separate group of terrains.)")]
        public Transform TerrainsSelector;
#if DLD_BEZ_SOL
        [Tooltip("Drag and drop splines that will drive painting.")]
        public BezierSpline[] Splines;
#endif
        [Tooltip("Drag and drop a GameObjects with childs that drive painting.")]
        public Transform[] Lines;

        [Header("Main Contols")]
        [Tooltip("Drag and drop the SelectionGrid the user will use to select what to paint.")]
        public SelectionGrid PresetSelectionGrid;
        [Tooltip("Drag and drop the dropdown from the GUI.")]
        public Dropdown TargetingDropdown;
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

        #region Enums

        /// <summary>
        /// Type of painting
        /// </summary>
        public enum PaintMode { Paint, Paint3D }

        #endregion

        #region Private Members

        private Painter m_painter;
        private PaintMode m_paintMode = PaintMode.Paint;

        private float m_pathSize = 6f;
        private float m_embankmentSize = 24f;
        private float m_slopeLimit = 25f;
        private float m_ramp = 0f;
        private TerrainLayer m_pathLayer = null;
        private TerrainLayer m_embankmentLayer = null;

        private int m_activeLineSelection = 0;
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
            InitPreset();
            m_painter = new Painter(RecordSnapshot);
        }

        /// <summary>
        /// Validates that everything is in order.
        /// </summary>
        private bool ValidateSetup()
        {
            m_validSetup = true;
            m_optionalButtonsProvided = true;
            if (PresetSelectionGrid == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Selection grid missing. Drag and drop it from the GUI.", gameObject.name);
            }
            if (TargetingDropdown == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: Targeting dropdown missing. Drag and drop it from the GUI.", gameObject.name);
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
            if (TerrainsSelector == null)
            {
                m_validSetup = false;
                Debug.LogErrorFormat("[{0}]: TerrainsSelector missing. Drag and drop one of the terrains which belongs " +
                    "to the group of terrains to paint on.\nAlternatively drag & drop anything that's directly above or below the " +
                    "terrains to be selected. (Some projects may use more than one separate group of terrains.)", gameObject.name);
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
            InitDropdown();
        }

        /// <summary>
        /// Initialises the dropdowns
        /// </summary>
        private void InitDropdown()
        {
            Dropdown.OptionDataList ddList = new Dropdown.OptionDataList();
            ddList.options.Add(new Dropdown.OptionData("Select Line to Paint"));
            ddList.options.Add(new Dropdown.OptionData("Paint all (Bulk Paint)"));
            for (int i = 0; i < Lines.Length; i++)
            {
                ddList.options.Add(new Dropdown.OptionData(Lines[i].name));
            }
#if DLD_BEZ_SOL
            for (int i = 0; i < Splines.Length; i++)
            {
                ddList.options.Add(new Dropdown.OptionData(Splines[i].name));
            } 
#endif
            TargetingDropdown.options = ddList.options;
            TargetingDropdown.onValueChanged.AddListener(SelectTarget);
        }

        /// <summary>
        /// Sets the first preset
        /// </summary>
        private void InitPreset()
        {
            if (Presets.Length > 0)
            {
                SelectionChange(0);
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
        /// Called by the toggles to select path type
        /// </summary>
        public void SelectPaintMode(bool paintMode)
        {
            m_paintMode = paintMode ? PaintMode.Paint : PaintMode.Paint3D;
            PresetSelectionGrid.OnModeChange(m_paintMode);
        }

        /// <summary>
        /// Handles selection change
        /// </summary>
        public void SelectionChange(int val)
        {
            if (val < 0f)
            {
                return;
            }
            m_pathSize = Presets[val].PathSize;
            m_embankmentSize = Presets[val].EmbankmentSize;
            m_slopeLimit = Presets[val].SlopeLimit;
            m_ramp = Presets[val].Ramp;
            m_pathLayer = Presets[val].PathLayer;
            m_embankmentLayer = Presets[val].EmbankmentLayer;
        }

        /// <summary>
        /// Called from GUI to initiate painting
        /// </summary>
        public void SelectTarget(int target)
        {
            // Subtract the dropdown label
            target--;
            if (target < 0)
            {
                // The label was selected
                return;
            }
            Paint(target);
            TargetingDropdown.value = 0;
        }

        #endregion

        #region Point getters from lines/splines

        /// <summary>
        /// Gets the lines to apply the selected painting to 
        /// </summary>
        private List<List<Vector3>> GetLines(int target)
        {
            List<List<Vector3>> lines = new List<List<Vector3>>();
            target--;
            if (target < 0)
            {
                // Apply to all was selected
                foreach (Transform collection in Lines)
                {
                    lines.Add(GetPointsAsChildren(collection));
                }
#if DLD_BEZ_SOL
                foreach (BezierSpline spline in Splines)
                {
                    lines.Add(GetSplinePoints(spline));
                } 
#endif
            }
            else if (target < Lines.Length)
            {
                lines.Add(GetPointsAsChildren(Lines[target]));
            }
            else
            {
#if DLD_BEZ_SOL
                target -= PointCollections.Length;
                lines.Add(GetSplinePoints(Splines[target])); 
#endif
            }

            return lines;
        }

        /// <summary>
        /// Gathers child objects positions as points. This is only for illustration.
        /// It's not a great way to do it with more than two, because the order of children may not be guaranteed.
        /// </summary>
        private List<Vector3> GetPointsAsChildren(Transform parent)
        {
            List<Vector3> points = new List<Vector3>();
            foreach (Transform child in parent)
            {
                points.Add(child.position);
            }
            return points;
        }

#if DLD_BEZ_SOL
        /// <summary>
        /// Gets points from a BezierSpline
        /// </summary>
        private List<Vector3> GetSplinePoints(BezierSpline spline)
        {
            List<Vector3> points = new List<Vector3>();
            float stepping = (m_pathSize * 0.5f) / spline.Length;
            float progress = 0f;
            while (progress < 1f)
            {
                points.Add(spline.GetPoint(progress));
                progress += stepping;
            }
            points.Add(spline.GetPoint(1f));
            return points;
        } 
#endif

        #endregion

        #region Paint Mechanism

        /// <summary>
        /// Paint using the seleted lines
        /// </summary>
        private void Paint(int lineSelection)
        {
            if (!m_validSetup)
            {
                return;
            }
            m_activeLineSelection = lineSelection;
            if (m_undoEnabled)
            {
                m_undoSnapshot = new Dictionary<TerrainData, TerrainSnapshot>();
                m_undoStack.Push(m_undoSnapshot);
            }
            ApplyToLines(GetLines(m_activeLineSelection));
        }

        /// <summary>
        /// Applies the selected painting following the selected lines
        /// </summary>
        private void ApplyToLines(List<List<Vector3>> lines)
        {
            // A better design would be to not care how many lines and always use Bulk Painting, but wanted to show the difference here
            if (lines.Count == 1)
            {
                ApplyToLine(lines[0]);
            }
            else
            {
                m_painter.StartBulkPaint(TerrainsSelector.position);
                foreach (List<Vector3> line in lines)
                {
                    ApplyToLine(line);
                }
                m_painter.EndBulkPaint();
            }
        }

        /// <summary>
        /// Applies the selected painting following the line
        /// </summary>
        private void ApplyToLine(List<Vector3> lines)
        {
            switch (m_paintMode)
            {
                case PaintMode.Paint:
                    m_painter.Paint(lines, Painter.Size(m_pathSize), Painter.EmbankmentSize(m_embankmentSize),
                        Painter.SlopeLimit(m_slopeLimit), Painter.EvenRamp(m_ramp),
                        Painter.Texture(m_pathLayer), Painter.EmbankmentTexture(m_embankmentLayer));
                    break;
                case PaintMode.Paint3D:
                    m_painter.Paint3D(lines, Painter.Size(m_pathSize), Painter.EmbankmentSize(m_embankmentSize),
                        Painter.SlopeLimit(m_slopeLimit), Painter.Texture(m_pathLayer), Painter.EmbankmentTexture(m_embankmentLayer));
                    break;
                default:
                    Debug.LogErrorFormat("[PPAPIExample] Houston! I have no idea what to do with this: {0}", m_paintMode);
                    break;
            }
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
            ApplyToLines(GetLines(m_activeLineSelection));
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
