using System;
using Constants;
using UnityEngine;

namespace Projectiles
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float maxProjectileDistance;
        [SerializeField] private BulletType bulletType;

        public BulletType BulletType
        {
            set
            {
                bulletType = value;
                _meshCollider.sharedMesh = value.Mesh;
                _meshFilter.mesh = value.Mesh;
            }
        }

        private Vector3 _firingPoint;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private void Awake()
        {
            _meshCollider = GetComponent<MeshCollider>();
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            transform.Rotate(90, 0, 0);
            _firingPoint = transform.position;
            Physics.IgnoreLayerCollision(8, 9, true);

            // This makes the bullet initialize correctly if set through the inspector
            BulletType = bulletType;
        }

        private void Update()
        {
            MoveProjectile();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Player))
            {
                var triggeringPlayer = other.gameObject.GetComponent<Player>();
                triggeringPlayer.TakeDamage(bulletType.DamageInfo.Damage);
                Destroy(gameObject);
            }
            else if (other.CompareTag(Tags.Wall)) // Obiectele puse de Daria pot avea tag-ul Wall 
            {
                Destroy(gameObject);
            }
        }

        private void MoveProjectile()
        {
            if (Vector3.Distance(_firingPoint, transform.position) > maxProjectileDistance)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.Translate(Vector3.up * (bulletType.Speed * Time.deltaTime));
            }
        }
    }
}