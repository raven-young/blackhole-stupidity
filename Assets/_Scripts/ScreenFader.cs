using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BlackHole
{
    public class ScreenFader : MonoBehaviour
    {
        public static void FadeFromBlack(float fadeDuration)
        {
            GameObject _blackPanelPrefab = Resources.Load<GameObject>("_Prefabs/BlackPanel");
            Image _blackPanel = Instantiate(_blackPanelPrefab).GetComponent<Image>();
            _blackPanel.DOFade(1, 0).SetUpdate(true);
            _blackPanel.DOFade(0, fadeDuration).SetUpdate(true);
        }

        public static void FadeToBlack(float fadeDuration)
        {
            GameObject _blackPanelPrefab = Resources.Load<GameObject>("_Prefabs/BlackPanel");
            Image _blackPanel = Instantiate(_blackPanelPrefab).GetComponent<Image>();
            _blackPanel.DOFade(0, 0).SetUpdate(true);
            _blackPanel.DOFade(1, fadeDuration).SetUpdate(true);
        }
    }
}