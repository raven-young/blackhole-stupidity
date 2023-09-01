using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    [RequireComponent(typeof(Canvas))]
    public abstract class Menu : MonoBehaviour
    {
        public virtual void OnBackPressed()
        {
            MenuManager.Instance.CloseMenu();
        }
    }
}