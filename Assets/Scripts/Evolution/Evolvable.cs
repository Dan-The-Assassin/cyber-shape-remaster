using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Evolution
{
    public class Evolvable : MonoBehaviour
    {
        [field: SerializeField] public Stage Stage { get; private set; }

        public UnityEvent OnEvolution { get; } = new();

        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private Animator _animator;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _animator = GetComponent<Animator>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            ApplyStage();
        }

        public void Evolve(int stages = 1)
        {
            if (Stage.NextStage is null)
            {
                return;
            }

            for (var i = 0; i < stages && Stage.NextStage is not null; i++)
            {
                Stage = Stage.NextStage;
            }

            if (_particleSystem)
            {
                _particleSystem.Play();
            }
            _animator.SetTrigger(Constants.Animations.Evolution.Triggers.Evolution);
            var animatorState = _animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex("Base Layer"));
            Invoke(nameof(ApplyStage), animatorState.length / 4);
            OnEvolution.Invoke();
        }

        private void ApplyStage()
        {     
            _meshFilter.mesh = Stage.Mesh;
            _meshCollider.sharedMesh = Stage.Mesh;
        }
    }
}
