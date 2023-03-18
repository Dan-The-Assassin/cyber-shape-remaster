using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class EnemyBody : MonoBehaviour
    {
        public UnityEvent OnCameraEnter { get; } = new();
        public UnityEvent OnCameraExit { get; } = new();

        private void OnBecameVisible()
        {
            OnCameraEnter.Invoke();
        }

        private void OnBecameInvisible()
        {
            OnCameraExit.Invoke();
        }
    }
}