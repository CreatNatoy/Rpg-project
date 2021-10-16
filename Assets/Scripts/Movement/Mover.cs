using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 


[RequireComponent(typeof(NavMeshAgent))]
public class Mover : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private NavMeshAgent _navMeshAgent;
    private Ray _ray; // луч (ничанающий от точки, и идущиее в направление бесконечно)
    private RaycastHit _hit; // информация получаная из луча

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>(); 
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        
        UpdateAnimator();
    }

    private void MoveToCursor()
    {
        //  Input.mousePosition  сообщает положение мыши
        //  Camera.main.ScreenPointToRay()   Возвращает луч, идущий от камеры через точку на экране.
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //  Debug.DrawRay(_lastRay.origin, _lastRay.direction * 100);
        //  Physics.Raycast() Возвращает истину, если луч пересекает коллайдер, в противном случае - ложь.
        bool hasHit = Physics.Raycast(_ray, out _hit); 
        if(hasHit)
        {
            _navMeshAgent.destination = _hit.point;
        }
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;  //получаем скорость 
        //transform.InverseTransformDirection()  переобразование из мирового пространство в локальное пространство  
        Vector3 localVelocity = transform.InverseTransformDirection(velocity); 
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("Blend", speed); 
    }
}
