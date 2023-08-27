using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace BlackHole
{
    public class PickupCrystal : Pickup
    {
        [SerializeField] private DamageNumber _cashNumberPosPrefab;

        protected override void ApplyItem()
        {
            base.ApplyItem();
            // here, we only increment the cash score in Scoring.cs. the cash itself is transferred to the bank at the end of the game
            _cashNumberPosPrefab.Spawn(transform.position, _gameParams.CrystalValue * Scoring.LoopCount);
            Scoring.Instance.IncrementCashGained(_gameParams.CrystalValue * Scoring.LoopCount);
        }
    }
}
