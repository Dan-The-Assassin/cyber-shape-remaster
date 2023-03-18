using UnityEngine;

namespace Projectiles
{
    public class BulletExplosion : MonoBehaviour
    {
        private void Awake()
        {
            var main = GetComponent<ParticleSystem>().main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            ExplosionPool.Instance.Release(gameObject);
        }
    }
}
