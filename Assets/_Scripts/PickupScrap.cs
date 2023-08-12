using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class PickupScrap : Pickup
    {
        protected override void ApplyItem()
        {
            base.ApplyItem();
            Ship.Instance.CurrentHealth += _gameParams.ScrapValue;
            Ship.Instance.CurrentHealth = Mathf.Min(_gameParams.MaxHealth, Ship.Instance.CurrentHealth);
            CanvasManager.Instance.UpdateHealth(Ship.Instance.CurrentHealth);
        }
    }
}
