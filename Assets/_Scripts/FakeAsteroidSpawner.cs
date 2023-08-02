using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Randomly spawn fake asteroids for menu screens etc.
public class FakeAsteroidSpawner : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private GameObject _fakeAsteroidPrefab;

    private float _spawnTimer = 0;
    private float _spawnPeriod = 2.5f;
    private float _screenBoundsX;
    private float _screenBoundsY;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Spawning fake asteroids");
        // Wait until main menu camera has fetched screenbounds
        StartCoroutine(GetScreenBounds());
    }

    IEnumerator GetScreenBounds()
    {
        yield return new WaitForEndOfFrame();
        _screenBoundsX = _gameParams.ScreenBounds.x;
        _screenBoundsY = _gameParams.ScreenBounds.y;
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer > _spawnPeriod && Random.Range(0f, 1f) > 0.8f)
        {
            _spawnTimer = 0f;
            float spawnY = Random.Range(0f, 1f) > 0.5f ? _screenBoundsY : -_screenBoundsY;
            Vector2 spawnPos = new(1.1f*Random.Range(-_screenBoundsX, _screenBoundsX), 1.1f*spawnY);
            GameObject asteroid = Instantiate(_fakeAsteroidPrefab, spawnPos, Quaternion.identity);
            float randomScale = Random.Range(0.5f, 1f);
            asteroid.transform.localScale = new(randomScale, randomScale, randomScale);
            float randomAngle = Random.Range(0, 70f);
            Vector2 _verticalDirection = spawnPos.y < 0 ? Vector2.down : Vector2.up;
            Vector2 direction = _verticalDirection.Rotate(spawnPos.x > 0 ? randomAngle : -randomAngle);
            float randomImpulse = Random.Range(15f, 30f);
            asteroid.GetComponent<Rigidbody2D>().AddForce(-randomImpulse * _gameParams.RandomAsteroidImpulse * direction, ForceMode2D.Impulse);
        }
    }
}
