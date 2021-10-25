using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _healthPoints = 100f;
        private Animator _animator;
        private bool isDead = false;

        private void Start()
        {
            _animator = GetComponent<Animator>();
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
        }
    }
}