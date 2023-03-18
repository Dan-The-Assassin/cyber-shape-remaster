using UnityEngine;

namespace Evolution
{
    [CreateAssetMenu(fileName = "EvolutionStageDetails", menuName = "Scriptable Objects/Evolution Stage Details")]
    public class Stage : ScriptableObject
    {
        [field: SerializeField] public Mesh Mesh { get; private set; }
        [field: SerializeField] public Stage NextStage { get; private set; }
        [field: SerializeField] public EnemyStageData EnemyData { get; private set; }
        [field: SerializeField] public PlayerStageData PlayerData { get; private set; }
    }
}