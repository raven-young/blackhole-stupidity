using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    // Adapted from a Udemy course by Wilmer Lin
    public class MenuManager : MonoBehaviour
    {
        private static MenuManager _instance;
        public static MenuManager Instance { get => _instance; }

        // Don't change types below to subtyles, else GetNestedFieldValuesOfType won't work
        public Menu MainMenuPrefab;
        public Menu DifficultyMenuPrefab;
        //public Menu UpgradeMenuPrefab;
        public Menu PauseMenuPrefab;
        public Menu VictoryScreenPrefab;

        [SerializeField] private Transform _menuParent;

        private readonly Stack<Menu> _menuStack = new();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            } 
            else
            {
                _instance = this;
                InitializeMenus();
            }

            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void InitializeMenus()
        {
            if (_menuParent == null)
            {
                GameObject menuParentObject = new("Menus");
                _menuParent = menuParentObject.transform;
                DontDestroyOnLoad(_menuParent.gameObject);
            }

            List<object> menuPrefabs = new(this.GetNestedFieldValuesOfType<Menu>());

            foreach (Menu prefab in menuPrefabs)
            {
                if (prefab != null)
                {
                    Menu menuInstance = Instantiate(prefab, _menuParent);
                    if (prefab != MainMenuPrefab || SceneManager.GetActiveScene().name == "BlackHole") // when testing Black Hole scene in editor
                    {
                        menuInstance.gameObject.SetActive(false);
                    }
                    else
                    {
                        OpenMenu(menuInstance);
                    }
                }
            }  
        }

        public void OpenMenu(Menu menuInstance)
        {

            if (menuInstance.gameObject.activeSelf)
            {
                return;
            }


            if (menuInstance == null)
            {
                Debug.LogWarning("MENUMANAGER OpenMenu ERROR: Invalid menu");
            }

            if (_menuStack.Count > 0)
            {
                foreach (Menu menu in _menuStack)
                {
                    menu.gameObject.SetActive(false);
                }
            }

            menuInstance.gameObject.SetActive(true);
            menuInstance.SetFirstSelected();
            _menuStack.Push(menuInstance);
        }

        public void CloseMenu()
        {
            if (_menuStack.Count == 0)
            {
                Debug.LogWarning("MENUMANAGER CloseMenu ERROR: No menus in stack");
                return;
            }

            Menu topMenu = _menuStack.Pop();
            topMenu.gameObject.SetActive(false);

            if (_menuStack.Count > 0)
            {
                Menu nextMenu = _menuStack.Peek();
                nextMenu.gameObject.SetActive(true);
                nextMenu.SetFirstSelected();
            }
        }
    }
}