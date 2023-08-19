using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    public class AchievementNotification : MonoBehaviour
    {

        private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();

        public void StartAchievementsDisplay()
        {
            StartCoroutine(CoroutineCoordinator());
        }

        public void EnqeueueAchievement(AchievementsManager.Achievement achievement)
        {
            coroutineQueue.Enqueue(DisplayAchievementNotification(achievement));
        }

        IEnumerator CoroutineCoordinator()
        {
            while (true)
            {
                while (coroutineQueue.Count > 0)
                    yield return StartCoroutine(coroutineQueue.Dequeue());
                yield return null;
            }
        }

        private RectTransform _achievementPanel;
        [SerializeField] private TMP_Text _achievementText;
        private float _originalPanelPosY;

        private void Awake()
        {
            _achievementPanel = GetComponent<RectTransform>();
            _originalPanelPosY = _achievementPanel.anchoredPosition.y;
        }

        public IEnumerator DisplayAchievementNotification(AchievementsManager.Achievement achievement)
        {
            float newPanelPosY = _originalPanelPosY + (SceneManager.GetActiveScene().name == "MainMenu" ? 330f : 300f); // refactor later

            _achievementText.text = "Achievement:\n" + achievement.Name;

            // Display
            _achievementPanel.DOAnchorPosY(newPanelPosY, 0.4f).SetEase(Ease.OutCubic).SetUpdate(true);
            SoundManager.Instance.PlaySFX(SoundManager.SFX.ButtonPress);
            yield return new WaitForSecondsRealtime(3.3f);

            // Fade
            _achievementPanel.DOAnchorPosY(1.4f * newPanelPosY, 0.4f).SetEase(Ease.OutCubic).SetUpdate(true);
            _achievementPanel.GetComponent<Image>().DOFade(0f, 0.4f).SetUpdate(true);
            _achievementText.DOFade(0f, 0.4f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.5f);

            // Reset
            _achievementPanel.DOAnchorPosY(_originalPanelPosY, 0f).SetUpdate(true);
            _achievementPanel.GetComponent<Image>().DOFade(1f, 0f).SetUpdate(true);
            _achievementText.DOFade(1f, 0f).SetUpdate(true);
        }
    }
}