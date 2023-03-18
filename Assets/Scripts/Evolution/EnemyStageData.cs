using System;
using UnityEngine;

namespace Evolution
{
    [Serializable]
    public class EnemyStageData
    {
        [field: SerializeField] public float Health { get; private set; }
        [field: SerializeField] public float RollSpeed { get; private set; }
        [field: SerializeField] public float NavSpeed { get; private set; }
        [field: SerializeField] public float CollisionDamage { get; private set; }
    };
}