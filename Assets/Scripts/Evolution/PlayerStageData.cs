using System;
using UnityEngine;

namespace Evolution
{
    [Serializable]
    public class PlayerStageData
    {
        [field: SerializeField] public float Health { get; private set; }
    }
}