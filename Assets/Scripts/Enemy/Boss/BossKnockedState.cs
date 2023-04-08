using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossKnockedState : BossState
{
    private float _destinationOffset;
    private int _knockbackSpeed;
    private float _knockbackDistance;
    private float _knockbackStopDistance;
    private Vector3? _knockedBackDestination = null;
    private float _destinationEpsilon;
    private float _maxNavSpeed;
    private float _regularStopDistance;
    public void Init(GameObject player, BossController bossController, Rigidbody rb, Animator anim, NavMeshAgent nav, float destinationOffset, int knockbackSpeed, float knockbackDistance, float knockbackStopDistance, float destinationEpsilon, float maxNavSpeed, float regularStopDistance)
    {
        base.Init(player, bossController, rb, anim, nav);
        _destinationOffset = destinationOffset; 
        _knockbackSpeed = knockbackSpeed;
        _knockbackDistance = knockbackDistance;
        _knockbackStopDistance = knockbackStopDistance;
        _destinationEpsilon = destinationEpsilon;
        _maxNavSpeed = maxNavSpeed;
        _regularStopDistance = regularStopDistance;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Nav.speed = _knockbackSpeed;

        var direction = Player.transform.position - transform.position;
        direction.Normalize();
        direction.x *= -1;
        direction.z *= -1;
        direction.y = 0;

        _knockedBackDestination = transform.position + direction * _knockbackDistance;
    }

    public override void OnExit()
    {
        base.OnExit();
        Nav.speed = _maxNavSpeed;
        Nav.stoppingDistance = _regularStopDistance;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if ((_knockedBackDestination.Value - transform.position).magnitude <=
             _destinationOffset + _destinationEpsilon)
        {
            BossController.ChangeBossState(BossController.BossStates.MoveState);
            return;
        }

        MoveTo(_knockedBackDestination.Value);
        Nav.stoppingDistance = _knockbackStopDistance;
    }

    public override void RegularUpdate()
    {
        base.RegularUpdate();
    }

    private void MoveTo(Vector3 destination, bool noOffset = false)
    {
        var rot = destination - transform.position;
        rot.y = 0;

        Rb.AddTorque(new Vector3(rot.z / 2, 0, -rot.x / 2) * _knockbackSpeed);

        // Don't let the enemy climb over the player
        var destinationWithOffset =
            destination - (destination - transform.position).normalized * _destinationOffset;

        Nav.SetDestination(noOffset ? destination : destinationWithOffset);
    }
}
