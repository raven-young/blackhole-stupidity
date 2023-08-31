using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    // Adapted from a Udemy course by Wilmer Lin
    public class MenuManager : MonoBehaviour
    {
        public Menu MainMenuPrefab;
        public Menu DifficultyMenuPrefab;
        //public Menu UpgradeMenuPrefab;

        [SerializeField] private Transform _menuParent;

        private readonly Stack<Menu> _menuStack = new();

        private void Awake()
        {
            InitializeMenus();
        }
         
        private void InitializeMenus()
        {
            if (_menuParent == null)
            {
                GameObject menuParentObject = new("Menus");
                _menuParent = menuParentObject.transform;
            }

            List<object> menuPrefabs = new(this.GetNestedFieldValuesOfType<Menu>());

            foreach (Menu prefab in menuPrefabs)
            {
                Debug.Log(prefab + " " + _menuParent);
                if (prefab != null)
                {
                    Menu menuInstance = Instantiate(prefab, _menuParent);
                    if (prefab != MainMenuPrefab)
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
            }
        }
    }
}