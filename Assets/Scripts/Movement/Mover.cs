using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 


[RequireComponent(typeof(NavMeshAgent))]
public class Mover : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private NavMeshAgent _navMeshAgent;
    private Ray _ray; // ��� (���������� �� �����, � ������� � ����������� ����������)
    private RaycastHit _hit; // ���������� ��������� �� ����

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
        //  Input.mousePosition  �������� ��������� ����
        //  Camera.main.ScreenPointToRay()   ���������� ���, ������ �� ������ ����� ����� �� ������.
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //  Debug.DrawRay(_lastRay.origin, _lastRay.direction * 100);
        //  Physics.Raycast() ���������� ������, ���� ��� ���������� ���������, � ��������� ������ - ����.
        bool hasHit = Physics.Raycast(_ray, out _hit); 
        if(hasHit)
        {
            _navMeshAgent.destination = _hit.point;
        }
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;  //�������� �������� 
        //transform.InverseTransformDirection()  ��������������� �� �������� ������������ � ��������� ������������  
        Vector3 localVelocity = transform.InverseTransformDirection(velocity); 
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("Blend", speed); 
    }
}
