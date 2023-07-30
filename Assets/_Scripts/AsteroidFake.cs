using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class AsteroidFake : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private Rigidbody2D _rb;

    private void Start()
    {
        // random flip
        gameObject.GetComponent<SpriteRenderer>().flipY = UnityEngine.Random.Range(0f, 1f) < 0.5f;

        var spin = UnityEngine.Random.Range(0f, 1f) < 0.5f ? 1 : -1;
        // random torque
        _rb.AddTorque(UnityEngine.Random.Range(5f,20f) * spin, ForceMode2D.Impulse);
    }
    private void Update()
    {
        if (transform.position.y < -60 || transform.position.y > 60 || transform.position.x > 60 || transform.position.x < -60)
            Destroy(gameObject);
    }
}