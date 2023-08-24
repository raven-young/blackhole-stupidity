using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace BlackHole
{
    public class Asteroid : MonoBehaviour
    {

        [SerializeField] protected GameParams _gameParams;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private GameObject _explosionEffect;
        [SerializeField] protected FlashColor flashEffect;
        [SerializeField] private AudioClip _explosionClip;

        private int _currentHealth;
        private int _playerDamage;

        public static event Action<AvatarReactions.ExpressionEvents> OnAsteroidHit;

        private void Start()
        {
            _currentHealth = _gameParams.AsteroidHealth;

            // random flip
            gameObject.GetComponent<SpriteRenderer>().flipY = UnityEngine.Random.Range(0f, 1f) < 0.5f;

            var spin = UnityEngine.Random.Range(0f, 1f) < 0.5f ? 1 : -1;
            // random torque
            _rb.AddTorque(10f * spin, ForceMode2D.Impulse);

            _playerDamage = _gameParams.PlayerDamage;
            switch (SettingsManager.Instance.SelectedDifficulty)
            {
                case SettingsManager.DifficultySetting.Hard:
                    _playerDamage += _gameParams.HardPlayerDamageBonus;
                    break;
            }

        }
        private void Update()
        {
            // Despawn if below x axis
            if (transform.position.y < -1)
                Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            // force in direction of black hole
            float force = BlackHoleObject.Instance.CurrentForce;
            _rb.AddForce(-transform.position * force * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        public void TakeDamage(int damage)
        {

            flashEffect.Flash();
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                Scoring.Instance.IncrementScore(_gameParams.ShotAsteroidScore);
                Die(diedFromShooting: true);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            switch (collision.gameObject.layer)
            {
                case 6: // ship
                    ScreenShake.TriggerShake(_gameParams.ScreenShakeDuration);
                    collision.gameObject.GetComponent<Ship>().TakeDamage(_playerDamage);
                    OnAsteroidHit?.Invoke(AvatarReactions.ExpressionEvents.AsteroidHit);
                    Die(diedFromShooting: false);
                    break;
                case 10: // black hole
                         // increase BH
                    BlackHoleObject.Instance.GrowBlackHole(_gameParams.BlackHoleGrowthRate);
                    Die(diedFromShooting: false);
                    break;
                case 12: // shield
                    if (Shield.Instance.ShieldActive)
                    {
                        ScreenShake.TriggerShake(0.5f*_gameParams.ScreenShakeDuration);
                        Shield.Instance.DisableShield();
                        Die(diedFromShooting: false);
                    }
                    break;
                default:
                    break;
            }

        }

        protected virtual void Die(bool diedFromShooting)
        {
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            GameObject effect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.3f);
            Destroy(gameObject);
        }
    }
}