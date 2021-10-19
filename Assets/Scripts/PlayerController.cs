using UnityEngine;

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
        //  Input.mousePosition  сообщает положение мыши
        //  Camera.main.ScreenPointToRay()   ¬озвращает луч, идущий от камеры через точку на экране.
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition); // луч (ничанающий от точки, и идущиее в направление бесконечно)
        RaycastHit _hit; // информаци€ получана€ из луча
        //  Debug.DrawRay(_lastRay.origin, _lastRay.direction * 100);
        //  Physics.Raycast() ¬озвращает истину, если луч пересекает коллайдер, в противном случае - ложь.
        bool hasHit = Physics.Raycast(_ray, out _hit);
        if (hasHit)
        {
            _mover.MoveTo(_hit.point);
        }
    }


}
