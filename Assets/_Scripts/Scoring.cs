using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class Scoring : MonoBehaviour
{
    public static Scoring Instance;

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private TMP_Text _scoreTextGameplay, _scoreTextVictory, _scoreTextGameOver;
    [SerializeField] private GameObject _multiplierDamageNumberPrefab;

    public static event Action<int> OnScored;

    private int _score = 0;
    private int _finalScore = 0;
    private string _finalAccuracy = "";
    public int ComboCount = 0;
    private bool _newHighscore = false;
    private TMP_Text _activeText;
    bool _startTweening = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void IncrementScore(int amount)
    {
        _score += Mathf.Max(1, ComboCount) * amount;
        _scoreTextGameplay.text = "Score: " + _score;
    }

    public void CalculateFinalScore(bool victorious)
    {
        _finalScore = victorious ? (int)(_gameParams.VictoryMultiplier * _score) : (int)(_gameParams.GameOverMultiplier * _score);

        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy: _finalScore = (int)(_finalScore*_gameParams.EasyScoreMultiplier); break;
            case SettingsManager.DifficultySetting.Normal: _finalScore = (int)(_finalScore*_gameParams.NormalScoreMultiplier); break;
            case SettingsManager.DifficultySetting.Hard: _finalScore = (int)(_finalScore*_gameParams.HardScoreMultiplier); break;
            case SettingsManager.DifficultySetting.Expert: _finalScore = (int)(_finalScore*_gameParams.ExpertScoreMultiplier); break;
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
        _activeText.text = _newHighscore ? "New Highscore: " + _score + _finalAccuracy :  "Score: " + _score + _finalAccuracy;

        StartCoroutine(UpdateScore());
    }

    private IEnumerator UpdateScore()
    {
        float duration = 4f; // display score changing to finalscore in this many seconds
        float updateFreq = 0.1f;
        int updateSteps = (int)(duration / updateFreq);
        int delta = (int)((_finalScore-_score)/ updateSteps);

        while (_score < _finalScore)
        {
            _score += delta;
            _activeText.text = _newHighscore ? "New Highscore: " + _score + _finalAccuracy : "Score: " + _score + _finalAccuracy;
            yield return new WaitForSecondsRealtime(updateFreq);
        }

        _score = _finalScore;
        _activeText.text = _newHighscore ? "New Highscore: " + _score + _finalAccuracy : "Score: " + _score + _finalAccuracy;
    }
}
