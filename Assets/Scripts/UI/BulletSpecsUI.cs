using Constants;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BulletSpecsUI : MonoBehaviour
    {
        private TextMeshProUGUI _bulletSpecsText;

        private void Awake()
        {
            _bulletSpecsText = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateBulletSpecsText(float damage, float speed)
        {
            _bulletSpecsText.text = $"Damage: {damage} \n" + $"Speed: {speed}\n";
        }
    }
}
