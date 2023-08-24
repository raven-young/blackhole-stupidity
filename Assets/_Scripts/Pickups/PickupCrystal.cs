using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace BlackHole
{
    public class PickupCrystal : Pickup
    {

        [SerializeField] private Bank _bankSO;
        [SerializeField] private DamageNumber _cashNumberPosPrefab;

        protected override void ApplyItem()
        {
            base.ApplyItem();
            _bankSO.CashTransfer(_gameParams.CrystalValue * Scoring.LoopCount);
            _cashNumberPosPrefab.Spawn(transform.position, _gameParams.CrystalValue * Scoring.LoopCount);
            Scoring.Instance.IncrementCashGained(_gameParams.CrystalValue * Scoring.LoopCount);
        }
    }
}
