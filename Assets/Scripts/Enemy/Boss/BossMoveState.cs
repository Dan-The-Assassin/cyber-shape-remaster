using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossMoveState : BossState
{
    private float _rollSpeed;
    private float _destinationOffset;
    /// <summary>
    /// The max distance after which the player is considered far from the boss
    /// </summary>
    protected float MaxDistanceFar;
    /// <summary>
    /// The minimum distance under which the player is considered to be close to the boss
    /// </summary>
    protected float MinDistanceClose;

    public void Init(GameObject player, BossController bossController, Rigidbody rb, Animator anim, NavMeshAgent nav, float rollSpeed, float destinationOffset,float maxDistanceFar, float minDistanceClose)
    {
        base.Init(player, bossController, rb, anim, nav);
        _rollSpeed = rollSpeed;
        _destinationOffset = destinationOffset;
        MinDistanceClose = minDistanceClose;
        MaxDistanceFar = maxDistanceFar;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (Player.transform.hasChanged)
        {
            var destination = Player.transform.position;
            MoveTo(destination);
        }
    }

    public override void RegularUpdate()
    {
        base.RegularUpdate();

        float distance = Vector3.Distance(transform.position, Player.transform.position);

        if (distance < MaxDistanceFar && distance > MinDistanceClose)
        {
            BossController.ChangeBossState(BossController.BossStates.ShootState);
        }
        if (distance >= MaxDistanceFar)
        {
            BossController.ChangeBossState(BossController.BossStates.RamState);
        }
        if (distance <= MinDistanceClose)
        {
            bool chance = (Random.value >= 0.5);
            if(chance)
            {
                BossController.ChangeBossState(BossController.BossStates.ShootState);
            }
            else
            {
                BossController.ChangeBossState(BossController.BossStates.RamState);
            }
        }
    }

    private void MoveTo(Vector3 destination, bool noOffset = false)
    {
        var rot = destination - transform.position;
        rot.y = 0;

        Rb.AddTorque(new Vector3(rot.z / 2, 0, -rot.x / 2) * _rollSpeed);

        // Don't let the enemy climb over the player
        var destinationWithOffset =
            destination - (destination - transform.position).normalized * _destinationOffset;

        Nav.SetDestination(noOffset ? destination : destinationWithOffset);
    }
}
