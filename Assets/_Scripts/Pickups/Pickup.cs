using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackHole
{
    public class Pickup : MonoBehaviour
    {

        [SerializeField, Range(0f,100f)] private float _collectedSpeed = 14f;
        [SerializeField] protected GameParams _gameParams;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private AudioClip _pickupClip;

        private bool _collected = false;

        private void Start()
        {
            // random flip
            gameObject.GetComponent<SpriteRenderer>().flipY = Random.Range(0f, 1f) < 0.5f;

            var spin = Random.Range(0f, 1f) < 0.5f ? 1 : -1;
            // random torque
            _rb.AddTorque(10f * spin, ForceMode2D.Impulse);
        }
        private void Update()
        {
            // Despawn if below x axis
            if (transform.position.y < -1)
                Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            if (_collected)
            {
                transform.position = Vector2.MoveTowards(transform.position, Ship.Instance.transform.position, _collectedSpeed * Time.deltaTime);
                return;
            }

            // force in direction of black hole
            float force = BlackHoleObject.Instance.CurrentForce;
            _rb.AddForce(-transform.position * force * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject.layer == 9) // ship's collector
            {
                _collected = true;
            }

            if (collision.gameObject.layer == 6) // ship
            {
                ApplyItem();
                Scoring.Instance.IncrementScore(_gameParams.CollectedItemScore);
                Destroy(gameObject);
            }
        }

        protected virtual void ApplyItem()
        {
            SoundManager.Instance.PlaySound(_pickupClip);
        }
    }
}