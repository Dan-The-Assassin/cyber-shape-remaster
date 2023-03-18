using System;
using Constants;
using Enemy;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EnemyIndicator : MonoBehaviour
    {
        public GameObject TargetEnemy { get; set; }

        private GameObject _player;
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Image _image;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _player = GameObject.FindWithTag(Tags.Player);
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            var enemyController = TargetEnemy.GetComponent<EnemyController>();
            var enemyBody = TargetEnemy.GetComponentInChildren<EnemyBody>();
            enemyBody.OnCameraEnter.AddListener(() => _image.enabled = false);
            enemyBody.OnCameraExit.AddListener(() => _image.enabled = true);

            enemyController.OnDeath.AddListener(() =>
            {
                Destroy(gameObject);
            });
        }

        private void Update()
        {
            var direction = TargetEnemy.transform.position - _player.transform.position;

            direction.y = direction.z;
            direction.z = 0;
            direction.Normalize();

            var angle = Vector3.SignedAngle(Vector3.up, direction, Vector3.forward);

            _rectTransform.eulerAngles = Vector3.forward * angle;

            angle = Vector3.Angle(Vector3.up, direction);

            var h = _canvas.GetComponent<RectTransform>().rect.height;
            var w = _canvas.GetComponent<RectTransform>().rect.width;

            var diagonalAngle = Mathf.Atan(w / h) * Mathf.Rad2Deg;
            var angleRadians = Mathf.Deg2Rad * angle;
            float dist;

            if (angle >= 0 && angle < diagonalAngle)
            {
                dist = h / 2 / Mathf.Cos(angleRadians);
            }
            else if (angle >= diagonalAngle && angle < 180 - diagonalAngle)
            {
                dist = w / 2 / Mathf.Cos(Mathf.PI / 2 - angleRadians);
            }
            else
            {
                dist = h / 2 / Mathf.Cos(Mathf.PI - angleRadians);
            }

            var offset = 50;
            _rectTransform.anchoredPosition = (dist - offset) * direction;
        }
    }
}