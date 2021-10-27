using RPG.Core;
using UnityEngine;
using UnityEngine.AI; 


namespace RPG.Movement
{
[RequireComponent(typeof(NavMeshAgent))]
public class Mover : MonoBehaviour, IAction
{
    [SerializeField] private Transform _target;

    private NavMeshAgent _navMeshAgent;
    private ActionScheduler _action;
        private Health _health; 

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>(); 
        _action = GetComponent<ActionScheduler>();
        _health = GetComponent<Health>(); 
    }

    private void Update()
    {
        _navMeshAgent.enabled = !_health.IsDead(); 
        UpdateAnimator();
    }

    public void StartMoveAction(Vector3 destination)
    {
        _action.StartAction(this);
        MoveTo(destination);
    }

    public void Cancel()
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
        Vector3 velocity = _navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity); 
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("Blend", speed); 
    }
}
}
