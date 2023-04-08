using Constants.Animations;
using Enemy;
using Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossShootState : BossState
{
    private int _projectileCountPerAttackMin;
    private int _projectileCountPerAttackMax;
    private GameObject _projectile;
    private string TelegraphAttackAnim = BossAnimation.TelegraphAttackAnim;

    public void Init(GameObject player, BossController bossController, Rigidbody rb, Animator anim, NavMeshAgent nav, int projectileCountPerAttackMin, int projectileCountPerAttackMax, GameObject projectile)
    {
        base.Init(player, bossController, rb, anim, nav);
        _projectileCountPerAttackMin = projectileCountPerAttackMin;
        _projectileCountPerAttackMax = projectileCountPerAttackMax;
        _projectile = projectile;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Anim.SetBool(TelegraphAttackAnim, true);
    }

    public override void OnExit()
    {
        base.OnExit();
        //just in case the state is stopped early
        Anim.SetBool(TelegraphAttackAnim, false);
        Shoot();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void RegularUpdate()
    {
        base.RegularUpdate();

        if (Anim.GetCurrentAnimatorStateInfo(0).IsName(TelegraphAttackAnim) && Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0)
        {
            BossController.ChangeBossState(BossController.BossStates.MoveState);
        }
    }

    public void Shoot()
    {
        // check if it's time to shoot
        // Instantiate projectileCountPerAttack projectiles in a circle around the enemy
        int projectileCountPerAttack = Random.Range(_projectileCountPerAttackMin, _projectileCountPerAttackMax+1);
        var angle = 360 / projectileCountPerAttack;
        for (var i = 0; i < projectileCountPerAttack; i++)
        {
            var projectileRotation = Quaternion.Euler(0, angle * i, 0);
            var projectilePosition = transform.position + projectileRotation * Vector3.forward * 2;
            Instantiate(_projectile, projectilePosition, projectileRotation);
        }

        Anim.SetBool(TelegraphAttackAnim, false);
    }
}
