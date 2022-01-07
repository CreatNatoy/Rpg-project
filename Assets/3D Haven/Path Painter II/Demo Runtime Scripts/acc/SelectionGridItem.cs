using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Haven.Demo
{
    /// <summary>
    /// Pull this onto toggles for selection grid
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class SelectionGridItem : MonoBehaviour
    {
        #region Public/Inspector Memebers

        [Tooltip("Should this be available for Paint3D?\nRamp setting is not used by" +
            "Paint3D, so ramp versions of an item make no sense for it.")]
        [SerializeField]
        private bool m_paint3DType = true;

        [Tooltip("Must be set to a unique value unless used as an empty shelf. " +
            "In that case set to -1 if no item on it.")]
        public int Value = -99;

        #endregion

        #region Public properties

        /// <summary>
        /// Should this be available for 3D painting? 
        /// </summary>
        public bool Paint3DType { get { return m_paint3DType; } }

        #endregion

        #region Private Members

        private SelectionGrid m_parentGrid;
        private Toggle m_toggle;
        private Image m_image;
        private Image m_disabledImage;

        #endregion

        #region Initialisation

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        private void Start()
        {
            m_toggle = GetComponent<Toggle>();
            foreach (Transform child in transform)
            {
                if (child.name == "Toggle Image")
                {
                    m_image = child.GetComponent<Image>();
                }
                else if (child.name == "Disabled Image")
                {
                    m_disabledImage = child.GetComponent<Image>();
                }
            }
            if (Value < 0)
            {
                if (Value < -1)
                {
                    Debug.LogWarningFormat("[{0}]: Value was not set. Deactivating self.", gameObject.name);
                }
                Diable();
                return;
            }
            m_parentGrid = transform.parent.GetComponent<SelectionGrid>();
            if (m_parentGrid == null)
            {
                Debug.LogErrorFormat("[{0}]: Parent GridSelector missing. Make sure this " +
                    "toggle is the child of a GameObject (generally Panel) that contains a " +
                    "GridSelector script component.", gameObject.name);
                Diable();
                return;
            }
            if (!m_parentGrid.Checkin(this))
            {
                Diable();
                return;
            }
            // Add Listener
            m_toggle.onValueChanged.AddListener(OnValueChange);
        }

        #endregion

        #region GUI Interface

        /// <summary>
        /// Used to swtich painting on/off
        /// </summary>
        public void OnValueChange(bool on)
        {
            m_parentGrid.OnSelectionChange(on ? Value : -1);
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Select this item
        /// </summary>
        public void Select()
        {
            m_toggle.isOn = true;
        }

        /// <summary>
        /// Called when paint mode changes. Returns true if the item was selected, but had to be disabled.
        /// </summary>
        public bool OnModeChange(PPAPIExampleSpline.PaintMode mode)
        {
            if (Value < 0)
            {
                return false;
            }
            switch (mode)
            {
                case PPAPIExampleSpline.PaintMode.Paint:
                    // Everything available in this mode
                    Enable();
                    return false;
                case PPAPIExampleSpline.PaintMode.Paint3D:
                    if (!m_paint3DType)
                    {
                        Diable();
                        return m_toggle.isOn;
                    }
                    return false;
            }
            Debug.LogErrorFormat("[PPAPIExample] Houston! I have no idea what to do with this: {0}", mode);
            return false;
        }

        #endregion

        #region Private Utility

        /// <summary>
        /// Enables this toggle
        /// </summary>
        private void Enable()
        {
            m_toggle.interactable = true;
            if (m_image != null)
            {
                m_image.enabled = true;
            }
            if (m_disabledImage != null)
            {
                m_disabledImage.enabled = false;
            }
        }

        /// <summary>
        /// Disables this toggle
        /// </summary>
        private void Diable()
        {
            if (m_image != null)
            {
                m_image.enabled = false;
            }
            if (m_disabledImage != null)
            {
                m_disabledImage.enabled = true;
            }
            m_toggle.interactable = false;
        }

        #endregion
    }
}
