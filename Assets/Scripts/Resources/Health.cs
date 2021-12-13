using UnityEngine;
using RPG.Saving;
using UnityEngine.Events;
using RPG.Core;
using RPG.Stats;

namespace RPG.Resources
{ 
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float _healthPoints = 100f;

        private Animator _animator;
        private bool isDead = false;
        private ActionScheduler _actionScheduler;

        public event UnityAction<float> UpdateHealthBar; 

        public bool IsDead() { return isDead; }

        private void Start()
        {
            _healthPoints = GetComponent<BaseStats>().GetHealth();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>(); 
        }


        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            UpdateHealthBar?.Invoke(_healthPoints);
            if (_healthPoints == 0)
            {
                Die();
            }
        }

        public float GetHealthBar()
        {
            return _healthPoints * 0.01f; 
        }

        private void Die()
        {
            if(isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return _healthPoints;
        }

        public void RestoreState(object state)
        {
            _healthPoints = (float)state;

            if(_healthPoints == 0)
            {
                Die();
            }
        }
    }
}