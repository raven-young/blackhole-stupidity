using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class AsteroidFake : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Sprite _vachette; // easter egg

    private void Start()
    {
        // random flip
        gameObject.GetComponent<SpriteRenderer>().flipY = UnityEngine.Random.Range(0f, 1f) < 0.5f;

        var spin = UnityEngine.Random.Range(0f, 1f) < 0.5f ? 1 : -1;
        float torque = UnityEngine.Random.Range(5f, 20f);

        if (UnityEngine.Random.Range(0f, 1f) < 0.02f)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = _vachette;
            torque *= 0.3f;
        }
        // random torque
        _rb.AddTorque(torque * spin, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (transform.position.y < -60 || transform.position.y > 60 || transform.position.x > 60 || transform.position.x < -60)
            Destroy(gameObject);
    }
}