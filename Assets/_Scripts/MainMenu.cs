using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _startButton, _quitButton;
    [SerializeField] private SpriteRenderer _blackPanel;
    private void Start()
    {
        Time.timeScale = 1f;
        SoundManager.Instance.StartMainMenuMusic();
        _gameParams.ScreenBounds = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z));
        float _buttonY = _startButton.transform.position.y;
        _startButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        _quitButton.transform.DOMoveY(_buttonY + 0.7f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    // Wrapper for button
    public void StartGame(int selectedDifficulty)
    {
        StartCoroutine(StartGameRoutine(selectedDifficulty));
    }

    private IEnumerator StartGameRoutine(int selectedDifficulty)
    {
        _blackPanel.DOFade(1f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        SettingsManager.Instance.SelectedDifficulty = (SettingsManager.DifficultySetting)selectedDifficulty;
        Debug.Log("Starting game with difficulty: " + selectedDifficulty + " " + (SettingsManager.DifficultySetting)selectedDifficulty);

        DOTween.KillAll();
        SceneManager.LoadScene("CutsceneIntro");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
