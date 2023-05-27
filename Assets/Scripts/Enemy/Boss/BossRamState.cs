using Constants.Animations;
using Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

public class BossRamState : BossState
{
    private bool _isRamming;
    private bool _isRamFollowing = false;

    private float _ramSpeed;
    private float _ramStopDistance;
    private float _destinationOffset;
    private float _destinationEpsilon;
    private int _ramFollowTime;
    private int _ramFollowDuration;
    private Vector3 _ramDestination;
    private float _telegraphSpeed;
    private float _sizeChange;
    private Vector3 _defaultSize;
    private TrailRenderer _trail;

    private float _regularStopDistance;
    private float _maxNavSpeed;
    public void Init(GameObject player, BossController bossController, Rigidbody rb, Animator anim, NavMeshAgent nav, float ramSpeed, float ramStopDistance, float destinationOffset, float destinationEpsilon, int ramFollowDuration, float regularStopDistance, float maxNavSpeed, float telegraphSpeed, TrailRenderer trail)
    {
        base.Init(player, bossController, rb, anim, nav);
        _ramSpeed = ramSpeed;
        _ramStopDistance = ramStopDistance;
        _destinationOffset = destinationOffset;
        _destinationEpsilon = destinationEpsilon;
        _ramFollowDuration = ramFollowDuration;
        _maxNavSpeed = maxNavSpeed;
        _regularStopDistance = regularStopDistance;
        _telegraphSpeed = telegraphSpeed;
        _trail = trail;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Anim.SetBool(BossAnimation.TelegraphRamAttackAnim, true);
        _isRamming = false;
        var rot = Player.transform.position - transform.position;
        Rb.maxAngularVelocity = _telegraphSpeed;
        Rb.AddTorque(new Vector3(rot.z / 2, 0, -rot.x / 2) * _telegraphSpeed);
        _sizeChange = 1.0f;
        _defaultSize = transform.localScale;
        _trail.emitting = true;
    }

    public override void OnExit()
    {
        base.OnExit();
        Anim.SetBool(BossAnimation.TelegraphRamAttackAnim, false);
        Nav.stoppingDistance = _regularStopDistance;
        Nav.speed = _maxNavSpeed;
        Rb.maxAngularVelocity = 7;
        transform.localScale = _defaultSize;
        _trail.time = 1.0f;
        _trail.emitting = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if(_isRamming)
        {
            if (_isRamFollowing)
            {
                if (Time.time * 1000 - _ramFollowTime < _ramFollowDuration)
                {
                    _ramDestination = Player.transform.position;
                }
                else
                {
                    _isRamFollowing = false;
                }
            }

            // If the destination is locked and we reached it, stop ramming
            // If the destination is not locked, the stopping is handled on collision
            var distanceToLockedDestination = (_ramDestination - transform.position).magnitude;
            if (distanceToLockedDestination <= _destinationEpsilon + _ramStopDistance)
            {
                BossController.ChangeBossState(BossController.BossStates.MoveState);
                return;
            }
            _sizeChange = Mathf.Max(_sizeChange - 0.15f * Time.deltaTime, 1.0f);
            transform.localScale = new Vector3(_sizeChange, _sizeChange, _sizeChange);
            _trail.time -= Time.deltaTime * 0.2f;

            MoveTo(_ramDestination, _isRamFollowing);
        }
    }

    public override void RegularUpdate()
    {
        base.RegularUpdate();

        if (!_isRamming)
        {
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName(BossAnimation.TelegraphRamAttackAnim) && Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0)
            {
                Rb.angularVelocity = Vector3.zero;
                Anim.SetBool(BossAnimation.TelegraphRamAttackAnim, false);
                _isRamming = true;
                Nav.speed = _ramSpeed;
                Nav.stoppingDistance = _ramStopDistance;
                _isRamFollowing = true;
                _ramFollowTime = (int)(Time.time * 1000);
                _ramDestination = Player.transform.position;
            }
            else
            {
                var rot = Player.transform.position - transform.position;
                Rb.AddTorque(new Vector3(rot.z / 2, 0, -rot.x / 2) * _telegraphSpeed);
                _sizeChange = Mathf.Min(_sizeChange + 0.2f * Time.deltaTime, 1.25f);
                transform.localScale = new Vector3(_sizeChange, _sizeChange, _sizeChange);
            }
        }
    }

    private void MoveTo(Vector3 destination, bool noOffset = false)
    {
        var rot = destination - transform.position;
        rot.y = 0;

        Rb.AddTorque(new Vector3(rot.z / 2, 0, -rot.x / 2) * _telegraphSpeed);

        // Don't let the enemy climb over the player
        var destinationWithOffset =
            destination - (destination - transform.position).normalized * _destinationOffset;
        Nav.SetDestination(noOffset ? destination : destinationWithOffset);
    }
}
