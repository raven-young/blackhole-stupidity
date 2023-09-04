using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    public class LevelManager : MonoBehaviour
    {

        private static LevelManager _instance;
        public static LevelManager Instance { get => _instance; }

        //private static GameObject _loadingScreen;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            DontDestroyOnLoad(this);
            //_loadingScreen = Resources.Load<GameObject>("_Prefabs/BlackPanel");
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void LoadLevel(int buildIndex)
        {
            ScreenFader.FadeToBlack(0);
            StartCoroutine(LoadSceneAsync(buildIndex));
        }

        public void LoadLevel(string sceneName)
        {
            ScreenFader.FadeToBlack(0);
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(int buildIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
            //GameObject loadingScreen = Instantiate(); // etc.
            //loadingScreen.SetActive(true);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress);
                Debug.Log(progress);
                yield return null;
            }

        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            //_loadingScreen.SetActive(true);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress);
                yield return null;
            }

        }
    }
}