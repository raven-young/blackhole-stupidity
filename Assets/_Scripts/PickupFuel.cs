using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFuel : Pickup
{
    protected override void ApplyItem()
    {
        base.ApplyItem();
        Ship.Instance.CurrentFuel += _gameParams.FuelValue;
    }
}
