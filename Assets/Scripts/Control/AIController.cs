using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float _chaseDistance = 5f;

        private GameObject _player;
        private Health _health;
        private Fighter _fighter;
        private Mover _mover; 
        private Vector3 _guardPosition; 

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

           if(DistanceToPlayer() && _fighter.CanAttack(_player))
            {
                _fighter.Attack(_player); 
            }
           else
            {
                _mover.StartMoveAction(_guardPosition); 
            }    
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