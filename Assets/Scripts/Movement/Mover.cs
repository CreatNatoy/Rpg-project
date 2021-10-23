using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 


namespace RPG.Movement
{
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

    public void Stop()
    {
        _navMeshAgent.isStopped = true;
    }

    public void MoveTo(Vector3 destination)
    {
        _navMeshAgent.destination = destination;
        _navMeshAgent.isStopped = false;
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = _navMeshAgent.velocity;  //�������� �������� 
        //transform.InverseTransformDirection()  ��������������� �� �������� ������������ � ��������� ������������  
        Vector3 localVelocity = transform.InverseTransformDirection(velocity); 
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("Blend", speed); 
    }
}
}
