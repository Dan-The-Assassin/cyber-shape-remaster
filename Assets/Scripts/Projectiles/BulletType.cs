using UnityEngine;

namespace Projectiles
{
    [CreateAssetMenu(fileName = "BulletType", menuName = "Scriptable Objects/Bullet Type")]
    public class BulletType : ScriptableObject
    {
        [field: SerializeField] public Sprite Sprite { get; private set; }

        [field: SerializeField] public Mesh Mesh { get; private set; }

        [field: Min(0)]
        [field: SerializeField]
        public float RespawnTime { get; private set; }

        [field: Min(0)]
        [field: SerializeField]
        public float Speed { get; private set; }

        [field: SerializeField] public DamageInfo DamageInfo { get; private set; }
    }
}