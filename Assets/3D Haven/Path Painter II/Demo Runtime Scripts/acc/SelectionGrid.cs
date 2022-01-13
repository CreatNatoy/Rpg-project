using System.Collections.Generic;
using UnityEngine;
using System;

namespace Haven.Demo
{
    /// <summary>
    /// Pull this onto the panel that contains the toggles used for grid selection
    /// </summary>
    public class SelectionGrid : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// Set the maximum selectable value in Awake
        /// </summary>
        [HideInInspector]
        public int MaxValue = -1;

        /// <summary>
        /// Delegate that gets invoked on value change
        /// </summary>
        public Action<int> OnValueChange
        {
            set
            {
                m_onValueChange -= value;
                m_onValueChange += value;
            }
        }
        private Action<int> m_onValueChange;

        #endregion

        #region Private Members

        private Dictionary<int, SelectionGridItem> m_items = new Dictionary<int, SelectionGridItem> ();
        private HashSet<int> m_valueSet = new HashSet<int>();
        private SelectionGridItem m_defaultItem = null;

        #endregion

        #region Initialisation

        /// <summary>
        /// Child toggles use this to checkin
        /// </summary>
        public bool Checkin(SelectionGridItem item)
        {
            if (MaxValue < 0)
            {
                Debug.LogWarningFormat("[{0}]: Maximum value was not set. The parent of the SelectionGrid must" +
                    " set the maximum selectable value in Awake", gameObject.name);
                return false;
            }
            if (MaxValue < item.Value)
            {
                Debug.LogWarningFormat("[{0}]: The value of Toggle.GridSelectorItem {1} exceeds the maximum value." +
                    "Make sure to set a unique value that's between 0 and {2} for each item. This item will be disabled.", 
                    gameObject.name, item.name);
                return false;
            }
            if (m_valueSet.Contains(item.Value))
            {
                Debug.LogWarningFormat("[{0}]: The value of Toggle.GridSelectorItem {1} is already taken." +
                    "Make sure to set a unique value for each item. This item will be disabled.", gameObject.name, item.name);
                return false;
            }
            m_items[item.Value] = item;
            m_valueSet.Add(item.Value);
            if (m_defaultItem == null || (item.Paint3DType && item.Value < m_defaultItem.Value))
            {
                m_defaultItem = item;
            }
            return true;
        }

        #endregion

        #region GUI Interface

        /// <summary>
        /// Called by items when value changes
        /// </summary>
        public void OnSelectionChange(int val)
        {
            if (m_onValueChange == null)
            {
                return;
            }
            m_onValueChange(val);
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Called when paint mode changes
        /// </summary>
        public void OnModeChange(PPAPIExampleSpline.PaintMode mode)
        {
            foreach (int key in m_items.Keys)
            {
                if (m_items[key].OnModeChange(mode) && m_defaultItem != null)
                {
                    m_defaultItem.Select();
                }                
            }
        }

        #endregion
    }
}
