using System;
using Evolution;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Enemy
{
    public class EnemyController : AbstractEnemyController
    {
        public UnityEvent OnDeath { get; } = new();

        [SerializeField] private float firingSpeed;
        [SerializeField] private GameObject projectile; 
        [SerializeField] private Transform enemyFirePoint;
        [SerializeField] private int minDistance = 10;
        
        public EnemyStageData StageData => _evolvable.Stage.EnemyData;

        private GameObject _player;
        private Rigidbody _rigidbody;
        private Canvas _healthBar;
        private Slider _healthBarSlider;
        private float _maxHealth;
        public float health;
        private Evolvable _evolvable;
        private int _distance;
        private float _lastTimeShot;
        private Camera _camera;
         protected override void Awake()
        {
            base.Awake();
            
            _player = GameObject.FindWithTag("Player");
            _healthBar = GetComponentInChildren<Canvas>();
            _healthBarSlider = _healthBar.GetComponentInChildren<Slider>();
            _evolvable = GetComponentInChildren<Evolvable>();
            _rigidbody = enemyBody.GetComponent<Rigidbody>();
            _camera = Camera.main;
        }

        private void Start()
        {
            health = StageData.Health;
            SetMaxHealth(health);
            SetHealth(health);

            _rollSpeed = StageData.RollSpeed;
            _nav.speed = StageData.NavSpeed;
        }

        private void Update()
        {
            _healthBar.transform.rotation = _camera.transform.rotation;
            var distance = Vector3.Distance(_player.transform.position, transform.position);
            transform.LookAt(_player.transform);
            
            if (_player.transform.hasChanged)
            {
                var destination = _player.transform.position;
                var rotX = destination[0] - enemyBody.transform.position.x;
                var rotZ = destination[2] - enemyBody.transform.position.z;
                _rigidbody.AddTorque(new Vector3(rotX / 2, 0, rotZ / 2) * _rollSpeed);
                _nav.SetDestination(destination);
            }

            if (distance < minDistance)
            {
                Shoot();
            }
        }

        private void SetMaxHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
        }

        private void SetHealth(float health)
        {
            _healthBarSlider.value = health / _maxHealth;
        }

        public override void TakeDamage(float damage)
        {
            health -= damage;
            SetHealth(health);
            if (health <= 0)
            {
                _player.GetComponent<Player>().AddScore(1);
                OnDeath.Invoke();
                Destroy(gameObject);
            }
        }

        public override float CollisionDamage => _evolvable.Stage.EnemyData.CollisionDamage;

        private void Shoot()
        {
            if (_lastTimeShot + firingSpeed < Time.time)
            {
                _lastTimeShot = Time.time;
                Instantiate(projectile, enemyFirePoint.position, enemyFirePoint.rotation);
            }
        }
    }
}