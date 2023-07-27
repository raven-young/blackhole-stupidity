using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScrap : Pickup
{
    [SerializeField] private int _scrapBonus;
    protected override void ApplyItem()
    {
        base.ApplyItem();
        Ship.Instance.CurrentHealth += _scrapBonus;
    }
}
