using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackHole
{

    public abstract class Menu<T>:Menu where T: Menu<T>
    {
        private static T _instance;
        public static T Instance { get => _instance; }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = (T)this;
            }
        }
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public static void Open()
        {
            if (MenuManager.Instance != null && _instance != null)
            {
                MenuManager.Instance.OpenMenu(_instance);
            }
        }
    }

    [RequireComponent(typeof(Canvas))]
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject _firstSelected;

        public virtual void SetFirstSelected()
        {
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_firstSelected, new BaseEventData(eventSystem));
        }

        public virtual void OnBackPressed()
        {
            MenuManager.Instance.CloseMenu();
        }
    }
}