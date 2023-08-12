using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BlackHole
{
    public class GameOverTransition : MonoBehaviour
    {
        public static GameOverTransition Instance;

        [SerializeField] private AudioClip _gameOverClip;
        [SerializeField] private Transform _shipTransform;
        [SerializeField] private Camera _cam;
        [SerializeField] private Image _raccoonAvatar;
        private Material _raccoonMaterial;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start()
        {
            _raccoonMaterial = _raccoonAvatar.material;
        }

        public IEnumerator StartGameOverTransition()
        {
            Ship.Instance.CannotMove = true;
            Ship.Instance.IsInvincible = true;

            SoundManager.Instance.PlaySFX(SoundManager.SFX.AlertSFX);

            if (Ship.Instance.CurrentHealth <= 0)
            {
                // Explode ship
                StartCoroutine(Ship.Instance.Die());

                // Raccoon shake
                _raccoonMaterial.DOFloat(20, "_ShakeUvSpeed", 3f).SetDelay(0.5f);
                _raccoonMaterial.DOFloat(5, "_ShakeUvX", 3f).SetDelay(0.5f);
                _raccoonMaterial.DOFloat(5, "_ShakeUvY", 3f).SetDelay(0.5f);
                yield return new WaitForSecondsRealtime(4f);
            }
            else
            {
                // Ship falls into BH
                _shipTransform.DOMove(Vector3.zero, 1f).SetUpdate(true);

                // Raccoon is spaghettified
                _raccoonMaterial.DOFloat(2, "_RoundWaveStrength", 8f).SetDelay(0.5f);

                // Camera zooms into BH (camera movement broken)

                // Add a movement tween at the beginning
                //Sequence mySequence = DOTween.Sequence();

                //mySequence.Append(_cam.transform.DOMoveY(3.5f, 2f).SetUpdate(true));
                //mySequence.Append(_cam.transform.DOMoveY(3.4f, 9f).SetUpdate(true)); // camera teleports back after first tween so use this hacky workaround

                //_cam.DOOrthoSize(1.8f, 2f).SetUpdate(true);
                yield return new WaitForSecondsRealtime(3f);
            }

            GameManager.Instance.PauseGame();
            // Game over screen
            SoundManager.Instance.PlaySound(_gameOverClip);
            CanvasManager.Instance.RenderGameOverScreen(false);

            yield return new WaitForSecondsRealtime(7f);
            SoundManager.Instance.StartMainGameMusic(4f);
        }
    }
}