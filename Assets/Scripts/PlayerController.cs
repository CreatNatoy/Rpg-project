using UnityEngine;
using RPG.Movement;

namespace RPG.Control
{    
    public class PlayerController : MonoBehaviour
    {
    private Mover _mover;

    private void Start()
    {
        _mover = GetComponent<Mover>();
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
            MoveToCursor();
    }

    private void MoveToCursor()
    {
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        RaycastHit _hit;
        bool hasHit = Physics.Raycast(_ray, out _hit);
        if (hasHit)
        {
            _mover.MoveTo(_hit.point);
        }
    }


    }
}
