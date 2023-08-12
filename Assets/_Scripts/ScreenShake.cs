using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackHole;

public class ScreenShake : MonoBehaviour
{

    // Transform of the GameObject you want to shake
    private Transform _transform;

    // Desired duration of the shake effect
    private static float _shakeDuration = 0f;

    // A measure of magnitude for the shake. Tweak based on your preference
    private static float _shakeMagnitude = 0.7f;

    // A measure of how quickly the shake effect should evaporate
    private static float _dampingSpeed = 1.0f;

    // The initial position of the GameObject
    Vector3 initialPosition;

    void Awake()
    {
        if (_transform == null)
        {
            _transform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        initialPosition = _transform.localPosition;

        GameManager.OnGameOver += StopShake;
        GameManager.OnVictory += StopShake;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= StopShake;
        GameManager.OnVictory -= StopShake;
    }

    void Update()
    {
        if (_shakeDuration > 0)
        {
            _transform.localPosition = initialPosition + Random.insideUnitSphere * _shakeMagnitude;

            _shakeDuration -= Time.deltaTime * _dampingSpeed;
        }
        else
        {
            _shakeDuration = 0f;
            _transform.localPosition = initialPosition;
        }
    }

    public static void TriggerShake(float shakeDuration)
    {
        _shakeDuration = shakeDuration;
    }

    void StopShake()
    {
        _shakeDuration = 0f;
        _transform.localPosition = initialPosition;
    }
}
