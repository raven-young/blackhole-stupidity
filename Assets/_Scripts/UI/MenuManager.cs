using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BlackHole
{
    // Adapted from a Udemy course by Wilmer Lin
    public class MenuManager : MonoBehaviour
    {
        private static MenuManager _instance;
        public static MenuManager Instance { get => _instance; }

        public static event Action EscapeActionPressed;

        // Don't change types below to subtypes, else GetNestedFieldValuesOfType won't work
        [SerializeField] private Menu MainMenuPrefab;
        [SerializeField] private Menu DifficultyMenuPrefab;
        [SerializeField] private Menu PauseMenuPrefab;
        [SerializeField] private Menu VictoryScreenPrefab;
        [SerializeField] private Menu UpgradeMenuPrefab;
        [SerializeField] private Menu ExtrasMenuPrefab;
        [SerializeField] private Menu AchievementsMenuPrefab;
        [SerializeField] private Menu SettingsMenuPrefab;
        [SerializeField] private Menu HighscoresMenuPrefab;
        [SerializeField] private Menu LeaderboardMenuPrefab;

        [SerializeField] private Transform _menuParent;

        private readonly Stack<Menu> _menuStack = new();

        private PlayerInputActions playerInputActions;

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
                playerInputActions = new PlayerInputActions();
            }

            DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
            playerInputActions.Enable();
            playerInputActions.Player.EscapeAction.performed += EscapeAction;
            playerInputActions.Player.Answer3.performed += EscapeAction;
        }

        private void OnDisable()
        {
            playerInputActions.Disable();
            playerInputActions.Player.EscapeAction.performed -= EscapeAction;
            playerInputActions.Player.Answer3.performed -= EscapeAction;
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

            if (menuInstance.gameObject.activeSelf && _menuStack.Count > 0 && _menuStack.Peek() == menuInstance)
            {
                Debug.LogWarning("MENUMANAGER OpenMenu ERROR: Attempted to open already open menu");
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

        public void CloseAllMenus()
        {
            while (_menuStack.Count > 0)
            {
                CloseMenu();
            }
        }

        private void EscapeAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                EscapeActionPressed?.Invoke();
            }
        }
    }
}