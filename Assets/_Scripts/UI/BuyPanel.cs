using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BlackHole
{
    public class BuyPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _headerText;
        public TMP_Text HeaderText { get => _headerText; set => _headerText = value; }
    }
}