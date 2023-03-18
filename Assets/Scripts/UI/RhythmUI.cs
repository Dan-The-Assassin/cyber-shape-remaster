using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RhythmUI : MonoBehaviour
    {
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private int numOfBeats = 5;

        private float _distance;
        private RhythmTimer _time;

        private void Awake()
        {
            _distance = GetComponent<RectTransform>().sizeDelta.x / 2;
            _time = GameObject.Find("Game Manager").GetComponent<RhythmTimer>();
        }

        private void Start()
        {
            InitialDraw();
        }

        private void ClearLines()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

        private void InitialDraw()
        {
            ClearLines();
            for (var i = 1; i <= numOfBeats; i++)
            {
                CreateLine(i);
            }
        }

        private void CreateLine(int i)
        {
            var newLine = Instantiate(linePrefab, transform, true);
            var newLine2 = Instantiate(linePrefab, transform, true);

            var lineComponent = newLine.GetComponent<RhythmLine>();
            var lineComponent2 = newLine2.GetComponent<RhythmLine>();

            lineComponent.Time = _time.Interval;
            lineComponent2.Time = _time.Interval;

            lineComponent.Distance = -_distance / numOfBeats;
            lineComponent.StartPos = _distance;
            lineComponent2.Distance = -lineComponent.Distance;
            lineComponent2.StartPos = - _distance;

            var offset = _time.Offset - (AudioSettings.dspTime - _time.DspTimeSong); //the offset of its position at creation is the offset of the track - how much time it's been since song started

            var distOffset = lineComponent.Speed() * _time.Offset;

            newLine.GetComponent<RectTransform>().localPosition = new Vector2(i * _distance / numOfBeats + distOffset, 0);
            newLine2.GetComponent<RectTransform>().localPosition = new Vector2(-i * _distance / numOfBeats - distOffset, 0);
        }
    }
}
