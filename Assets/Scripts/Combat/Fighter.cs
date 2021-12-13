using RPG.Movement;
using RPG.Core;
using UnityEngine;
using RPG.Saving;
using RPG.Resources;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable 
    {

        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private Weapon _defaultWeapon = null;


        private Health _target;
        float _timeSinceLastAttack = Mathf.Infinity; 
        private Mover _mover;
        private Animator _animator;
        private ActionScheduler _action;
        Weapon _currentWeapon = null; 

        private void Start()
        {
            _mover = GetComponent<Mover>();
            _action = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();

            if(_currentWeapon == null)
            EquipWeapon(_defaultWeapon); 
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

        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon = weapon; 
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(_rightHandTransform, _leftHandTransform, animator); 
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
            return Vector3.Distance(transform.position, _target.transform.position) < _currentWeapon.GetRange();
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

        private void Hit()
        {
                if(_target == null) {return;}

            if (_currentWeapon.HasProjectile())
                _currentWeapon.LaunchProjectile(_rightHandTransform, _leftHandTransform, _target);
            else
                _target.TakeDamage(_currentWeapon.GetGamage());
        }

        void Shoot()
        {
            Hit(); 
        }

        object ISaveable.CaptureState()
        {
            return _currentWeapon.name; 
        }

        void ISaveable.RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}