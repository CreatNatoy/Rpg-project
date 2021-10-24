using RPG.Movement;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        private Transform _target;
        float _timeSinceLastAttack = 0;
        private Mover _mover;
        private Animator _animator;
        private ActionScheduler _action;

        private void Start()
        {
            _mover = GetComponent<Mover>();
            _action = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime; 

            if(_target == null) return;

            if (_target != null && !GetIsInRange())
                _mover.MoveTo(_target.position);
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }

        }

        private void AttackBehaviour()
        {
            if(_timeSinceLastAttack > _timeBetweenAttacks)
            {
                _animator.SetTrigger("Attack");
                _timeSinceLastAttack = 0;
            }
            
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.position) < _weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            _action.StartAction(this);
            _target = combatTarget.transform;
        }

        public void Cancel()
        {
            _target = null; 
        }

        // Animation Event
        void Hit()
        {

        }
    }
}