using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFuel : Pickup
{
    [SerializeField] private int _fuelBonus;
    protected override void ApplyItem()
    {
        Ship.Instance.CurrentFuel += _fuelBonus;
    }
}
