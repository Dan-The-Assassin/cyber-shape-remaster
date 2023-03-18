using UnityEngine;

namespace UI
{
    public class RhythmLine : MonoBehaviour
    {
        public float Time { get; set; }
        public float Distance { get; set; } //distance it has to move to reach next beat, sign is opposite of index's sign
        public float StartPos { get; set; }
        public float Speed() => Distance / Time;

        private RectTransform _rectTransform;
        private RhythmTimer _timer;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            transform.position += new Vector3(Distance * UnityEngine.Time.deltaTime / Time, 0, 0);

            CheckPosition();
        }

        private void CheckPosition()
        {
            if ((Distance < 0 && _rectTransform.localPosition.x <= 0.0f)
                || (Distance > 0 && _rectTransform.localPosition.x >= 0.0f))
            {
                _rectTransform.localPosition = new Vector3(StartPos + _rectTransform.localPosition.x, 0, 0);
            }
        }
    }
}
