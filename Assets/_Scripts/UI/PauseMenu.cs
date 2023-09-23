using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackHole
{
    public class PauseMenu : Menu<PauseMenu>
    {
        protected override void OnEnable()
        {
            //base.OnEnable(); // Don't subscribe to EscapeActionPressed, else Button East will resume game AND Select Answer 3

            // When the MenuManager initializes in Awake, CanvasManager might not exist yet
            if (CanvasManager.Instance != null)
            {
                CanvasManager.Instance.ToggleTouchControls(false);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
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

        public void OnSettingsPressed()
        {
            SettingsMenu.Open();
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