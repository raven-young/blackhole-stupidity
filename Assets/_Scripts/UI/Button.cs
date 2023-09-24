using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BlackHole;

public class Button : SelectableUIComponent
{
    public static event Action<bool> BuyComplete;

    public void OnBuyComplete(bool doBuy)
    {
        BuyComplete?.Invoke(doBuy);
    }
}
