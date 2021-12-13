using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 3f;
        [SerializeField] private PatrolPath _patrolPath;
        [SerializeField] private float _wayPointToLerance = 1f;
        [SerializeField] private float _wayPointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] private float _patrolSpeedFraction = 0.2f; 

        private GameObject _player;
        private Health _health;
        private Fighter _fighter;
        private Mover _mover; 

        private Vector3 _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeArrivedAtWayPoint = Mathf.Infinity;
        private int _currentWayPointIndex = 0; 

        private void Start()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _player = GameObject.FindWithTag("Player");
            _guardPosition = transform.position; 

        }

        private void Update()
        {
            if (_health.IsDead()) return;

            if (DistanceToPlayer() && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeArrivedAtWayPoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition;

            if(_patrolPath != null )
            {
                if(AtWayPoint())
                {
                    _timeArrivedAtWayPoint = 0;
                    CycleWayPoint();
                }
               
                nextPosition = GetCurrentWayPoint(); 
            }
            if (_timeArrivedAtWayPoint > _wayPointDwellTime)
            {
                _mover.StartMoveAction(nextPosition, _patrolSpeedFraction);
            }
        }

        private bool AtWayPoint()
        {
            float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWayPoint < _wayPointToLerance; 
        }

        private void CycleWayPoint()
        {
            _currentWayPointIndex = _patrolPath.GetNextIndex(_currentWayPointIndex); 
        }

        private Vector3 GetCurrentWayPoint()
        {
            return _patrolPath.GetWayPoint(_currentWayPointIndex); 
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);
        }

        private bool DistanceToPlayer()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position); 
            return distanceToPlayer < _chaseDistance; 
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance); 
        }

    }
}