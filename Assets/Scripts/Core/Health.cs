using UnityEngine;

namespace RPG.Core  /*название папки где лежат */
{ 
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _healthPoints = 100f;
        private Animator _animator;
        private bool isDead = false;
        private ActionScheduler _actionScheduler;

        public bool IsDead() { return isDead; }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>(); 
        }


        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            if(_healthPoints == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if(isDead) return;

            isDead = true;
            _animator.SetTrigger("Die");
            _actionScheduler.CancelCurrentAction(); 
        }
    }
}