using System.Collections;
using UnityEngine;
using TMPro;
using System;
using DamageNumbersPro;
using DG.Tweening;
namespace BlackHole
{
    public class Scoring : MonoBehaviour
    {
        public static Scoring Instance;

        [SerializeField] private Bank _bankSO;
        [SerializeField] private GameParams _gameParamsSO;
        [SerializeField] private PlayerStats _playerStatsSO;

        [SerializeField] private TMP_Text _scoreTextGameplay, _scoreTextGameOver;
        [SerializeField] private DamageNumberGUI _multiplierDamageNumberPrefab;
        [SerializeField] private Transform _multiplierSpawnPoint;

        public static event Action<int> OnScored;

        private int _score = 0;
        private int _finalScore = 0;
        private string _finalAccuracy = "";
        public int ComboCount { get; set; } = 0;
        private bool _newHighscore = false;
        private TMP_Text _activeText;
        private int _cashGained = 0;
        private int _finalCashGained = 0;
        public static int LoopCount { get; private set; } = 1; 

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            LoopCount = 1;
        }

        public void IncrementScore(int amount)
        {
            _score += (int)(Mathf.Max(1, ComboCount) * amount * (Ship.Instance.IsOverdriveActive ? _gameParamsSO.OverdriveScoreMultiplier : 1));
            _scoreTextGameplay.text = "Score: " + _score;
            if (LoopCount > 1)
            {
                _scoreTextGameplay.text += "\nLoop: " + LoopCount;
            }
        }

        public void IncrementCashGained(int cash)
        {
            _cashGained += cash;
        }

        public void IncrementLoopCount()
        {
            LoopCount++;
            _scoreTextGameplay.text = "Score: " + _score + "\nLoop: " + LoopCount;
        }

        public void CalculateFinalScoreAndCash(bool victorious)
        {

            float finalMultiplier = victorious ? _gameParamsSO.VictoryMultiplier : _gameParamsSO.GameOverMultiplier;

            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Easy: finalMultiplier *= _gameParamsSO.EasyScoreMultiplier; break;
                case SettingsManager.DifficultySetting.Normal: finalMultiplier *= _gameParamsSO.NormalScoreMultiplier; break;
                case SettingsManager.DifficultySetting.Hard: finalMultiplier *= _gameParamsSO.HardScoreMultiplier; break;
                case SettingsManager.DifficultySetting.Expert: finalMultiplier *= _gameParamsSO.ExpertScoreMultiplier; break;
            }

            _finalScore = (int)(finalMultiplier * _score);
            _finalCashGained = (int)(finalMultiplier * _cashGained);

            _bankSO.CashTransfer(_finalCashGained);

            if (_finalScore > _playerStatsSO.GetHighscore())
            {
                _playerStatsSO.SetHighscore(_finalScore);
                _newHighscore = true;
            }

            OnScored?.Invoke(_finalScore);
        }

        public void DisplayFinalScoreAndCash(bool victorious)
        {
            CalculateFinalScoreAndCash(victorious);
            StartCoroutine(SpawnScoreMultipliers());

            _activeText = PostGameScreen.Instance.ScoreText;
            _finalAccuracy = "\nSolved: " + Math.Round(100f * QuestionAsteroid.Instance.SolveAccuracy) + "%";

            // Tween score and cash to final values
            string highscoretext = _newHighscore ? "New Highscore: " : "Score: "; 
            DOTween.To(() => _cashGained, x => _cashGained = x, _finalCashGained, 4f).SetUpdate(true);
            DOTween.To(() => _score, x => _score = x, _finalScore, 4f).OnUpdate( () => {
                _activeText.text = highscoretext + _score + _finalAccuracy + "\nLoot: $" + _cashGained;
            }).SetUpdate(true);     
        }

        private IEnumerator SpawnScoreMultipliers()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            float gameResult = GameManager.GameWasWon ? _gameParamsSO.VictoryMultiplier : _gameParamsSO.GameOverMultiplier;
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Powerup);
            DamageNumber d = _multiplierDamageNumberPrefab.Spawn(Vector3.zero, gameResult);
            d.SetAnchoredPosition(_multiplierSpawnPoint, _multiplierSpawnPoint, Vector2.zero);
            d.leftText = GameManager.GameWasWon ? "Victory x" : "Defeat x";
            yield return new WaitForSecondsRealtime(1.5f);
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Powerup);
            DamageNumber d2 = _multiplierDamageNumberPrefab.Spawn(Vector3.zero, SettingsManager.DifficultyScoreMultiplier);
            d2.SetAnchoredPosition(_multiplierSpawnPoint, _multiplierSpawnPoint, Vector2.zero);
            d2.leftText = SettingsManager.Instance.SelectedDifficulty.ToString() + " x";
        }
    }
}