using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class PickupCrystal : Pickup
    {

        [SerializeField] private Bank _bankSO;

        protected override void ApplyItem()
        {
            base.ApplyItem();
            _bankSO.CashTransfer(_gameParams.CrystalValue);
        }
    }
}
