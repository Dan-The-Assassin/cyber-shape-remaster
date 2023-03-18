using Constants;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = Constants.Scene;

namespace Enemy
{
    public class FinalBossController : AbstractEnemyController
    {
        [SerializeField] private GameObject projectile;

        [SerializeField] private int projectileCountPerAttack = 6;

        // Time between attacks in milliseconds
        [SerializeField] private int timeBetweenAttacks = 5 * 1000;
        [SerializeField] private int timeBetweenRamAttacks = 15 * 1000;
        [SerializeField] private int ramFollowDuration = 1 * 1000;

        private const int MaxRollSpeed = 8;
        private const int MaxNavSpeed = 2;
        private const int RamSpeed = 16;
        private const int KnockbackSpeed = 6;
        private const int MaxHp = 100;
        private const int Score = 1000;
        private const int RegularStopDistance = 5;
        private const float RamStopDistance = 1;

        private const float KnockbackStopDistance = 2f;
        private const float KnockbackDistance = 15;
        private const float RamDamage = 2;

        // This is the radius of the sphere that is used to detect the player
        private const float DestinationOffset = 1f;
        private const float DestinationEpsilon = 0.6f;

        private Canvas _healthBar;
        private Slider _healthBarSlider;
        private GameObject _player;
        private Rigidbody _rigidbody;
        private Camera _camera;
        private int _lastAttackTime = 0;
        private int _lastRamAttackTime = 0;
        private bool _canDealRamDamage = true;
        private bool _isRamFollowing = false;
        private Vector3? _ramDestination = null;
        private int _ramFollowTime = 0;
        private GameManager _gameManager;

        private bool _isRamAttack => _isRamFollowing || _ramDestination != null;

        // This is true while telegraphing attack AND while attacking
        private bool _isTelegraphingOrAttacking = false;
        private Animator _animator;
        private Vector3? _knockedBackDestination = null;

        public float health = MaxHp;

        private static readonly int TelegraphAttackAnimTrig = Animator.StringToHash("TelegraphAttack");
        private static readonly int TelegraphRamAttackAnimTrig = Animator.StringToHash("TelegraphRamAttack");


        private void Start()
        {
            SetHealth(MaxHp);
            _rollSpeed = MaxRollSpeed;
            _nav.speed = MaxNavSpeed;
        }

        protected override void Awake()
        {
            base.Awake();
            _player = GameObject.FindWithTag("Player");
            _healthBar = GetComponentInChildren<Canvas>();
            _healthBarSlider = _healthBar.GetComponentInChildren<Slider>();
            _rigidbody = enemyBody.GetComponent<Rigidbody>();
            _camera = Camera.main;
            _animator = GetComponent<Animator>();
            _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }

        private void MoveTo(Vector3 destination, bool noOffset = false)
        {
            var rot = destination - enemyBody.transform.position;
            rot.y = 0;

            _rigidbody.AddTorque(new Vector3(rot.z / 2, 0, -rot.x / 2) * _rollSpeed);

            // Don't let the enemy climb over the player
            var destinationWithOffset =
                destination - (destination - enemyBody.transform.position).normalized * DestinationOffset;
            _nav.SetDestination(noOffset ? destination : destinationWithOffset);
        }

        private void HandleKnockBackUpdate()
        {
            if (_knockedBackDestination == null)
            {
                return;
            }

            if ((_knockedBackDestination.Value - enemyBody.transform.position).magnitude <=
                DestinationOffset + DestinationEpsilon)
            {
                StopRamAttack();
                return;
            }

            MoveTo(_knockedBackDestination.Value);
            _nav.stoppingDistance = KnockbackStopDistance;
        }

        private void HandleRamAttackUpdate()
        {
            if (_isRamFollowing && Time.time * 1000 - _ramFollowTime > ramFollowDuration)
            {
                // Lock on the last position of the player
                _isRamFollowing = false;
                _ramDestination = _player.transform.position;
            }

            var destination = _ramDestination ?? _player.transform.position;

            // If the destination is locked and we reached it, stop ramming
            // If the destination is not locked, the stopping is handled on collision
            var distanceToLockedDestination = (destination - enemyBody.transform.position).magnitude;
            if (!_isRamFollowing &&
                distanceToLockedDestination <=
                DestinationEpsilon + RamStopDistance)
            {
                StopRamAttack();
                return;
            }

            MoveTo(destination, _isRamFollowing);
            _nav.stoppingDistance = RamStopDistance;
        }

        private void Update()
        {
            _healthBar.transform.rotation = _camera.transform.rotation;
            if (_knockedBackDestination != null)
            {
                HandleKnockBackUpdate();
                return;
            }

            if (_isRamAttack)
            {
                HandleRamAttackUpdate();
                return;
            }

            if (_player.transform.hasChanged)
            {
                var destination = _player.transform.position;
                MoveTo(destination);
                HandleAttacks();
            }

            if (health <= 0)
            {
                // TODO: Add death animation
                if (SceneManager.GetActiveScene().buildIndex == (int) Scene.Level5)
                {
                    _gameManager.DisplayGameWonText();
                }

                Destroy(gameObject);
            }
            
        }

        private void SetHealth(float health)
        {
            _healthBarSlider.value = health / MaxHp;
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

        public override float CollisionDamage
        {
            get
            {
                // Only give dmg once per collision
                if (_canDealRamDamage)
                {
                    _canDealRamDamage = false;
                    return RamDamage;
                }

                return 0;
            }
        }

        private void HandleAttacks()
        {
            if (_knockedBackDestination != null)
            {
                return;
            }

            if (_isTelegraphingOrAttacking) return;

            var time = (int)(Time.time * 1000);

            if (time - _lastRamAttackTime > timeBetweenRamAttacks)
            {
                TelegraphAttack(true);
                return;
            }

            if (time - _lastAttackTime > timeBetweenAttacks)
            {
                TelegraphAttack(false);
            }
        }

        private void TelegraphAttack(bool ram)
        {
            _animator.SetTrigger(ram ? TelegraphRamAttackAnimTrig : TelegraphAttackAnimTrig);
            _isTelegraphingOrAttacking = true;
        }

        private void StartRamAttack()
        {
            _nav.speed = RamSpeed;
            _rollSpeed = RamSpeed;
            _nav.stoppingDistance = RamStopDistance;
            _isRamFollowing = true;
            _ramFollowTime = (int)(Time.time * 1000);
        }

        private void StopRamAttack()
        {
            _ramDestination = null;
            _nav.speed = MaxNavSpeed;
            _rollSpeed = MaxNavSpeed;
            _knockedBackDestination = null;
            _nav.stoppingDistance = RegularStopDistance;
            _lastRamAttackTime = (int)(Time.time * 1000);
            _isTelegraphingOrAttacking = false;
            _canDealRamDamage = true;
        }

        private void RamKnockback()
        {
            if (_knockedBackDestination != null)
            {
                return;
            }

            _ramDestination = null;

            _nav.speed = KnockbackSpeed;
            _rollSpeed = KnockbackSpeed;

            var direction = _player.transform.position - enemyBody.transform.position;
            direction.Normalize();
            direction.x *= -1;
            direction.z *= -1;
            direction.y = 0;

            _knockedBackDestination = enemyBody.transform.position + direction * KnockbackDistance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Player))
            {
                RamKnockback();
            }
        }

        public void Shoot()
        {
            // check if it's time to shoot
            // Instantiate projectileCountPerAttack projectiles in a circle around the enemy
            var angle = 360 / projectileCountPerAttack;
            for (var i = 0; i < projectileCountPerAttack; i++)
            {
                var projectileRotation = Quaternion.Euler(0, angle * i, 0);
                var projectilePosition = transform.position + projectileRotation * Vector3.forward * 2;
                Instantiate(projectile, projectilePosition, projectileRotation);
            }

            _lastAttackTime = (int)(Time.time * 1000);
            _isTelegraphingOrAttacking = false;
        }
    }
}