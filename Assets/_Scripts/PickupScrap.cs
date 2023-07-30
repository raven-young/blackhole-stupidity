using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScrap : Pickup
{
    protected override void ApplyItem()
    {
        base.ApplyItem();
        Ship.Instance.CurrentHealth += _gameParams.ScrapValue;
        Ship.Instance.CurrentHealth = Mathf.Max(_gameParams.MaxHealth, Ship.Instance.CurrentHealth);
        CanvasManager.Instance.UpdateHealth(Ship.Instance.CurrentHealth);
    }
}
