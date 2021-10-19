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
        //  Input.mousePosition  �������� ��������� ����
        //  Camera.main.ScreenPointToRay()   ���������� ���, ������ �� ������ ����� ����� �� ������.
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ��� (���������� �� �����, � ������� � ����������� ����������)
        RaycastHit _hit; // ���������� ��������� �� ����
        //  Debug.DrawRay(_lastRay.origin, _lastRay.direction * 100);
        //  Physics.Raycast() ���������� ������, ���� ��� ���������� ���������, � ��������� ������ - ����.
        bool hasHit = Physics.Raycast(_ray, out _hit);
        if (hasHit)
        {
            _mover.MoveTo(_hit.point);
        }
    }


}
