using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackHole {
    public class ContinuePanel : MonoBehaviour
    {
        [SerializeField] private GameObject _yesButton;

        public bool IsBinaryChoiceActive = false;
        public bool ContinueGame = false;

        public void AskContinue()
        {
            IsBinaryChoiceActive = true;
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_yesButton, new BaseEventData(eventSystem));
            Time.timeScale = 0;
        }

        public void ExitContinue(bool doContinue)
        {
            ContinueGame = doContinue;
            IsBinaryChoiceActive = false;
            gameObject.SetActive(false);
            Time.timeScale = 1;
            if (doContinue) { Ship.Instance.ResetShipPosition(); }
        }
    }
}
