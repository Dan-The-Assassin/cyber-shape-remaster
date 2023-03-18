using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private GameObject healthPrefab;
        [SerializeField] private Player player;
        
        private List<HealthChunk> _health = new();

        private void Awake()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        private void Start()
        {
            DrawHealth();
        }

        private void Update()
        {
            for (var i = 0; i < player.MaxHealth; i++)
            {
                _health[i].SetHealth(i + 1 <= player.CurrentHealth);
            }
        }

        public void DrawHealth()
        {
            ClearHealth();
            for (var i = 0; i < player.MaxHealth; i++)
            {
                CreateHealthChunk();
            }
        }

        private void CreateHealthChunk()
        {
            var newHealth = Instantiate(healthPrefab, transform, true);

            var hpComponent = newHealth.GetComponent<HealthChunk>();
            hpComponent.SetHealth(true);
            _health.Add(hpComponent);
        }

        private void ClearHealth()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
            _health = new List<HealthChunk>();
        }
    }
}
