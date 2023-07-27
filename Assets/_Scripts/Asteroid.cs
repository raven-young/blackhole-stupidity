using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private GameObject _explosionEffect; 
    [SerializeField] protected FlashColor flashEffect;
    [SerializeField] private AudioClip _explosionClip;

    private int _currentHealth;

    private void Start()
    {
        _currentHealth = _gameParams.AsteroidHealth;

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
        // force in direction of black hole
        float force = BlackHole.Instance.CurrentForce;
        _rb.AddForce(-transform.position*force*Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void TakeDamage(int damage)
    {

        flashEffect.Flash();
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 6: // ship
                collision.gameObject.GetComponent<Ship>().TakeDamage(_gameParams.PlayerDamage);
                Die();
                break;
            case 10: // black hole
                // increase BH
                BlackHole.Instance.ChangeSize(_gameParams.BlackHoleGrowthRate);
                Die();
                break;
            default:
                break;
        }
        
    }

    private void Die()
    {
        SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
        CanvasManager.Instance.IncrementScore(_gameParams.ShotAsteroidScore);
        GameObject effect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.3f);
        Destroy(gameObject);
    }
}
