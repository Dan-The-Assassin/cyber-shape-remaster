using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Enemy;
using Projectiles;
using UnityEngine;
using UnityEngine.UI;
using Animations = Constants.Animations;
using UnityEngine.SceneManagement;
using Scene = Constants.Scene;

namespace UI
{
    public class HudManager : MonoBehaviour
    {
        [field: SerializeField] public Player Player { get; private set; }
        [field: SerializeField] public GameObject BulletUiPrefab { get; private set; }
        [field: SerializeField] public GameObject EnemyIndicatorPrefab { get; private set; }

        public PlayerHealth Hp { get; private set; }
        public ScoreUI ScoreUI { get; private set; }
        public WavesUI WavesUI { get; private set; }

        private List<BulletType> Bullets { get; set; } = new();
        
        private const string BulletUIContainerName = "Bullets";
        private const string BulletIconName = "Bullet";
        private const int BulletUiSize = 50;
        private const int BulletUiMargin = 30;
        private const int BulletSelectedUiExtraOffset = 20;

        private GameObject _bulletUiContainer;
        private readonly List<GameObject> _bulletUiList = new();

        private int _selectedBulletIndex = -1;

        private void Awake()
        {
            Hp = GetComponentInChildren<PlayerHealth>();
			ScoreUI = GetComponentInChildren<ScoreUI>();
            WavesUI = GetComponentInChildren<WavesUI>();
            _bulletUiContainer = GameObject.Find(BulletUIContainerName);
        }

        private void Update()
        {
            // TODO: It doesn't make sense to call these every frame. We should only call them in resposponse to some
            // event, such as when the player gets a new class of projectiles or they change the active ones.
            AddBullets(Player.UnlockedBulletTypes);
            StartCoroutine(SelectBullet(Player.CurrentBullet));
        }

        public void FollowEnemy(GameObject enemy)
        {
            var enemyIndicator = Instantiate(EnemyIndicatorPrefab, transform);
            enemyIndicator.GetComponent<EnemyIndicator>().TargetEnemy = enemy;
        }

        private void AddBullets(List<BulletType> projectiles)
        {
            foreach (var projectile in projectiles.Where(projectile => !Bullets.Contains(projectile)))
            {
                AddBullet(projectile);
            }
        }

        private void AddBullet(BulletType projectile)
        {
            var bulletUi = Instantiate(BulletUiPrefab, _bulletUiContainer.transform);

            bulletUi.transform.Find(BulletIconName).GetComponent<Image>().sprite = projectile.Sprite;
            var leftOffset = BulletUiSize * Bullets.Count + BulletUiMargin * Bullets.Count;
            bulletUi.GetComponent<RectTransform>().anchoredPosition = new Vector2(leftOffset, 0);
            Bullets.Add(projectile);
            _bulletUiList.Add(bulletUi);
        }

        private IEnumerator SelectBullet(BulletType bulletType)
        {
            // Get the index of the bullet in the list
            var index = Bullets.IndexOf(bulletType);

            if (index == -1) yield break;
            if (_selectedBulletIndex == index) yield break;

            yield return new WaitForEndOfFrame();

            if (_selectedBulletIndex != -1)
            {
                var currentBulletUi = _bulletUiList[_selectedBulletIndex];
                var currentBulletAnimator = currentBulletUi.GetComponent<Animator>();
                currentBulletAnimator.SetBool(Animations.BulletsUI.Triggers.SelectBullet, false);
            }

            var bulletUi = _bulletUiList[index];
            var animator = bulletUi.GetComponent<Animator>();
            animator.SetBool(Animations.BulletsUI.Triggers.SelectBullet, true);
            _selectedBulletIndex = index;
            UpdateBulletPositions(index);
        }

        private void UpdateBulletPositions(int selectedBulletIndex)
        {
            for (var i = 0; i < Bullets.Count; i++)
            {
                var bulletUi = _bulletUiList[i];
                var bulletUiRectTransform = bulletUi.GetComponent<RectTransform>();
                var leftOffset = BulletUiSize * i + BulletUiMargin * i +
                                 BulletSelectedUiExtraOffset * (Math.Max(i - selectedBulletIndex + 1, 0));
                bulletUiRectTransform.anchoredPosition = new Vector2(leftOffset, 0);
            }
        }
        
        public void GoToMenu()
        {
            SceneManager.LoadScene((int)Scene.MainMenuScene);
        }
    }
}
