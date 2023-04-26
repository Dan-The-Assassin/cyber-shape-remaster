using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public const float ProjectileHeight = 0.1f;
        public Transform gapTransform;
        public bool QueuedForShooting { get; set; }
        public Vector3 BulletPosition => _bullet.transform.position;

        public BulletType BulletType
        {
            set => _bullet.Type = value;
        }
        private Bullet _bullet;
        private MeshRenderer _meshRenderer;
        private ParticleSystem _particleSys;
        private ParticleSystemRenderer _particleSysRenderer;

        private void Awake()
        {
            _bullet = GetComponentInChildren<Bullet>();
            _meshRenderer = _bullet.GetComponent<MeshRenderer>();
            _particleSys = _bullet.GetComponent<ParticleSystem>();
            _particleSysRenderer = _bullet.GetComponent<ParticleSystemRenderer>();
        }

        public void ChangeBulletColor(Color baseColor, Color emissionColor)
        {
            _meshRenderer.material.SetColor("_BaseColor", baseColor);
            _meshRenderer.material.SetColor("_EmissionColor", emissionColor);
            _particleSys.Play();
            _particleSysRenderer.material.SetColor("_BaseColor", baseColor * 15f);
            _particleSysRenderer.material.SetColor("_EmissionColor", emissionColor * 15f);
        }

        private bool ShouldOrbit => _bullet.ShouldOrbit;

        public bool CanShoot()
        {
            return _bullet.ReadyForShooting && ShouldOrbit && !QueuedForShooting;
        }

        public void Shoot(Vector3 target, float damageMultiplier)
        {
            _bullet.Shoot(target, damageMultiplier);
            QueuedForShooting = false;
        }

        public void OrbitAround(Vector3 point, float orbitSpeed)
        {
            gapTransform.RotateAround(point, Vector3.up, 20 * Time.deltaTime * orbitSpeed);
            if (ShouldOrbit)
            {
                _bullet.transform.RotateAround(point, Vector3.up, 20 * Time.deltaTime * orbitSpeed);
            }
        }

        public void UpdateProjectilePosition(Vector3 deltaPos)
        {
            gapTransform.position += deltaPos;
            if (ShouldOrbit)
            {
                _bullet.transform.position += deltaPos;
            }
        }

        public void Init(Vector3 position, Quaternion rotation)
        {
            _bullet.transform.localPosition = position;
            _bullet.transform.localRotation = rotation;
            gapTransform.localPosition = position;
            gapTransform.localRotation = rotation;
        }
    }
}