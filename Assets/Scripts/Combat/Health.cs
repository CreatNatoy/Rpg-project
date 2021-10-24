using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _health = 100f;

        public void TakeDamage(float damage)
        {
            _health = Mathf.Max(_health - damage, 0);
            print(_health);
        }
    }
}