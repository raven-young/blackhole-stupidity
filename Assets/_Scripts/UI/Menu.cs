using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    [RequireComponent(typeof(Canvas))]
    public class Menu : MonoBehaviour
    {
        [SerializeField] private MenuManager _menuManager;

        private void Start()
        {
            _menuManager = FindObjectOfType<MenuManager>();
        }

        public void OnPlayPressed()
        {
            Menu difficultySelection = transform.parent.Find("DifficultySelection(Clone)").GetComponent<Menu>();
            _menuManager.OpenMenu(difficultySelection);
        }

        public void OnDifficultySelected(int selectedDifficulty)
        {
            SettingsManager.Instance.SelectedDifficulty = (SettingsManager.DifficultySetting)selectedDifficulty;
        }

        public void OnBackPressed()
        {
            _menuManager.CloseMenu();
        }
    }
}