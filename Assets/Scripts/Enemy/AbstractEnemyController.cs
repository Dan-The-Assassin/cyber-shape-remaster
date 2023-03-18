using System;
using System.Collections;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public abstract class AbstractEnemyController : MonoBehaviour
    {
        protected float _rollSpeed;
        private float _regularRollSpeed;
        private float _regularNavSpeed;
        private bool _isSlowed = false;
        private bool _isPoisoned = false;
        private Coroutine _slowCoroutine = null;
        private Coroutine _poisonCoroutine = null;
        private float _poisonCounter = 0;
        protected NavMeshAgent _nav;


        [field: SerializeField] private int SlowDuration { get; set; } = 3;
        [field: SerializeField] private int PoisonDelay { get; set; } = 3;
        [field: SerializeField] private float SlowAmount { get; set; } = 0.3f;
        [field: SerializeField] private float PoisonCount { get; set; } = 3;
        [SerializeField] protected GameObject enemyBody;

        public abstract float CollisionDamage { get; }

        private MeshRenderer _meshRenderer;
        private static readonly int IsSlow = Shader.PropertyToID("_isSlow");
        private static readonly int IsPoisoned = Shader.PropertyToID("_isPoisoned");

        protected virtual void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _nav = GetComponent<NavMeshAgent>();
        }

        public abstract void TakeDamage(float damage);

        public void ApplyBulletEffect(DamageInfo damageInfo)
        {
            switch (damageInfo.Effect)
            {
                case BulletEffect.SLOW:
                {
                    Slow();
                    return;
                }
                case BulletEffect.POISON:
                {
                    Poison(damageInfo.Damage);
                    return;
                }
                default:
                {
                    return;
                }
            }
        }

        private IEnumerator StartEffectTimer(int timer, Action onDone)
        {
            yield return new WaitForSeconds(timer);
            onDone();
            _slowCoroutine = null;
        }

        private void OnDoneSlow()
        {
            _rollSpeed = _regularRollSpeed;
            _nav.speed = _regularNavSpeed;
            _isSlowed = false;
            _meshRenderer.material.SetInt(IsSlow, 0);
        }

        private void Slow()
        {
            if (_isSlowed)
            {
                if (_slowCoroutine != null)
                {
                    StopCoroutine(_slowCoroutine);
                }

                _slowCoroutine = StartCoroutine(StartEffectTimer(SlowDuration,OnDoneSlow));
            }

            _isSlowed = true;
            _meshRenderer.material.SetInt(IsSlow, 1);
            _regularRollSpeed = _rollSpeed;
            _regularNavSpeed = _nav.speed;
            _rollSpeed = _regularRollSpeed * SlowAmount;
            _nav.speed = _regularNavSpeed * SlowAmount;

            _slowCoroutine = StartCoroutine(StartEffectTimer(SlowDuration,OnDoneSlow));
        }

        private void OnDonePoison(float poisonDamage)
        {
            Debug.Log(_poisonCounter);
            if (_poisonCounter < PoisonCount)
            {
                TakeDamage(poisonDamage);
                _poisonCounter++;
                _poisonCoroutine = StartCoroutine(StartEffectTimer(PoisonDelay, () => OnDonePoison(poisonDamage)));
            }
            else
            {
                _meshRenderer.material.SetInt(IsPoisoned, 0);
                _isPoisoned = false;
                _poisonCoroutine = null;
            }
        }

        private void Poison(float damage)
        {
            _isPoisoned = true;
            _poisonCounter = 0;
            _meshRenderer.material.SetInt(IsPoisoned, 1);
            if (_poisonCoroutine != null)
            {
                StopCoroutine(_poisonCoroutine);
            }

            _poisonCoroutine = StartCoroutine(StartEffectTimer(PoisonDelay, () => OnDonePoison(damage)));
        }
    }
}