using System.Collections;
using UnityEngine;
using TMPro;
using System;
using DamageNumbersPro;

namespace BlackHole
{
    public class Scoring : MonoBehaviour
    {
        public static Scoring Instance;

        [SerializeField] private GameParams _gameParams;
        [SerializeField] private PlayerStats _playerStats;

        [SerializeField] private TMP_Text _scoreTextGameplay, _scoreTextVictory, _scoreTextGameOver;
        [SerializeField] private DamageNumberGUI _multiplierDamageNumberPrefab;
        [SerializeField] private Transform _multiplierSpawnPoint;

        public static event Action<int> OnScored;

        private int _score = 0;
        private int _finalScore = 0;
        private string _finalAccuracy = "";
        public int ComboCount = 0;
        private bool _newHighscore = false;
        private TMP_Text _activeText;
        private int _cashGained = 0;
        public static int LoopCount { get; private set; } = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void IncrementScore(int amount)
        {
            _score += (int)(Mathf.Max(1, ComboCount) * amount * (Ship.Instance.IsOverdriveActive ? _gameParams.OverdriveScoreMultiplier : 1));
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

        public static void IncrementLoopCount()
        {
            LoopCount++;
        }

        public void CalculateFinalScore(bool victorious)
        {
            _finalScore = victorious ? (int)(_gameParams.VictoryMultiplier * _score) : (int)(_gameParams.GameOverMultiplier * _score);

            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Easy: _finalScore = (int)(_finalScore * _gameParams.EasyScoreMultiplier); break;
                case SettingsManager.DifficultySetting.Normal: _finalScore = (int)(_finalScore * _gameParams.NormalScoreMultiplier); break;
                case SettingsManager.DifficultySetting.Hard: _finalScore = (int)(_finalScore * _gameParams.HardScoreMultiplier); break;
                case SettingsManager.DifficultySetting.Expert: _finalScore = (int)(_finalScore * _gameParams.ExpertScoreMultiplier); break;
            }

            if (_finalScore > _playerStats.GetHighscore())
            {
                _playerStats.SetHighscore(_finalScore);
                _newHighscore = true;
            }

            OnScored?.Invoke(_finalScore);
        }

        public void DisplayFinalScore(bool victorious)
        {
            CalculateFinalScore(victorious);

            _activeText = victorious ? _scoreTextVictory : _scoreTextGameOver;

            _finalAccuracy = "\nSolved: " + Math.Round(100f * QuestionAsteroid.Instance.GetAccuracy()) + "%";
            _activeText.text = _newHighscore ? "New Highscore: " + _score + _finalAccuracy : "Score: " + _score + _finalAccuracy;
            _activeText.text += "\nLoot: $" + _cashGained;

            StartCoroutine(UpdateScore());
            StartCoroutine(SpawnScoreMultipliers());
        }

        private IEnumerator UpdateScore()
        {
            // to do: handle special cases
            float duration = 4f; // display score changing to finalscore in this many seconds
            float updateFreq = 0.05f;
            int updateSteps = (int)(duration / updateFreq);

            // refactor later too tired to think
            if (_finalScore >= _score)
            {
                int delta = Mathf.Max(1, ((_finalScore - _score) / updateSteps));
                while (_score < _finalScore)
                {
                    _score += delta;
                    _activeText.text = _newHighscore ? "New Highscore: " + _score + _finalAccuracy : "Score: " + _score + _finalAccuracy;
                    _activeText.text += "\nLoot: $" + _cashGained;
                    yield return new WaitForSecondsRealtime(updateFreq);
                }
            }
            else
            {
                int delta = Mathf.Min(-1, ((_finalScore - _score) / updateSteps));
                while (_score > _finalScore)
                {
                    _score += delta;
                    _activeText.text = _newHighscore ? "New Highscore: " + _score + _finalAccuracy : "Score: " + _score + _finalAccuracy;
                    _activeText.text += "\nLoot: $" + _cashGained;
                    yield return new WaitForSecondsRealtime(updateFreq);
                }
            }
            _score = _finalScore;
            _activeText.text = _newHighscore ? "New Highscore: " + _score + _finalAccuracy : "Score: " + _score + _finalAccuracy;
            _activeText.text += "\nLoot: $" + _cashGained;
        }

        private IEnumerator SpawnScoreMultipliers()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            float gameResult = GameManager.Instance.GameWasWon ? _gameParams.VictoryMultiplier : _gameParams.GameOverMultiplier;
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Powerup);
            DamageNumber d = _multiplierDamageNumberPrefab.Spawn(Vector3.zero, gameResult);
            d.SetAnchoredPosition(_multiplierSpawnPoint, _multiplierSpawnPoint, Vector2.zero);
            d.leftText = GameManager.Instance.GameWasWon ? "Victory x" : "Defeat x";
            yield return new WaitForSecondsRealtime(1.5f);
            SoundManager.Instance.PlaySFX(SoundManager.SFX.Powerup);
            DamageNumber d2 = _multiplierDamageNumberPrefab.Spawn(Vector3.zero, SettingsManager.DifficultyScoreMultiplier);
            d2.SetAnchoredPosition(_multiplierSpawnPoint, _multiplierSpawnPoint, Vector2.zero);
            d2.leftText = SettingsManager.Instance.SelectedDifficulty.ToString() + " x";
        }
    }
}