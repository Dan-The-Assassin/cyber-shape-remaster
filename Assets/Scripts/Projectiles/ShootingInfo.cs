using UnityEngine;

namespace Projectiles
{
    public class ShootingInfo
    {
        public Vector3 TargetPosition { get; }
        public Projectile Projectile { get; }
        public float DamageMultiplier { get; }

        public ShootingInfo(Vector3 targetPosition, Projectile projectile, float damageMultiplier)
        {
            TargetPosition = targetPosition;
            Projectile = projectile;
            DamageMultiplier = damageMultiplier;
        }
    }
}