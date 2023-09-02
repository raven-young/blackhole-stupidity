using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackHole
{
    public class PauseMenu : Menu<PauseMenu>
    {
        protected void OnEnable()
        {
            // When the MenuManager initializes in Awake, CanvasManager might not exist yet
            if (CanvasManager.Instance != null)
            {
                CanvasManager.Instance.ToggleTouchControls(false);
            }
        }

        public void OnDisable()
        {
            if (CanvasManager.Instance != null)
            {
                CanvasManager.Instance.ToggleTouchControls(SettingsManager.IsMobileGame);
            }
        }

        public void OnRestartPressed()
        {
            GameManager.Restart();
            base.OnBackPressed();
        }

        public void OnQuitToMenuPressed()
        {
            base.OnBackPressed();
            GameManager.QuitToMenu();
        }

        public override void OnBackPressed()
        {
            GameManager.ResumeGame();
            base.OnBackPressed();
        }

    }
}