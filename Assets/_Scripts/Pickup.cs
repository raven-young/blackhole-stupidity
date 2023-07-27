using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    [SerializeField] protected GameParams _gameParams;
    [SerializeField] private Rigidbody2D _rb;

    [SerializeField] private AudioClip _pickupClip;

    private void Update()
    {
        // Despawn if below x axis
        if (transform.position.y < -1)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        // force in direction of black hole
        float force = BlackHole.Instance.CurrentForce;
        _rb.AddForce(-transform.position * force * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) // ship
        {
            ApplyItem();
            CanvasManager.Instance.IncrementScore(_gameParams.CollectedItemScore);
            Destroy(gameObject);
        }
    }

    protected virtual void ApplyItem()
    {
        SoundManager.Instance.PlaySound(_pickupClip);
    }
}
