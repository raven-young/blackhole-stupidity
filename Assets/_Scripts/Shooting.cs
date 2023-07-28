using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Shooting : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private Bullet bulletPrefab;

    private Quaternion spreadAngle = Quaternion.identity;

    private PlayerInputActions playerInputActions;

    private static bool _canShoot = true;
    private bool _isAutomatic = true;
    private bool _isShooting = false;
    [SerializeField] private int _projectileNumber = 1;
    //[SerializeField] private float _projectileSpread = 30f;
    private float _shootCooldown;

    private static IObjectPool<Bullet> _pool;
    private static Shooting Instance;


    [SerializeField] private AudioClip _smallLaserClip;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Fire.performed += ShootAction;
        playerInputActions.Player.Fire.canceled += ShootAction;

        _pool = new ObjectPool<Bullet>(() =>
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.SetPool(_pool);
            return bullet;
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, false, 600, 1000
        );
    }

    private void OnDisable()
    {
        playerInputActions.Player.Fire.performed -= ShootAction;
        playerInputActions.Player.Fire.canceled -= ShootAction;
        playerInputActions.Player.Disable();
    }

    private void Update()
    {

        _shootCooldown += Time.deltaTime;
        if (_shootCooldown > _gameParams.FireRate)
        {
            _canShoot = true;
            _shootCooldown = 0f;
        }

    }

    public void ShootAction(InputAction.CallbackContext context)
    {
        if (context.performed && _canShoot)
        {
            _isShooting = true;
            _shootCooldown = 0f;

        }
        else if (context.canceled)
        {
            _isShooting = false;
        }
    }

    public static IEnumerator DisableShoot(float duration)
    {
        _canShoot = false;
        yield return new WaitForSeconds(duration);
        _canShoot = true;
    }

    private void FixedUpdate()
    {
        if (_canShoot && _isShooting)
        {
            Shoot();
            _canShoot = false;
            _isShooting = _isAutomatic;
        }

    }

    public void Shoot()
    {
        SoundManager.Instance.PlaySound(_smallLaserClip);
        float angle = Ship.Instance.transform.localRotation.eulerAngles.z;
        Vector2 direction = Vector2.up.Rotate(angle);
        Debug.Log(direction);

        for (int i = 0; i < 2*_projectileNumber; i++)
        {
            //Quaternion.AngleAxis((i - 0.5f * (_projectileNumber - 1)) * _projectileSpread, Vector3.forward) * firePoint.right;
            spreadAngle.eulerAngles = new Vector3(0, 0, -90 + Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
            Bullet bullet = _pool.Get();
            var firePoint = i % 2 == 0 ? firePointLeft : firePointRight;
            bullet.transform.SetPositionAndRotation(firePoint.position, spreadAngle);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = (direction * _gameParams.BulletForce);
        }
    }
}