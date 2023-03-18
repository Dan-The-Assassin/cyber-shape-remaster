using System;
using UnityEngine;

namespace Projectiles
{
    [Serializable]
    public class DamageInfo
    {
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public BulletEffect Effect { get; private set; }
    }
}