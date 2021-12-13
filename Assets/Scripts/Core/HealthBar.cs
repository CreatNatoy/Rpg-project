using RPG.Resources;
using UnityEngine;

namespace RPG.Core
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health _healthComponent = null;
        [SerializeField] private RectTransform _foreground = null;
        [SerializeField] private Canvas _rootCanvas = null;

        /*      private void Update()
              {
                  if(_healthComponent.GetHealthBar() <= 0 )
                  {
                      _rootCanvas.enabled = false;
                      return; 
                  }
                  else
                  _foreground.localScale = new Vector3(_healthComponent.GetHealthBar(), 1, 1); 
              }
        */
        private void OnEnable()
        {
            _healthComponent.UpdateHealthBar += UpdateHealthBar;
        }

        private void OnDisable()
        {
            _healthComponent.UpdateHealthBar -= UpdateHealthBar;
        }

        public void UpdateHealthBar(float zero)
        {
            if (_healthComponent.GetHealthBar() <= 0)
            {
                _rootCanvas.enabled = false;
                return;
            }
            else
                _foreground.localScale = new Vector3(_healthComponent.GetHealthBar(), 1, 1);
        }

    }
}
