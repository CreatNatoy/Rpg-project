using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    /*LateUpdate: LateUpdate() ���������� ���� ��� � �����, ����� ���������� Update().
     * ����� �������, ������� �������������� � Update() ����� ���������, ��� ������ LateUpdate().
     * �������� �������������� LateUpdate() ������ �������� �������� �� ������� �� �������� ����. 
     * ���� �� ����������� �������� ������ ��������� � ������� Update(),
     * �� �������� ������ � �������� � ����������������� ������ ����� � ������� LateUpdate(). 
     * ��� ����� �������������, ��� �������� ������ ��������� ����� �������, � �������� ���� ������������. */
    private void LateUpdate()
    {
        transform.position = _target.position; 
    }
}
