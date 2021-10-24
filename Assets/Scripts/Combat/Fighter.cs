using RPG.Movement;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float _weaponRange = 2f;
        private Transform _target;
        private Mover _mover;
        private ActionScheduler _action;

        private void Start()
        {
            _mover = GetComponent<Mover>();
            _action = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            if(_target == null) return;

            if (_target != null && !GetIsInRange())
                _mover.MoveTo(_target.position);
            else
                _mover.Cancel();
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
    }
}