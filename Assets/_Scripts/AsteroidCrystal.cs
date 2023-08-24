using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class AsteroidCrystal : Asteroid
    {

        [SerializeField] private GameObject _crystalPrefab;

        protected override void Die(bool diedFromShooting)
        {
            base.Die(diedFromShooting);

            if (!diedFromShooting) { return; }

            // Spawn crystals
            SpawnCrystals();

        }

        private void SpawnCrystals()
        {
            int spawnAmount = _gameParams.SpawnAmount + SettingsManager.ItemSpawnBonus;

            for (int i = 0; i < spawnAmount; i++)
            {
                float randomX = 0.1f * Random.Range(-1f, 1f) * _gameParams.ScreenBounds.x;
                Vector2 spawnPos = new(transform.position.x + randomX, transform.position.y);
                GameObject spawnedObject = Instantiate(_crystalPrefab, spawnPos, Quaternion.identity);

                float angle = Random.Range(-_gameParams.MaxSpawnAngle, _gameParams.MaxSpawnAngle);
                Vector2 direction = Vector2.up.Rotate(angle);

                spawnedObject.GetComponent<Rigidbody2D>().AddForce(-Random.Range(0.5f, 1f) * _gameParams.SpawnImpulse * direction,
                                                                   ForceMode2D.Impulse);
            }
        }
    }
}
