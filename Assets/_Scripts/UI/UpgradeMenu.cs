using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace BlackHole
{
    public class UpgradeMenu : Menu<UpgradeMenu>
    {
        public void OnStartPressed()
        {
            StartCoroutine(StartGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            ScreenFader.FadeToBlack(1f);
            yield return new WaitForSecondsRealtime(1f);
            DOTween.KillAll();
            SceneManager.LoadScene("CutsceneIntro");
            MenuManager.Instance.CloseAllMenus();
        }

        public void OnUpgradeSlotPressed(UpgradeSlot newslot)
        {
            UpgradeSlotManager.Instance.SwitchSelectedUpgradeSlot(newslot);
        }
    }
}