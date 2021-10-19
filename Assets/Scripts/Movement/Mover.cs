using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 


[RequireComponent(typeof(NavMeshAgent))]
public class Mover : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>(); 
    }

    private void Update()
    {   
        UpdateAnimator();
    }

    public void MoveTo(Vector3 destination)
    {
        GetComponent<NavMeshAgent>().destination = destination;
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
