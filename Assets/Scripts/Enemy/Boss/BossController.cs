using Constants;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;
using Scene = Constants.Scene;

namespace Enemy
{
    public class BossController : AbstractEnemyController
    {
        [SerializeField] private GameObject projectile;
        private const int projectileCountPerAttackMin = 6;
        private const int projectileCountPerAttackMax = 8;

        #region Health variables
        private const int MaxHp = 100;
        private Canvas _healthBar;
        private Slider _healthBarSlider;
        public float health = MaxHp;
        #endregion

        #region Component/Object Variables
        private GameObject _player;
        private Rigidbody _rigidbody;
        private Camera _camera;
        private GameManager _gameManager;
        private Animator _animator;
        private TrailRenderer _trail;
        #endregion

        #region Movement Variables
        private const int MaxRollSpeed = 8; //initially 8
        private const int MaxNavSpeed = 2;  //initially 2
        private const int RamSpeed = 16;
        private const int KnockbackSpeed = 6;

        private const int RamFollowDuration = 1 * 1000;
        private bool _canDealRamDamage = true;
        private const int RegularStopDistance = 5;
        private const float KnockbackStopDistance = 2f;
        private const float RamStopDistance = 1;
        private const float KnockbackDistance = 15;
        /// <summary>
        /// The max distance after which the player is considered far from the boss
        /// </summary>
        private const float MaxDistanceFar = 13.0f;
        /// <summary>
        /// The minimum distance under which the player is considered to be close to the boss
        /// </summary>
        private const float MinDistanceClose = 7.0f;
        private const float DestinationOffset = 1f;
        private const float DestinationEpsilon = 0.6f;
        private const float TelegraphRamSpeed = 12.0f;
        #endregion

        /// <summary>
        /// Cooldown till boss can shoot again in seconds
        /// </summary>
        private const float _shootCooldown = 2.0f;
        private bool _canShoot;
        /// <summary>
        /// Cooldown till boss can ram again in seconds
        /// </summary>
        private const float _ramCooldown = 4.0f;
        private bool _canRam;
        private const float RamDamage = 2;
        private const int Score = 1000;

        #region State Variables
        private BossState _activeState;
        private BossMoveState _moveState;
        private BossRamState _ramState;
        private BossKnockedState _knockedState;
        private BossShootState _shootState;

        public enum BossStates
        {
            MoveState,
            RamState,
            KnockedState,
            ShootState
        }
        #endregion

        #region State Functions
        public void ChangeBossState(BossStates state)
        {
            if (InvalidStateChange(state)) return;

            _activeState.OnExit();
            CheckCooldowns(state);
            SetActiveState(state);
            _activeState.OnEnter();
        }

        private void CheckCooldowns(BossStates state)
        {
            switch(state)
            {
                case BossStates.RamState: _canRam = false; StartCoroutine(RamCooldown()); break;
                case BossStates.ShootState: _canShoot = false; StartCoroutine(ShootCooldown()); break;
                case BossStates.MoveState: _canDealRamDamage = true; break;
            }
        }

        private bool InvalidStateChange(BossStates state)
        {
            return (state == BossStates.ShootState && !_canShoot) ||
                   (state == BossStates.RamState && !_canRam) || (state == BossStates.KnockedState && _activeState == _knockedState);
            //return false;
        }

        private void SetActiveState(BossStates state)
        {
            _activeState = state switch
            {
                BossStates.MoveState => _moveState,
                BossStates.RamState => _ramState,
                BossStates.KnockedState => _knockedState,
                BossStates.ShootState => _shootState,
                _ => _moveState
            };
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _healthBar = GetComponentInChildren<Canvas>();
            _healthBarSlider = _healthBar.GetComponentInChildren<Slider>();

            #region Get Components
            _player = GameObject.FindWithTag("Player");
            _rigidbody = enemyBody.GetComponent<Rigidbody>();
            _camera = Camera.main;
            _animator = GetComponent<Animator>();
            _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            _trail = GetComponent<TrailRenderer>();
            #endregion

            _moveState = gameObject.AddComponent<BossMoveState>();
            _ramState = gameObject.AddComponent<BossRamState>();
            _knockedState = gameObject.AddComponent<BossKnockedState>();
            _shootState = gameObject.AddComponent<BossShootState>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            SetHealth(MaxHp);
            _rollSpeed = MaxRollSpeed;
            _nav.speed = MaxNavSpeed;
            _trail.emitting = false;

            #region Initiate States
            _moveState.Init(_player, this, _rigidbody, _animator, _nav, _rollSpeed, DestinationOffset, MaxDistanceFar, MinDistanceClose);
            _shootState.Init(_player, this, _rigidbody, _animator, _nav, projectileCountPerAttackMin, projectileCountPerAttackMax, projectile);
            _ramState.Init(_player, this, _rigidbody, _animator, _nav, RamSpeed, RamStopDistance, DestinationOffset, DestinationEpsilon, RamFollowDuration, RegularStopDistance, MaxNavSpeed, TelegraphRamSpeed, _trail);
            _knockedState.Init(_player, this, _rigidbody, _animator, _nav, DestinationOffset, KnockbackSpeed, KnockbackDistance, KnockbackStopDistance, DestinationEpsilon, MaxNavSpeed, RegularStopDistance);
            _activeState = _moveState;

            _canShoot = true;
            _canRam = true;
            #endregion
        }

        private void SetHealth(float health)
        {
            _healthBarSlider.value = health / MaxHp;
        }

        // Update is called once per frame
        private void Update()
        {
            _healthBar.transform.rotation = _camera.transform.rotation;
            _activeState.RegularUpdate();
            if (health <= 0)
            {
                // TODO: Add death animation
                if (SceneManager.GetActiveScene().buildIndex == (int)Scene.Level5)
                {
                    _gameManager.DisplayGameWonText();
                }

                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            _activeState.PhysicsUpdate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Player))
            {
                if(_activeState != _ramState)
                ChangeBossState(BossStates.KnockedState);
            }
        }

        public override float CollisionDamage
        {
            get
            {
                if (_activeState != _knockedState)
                {
                    if (_canDealRamDamage)
                    {
                        _canDealRamDamage = false;
                        return RamDamage;
                    }
                }
                return 0;
            }
        }

        public override void TakeDamage(float damage)
        {
            health -= damage;
            SetHealth(health);
            if (health <= 0)
            {
                _player.GetComponent<Player>().AddScore(Score);
            }
        }

        private IEnumerator RamCooldown()
        {
            yield return new WaitForSeconds(_ramCooldown);
            _canRam = true;
        }

        private IEnumerator ShootCooldown()
        {
            yield return new WaitForSeconds(_shootCooldown);
            _canShoot = true;
        }
    }
}
