using RPG.Movement;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _weaponDamage = 2f;
        private Health _target;
        float _timeSinceLastAttack = Mathf.Infinity; /* Сразу наносил урон без задержки */ 
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
            if(_target.IsDead()) return;

            if (_target != null && !GetIsInRange())
                _mover.MoveTo(_target.transform.position, 1f);
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }

        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            if(_timeSinceLastAttack > _timeBetweenAttacks)
            {
                // This will trigger the Hit() event.
                TriggerAttack();
                _timeSinceLastAttack = 0;
            }

        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("Attack");
            _animator.SetTrigger("Attack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget ==  null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            _action.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
             StopAttack();
            _target = null;
            _mover.Cancel(); 
        }

        private void StopAttack()
        {
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("stopAttack");
        }

        // Animation Event
        private void Hit()
        {
                if(_target == null) {return;}
                //Health healthComponent = _target.GetComponent<Health>();
                _target.TakeDamage(_weaponDamage);
        }
    }
}