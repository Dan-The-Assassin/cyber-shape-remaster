using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthChunk : MonoBehaviour
    {
        [SerializeField] private Sprite fullChunk;
        [SerializeField] private Sprite blankChunk;
    
        private Image _healthImage;

        private void Awake()
        {
            _healthImage = GetComponent<Image>();
        }

        public void SetHealth(bool full)
        {
            _healthImage.sprite = full ? fullChunk : blankChunk;
        }
    }
}
