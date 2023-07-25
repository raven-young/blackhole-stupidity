using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private int _health = 3;
    [SerializeField] private GameObject _explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        // force in direction of black hole
        float force = BlackHole.Instance.CurrentForce;
        transform.GetComponent<Rigidbody2D>().AddForce(-transform.position*force*Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void takeDamage(int damage)
    {
        _health--;

        if (_health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 6: // player
                break;
            case 10: // black hole
                Die();
                break;
        } 
    }

    private void Die()
    {
        GameObject effect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.3f);
        Destroy(gameObject);
    }
}
