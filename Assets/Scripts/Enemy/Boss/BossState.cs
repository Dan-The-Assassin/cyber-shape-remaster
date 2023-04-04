using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BossState : MonoBehaviour
{

    protected GameObject Player;
    protected BossController BossController;
    protected Rigidbody Rb;
    protected Animator Anim;
    protected NavMeshAgent Nav;

    public virtual void Init(GameObject player, BossController bossController, Rigidbody rb, Animator anim, NavMeshAgent nav)
    {
        this.Player = player;
        this.BossController = bossController;
        this.Rb = rb;
        this.Anim = anim;
        this.Nav = nav;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void RegularUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }
}
