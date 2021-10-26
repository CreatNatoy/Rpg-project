using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{    
    public class PlayerController : MonoBehaviour
    {
    private Mover _mover;
    private Fighter _fighter;
    private Health _health; 

    private void Start()
    {
        _health = GetComponent<Health>(); 
        _mover = GetComponent<Mover>();
        _fighter = GetComponent<Fighter>();
    }

    private void Update()
    {
           if (_health.IsDead()) return; 

           if(InteractWithCombat()) return;
           if(InteractWithMovement()) return; 
    }

    private bool InteractWithCombat() 
    {
        RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
        foreach (RaycastHit hit in hits)
        {
            CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;
            if(!_fighter.CanAttack(target.gameObject))
            {
                continue;
            }

            if(Input.GetMouseButtonDown(0))
                _fighter.Attack(target.gameObject);
            
            return true;    
        }
        return false;
    }

    private bool InteractWithMovement()
        {
            RaycastHit _hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out _hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                    _mover.StartMoveAction(_hit.point);
                return true;
            }
            return false;
        }

    private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
