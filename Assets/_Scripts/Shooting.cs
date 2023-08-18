using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace BlackHole
{
    public class Shooting : MonoBehaviour
    {

        [SerializeField] private GameParams _gameParams;
        [SerializeField] private Transform _firePointLeft;
        [SerializeField] private Transform _firePointRight;
        [SerializeField] private Transform _firePointMiddle;
        [SerializeField] private Bullet _bulletPrefab;

        private Quaternion _spawnAngle = Quaternion.identity;

        private PlayerInputActions _playerInputActions;

        private static bool _canShoot = true;
        private bool _isAutomatic = true;
        private bool _isShooting = false;
        private bool _isAutoshooting = false;
        private float _shootCooldown;

        private static IObjectPool<Bullet> _pool;
        public static Shooting Instance;

        private delegate void Shoot();
        Shoot _shoot;

        [SerializeField] private AudioClip _smallLaserClip;

        private void Awake()
        {

            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Fire.performed += ShootAction;
            _playerInputActions.Player.Fire.canceled += ShootAction;

            _pool = new ObjectPool<Bullet>(() =>
            {
                Bullet bullet = Instantiate(_bulletPrefab);
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

        private void Start()
        {
            bool tripleShoot = SettingsManager.Instance.SelectedShipType == SettingsManager.ShipType.Destroyer;
            _shoot = tripleShoot ? TripleShoot : DoubleShoot;
            _isAutoshooting = SettingsManager.IsMobileGame;
            _isAutoshooting = true;
        }

        private void OnDisable()
        {
            _playerInputActions.Player.Fire.performed -= ShootAction;
            _playerInputActions.Player.Fire.canceled -= ShootAction;
            _playerInputActions.Player.Disable();
        }

        private void Update()
        {

            _shootCooldown += Time.deltaTime;
            if (_shootCooldown > SettingsManager.FirePeriod)
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
            if (_canShoot && (_isShooting || _isAutoshooting))
            {
                _shoot();
                _canShoot = false;
                _isShooting = _isAutomatic;
            }

        }

        public void DoubleShoot()
        {
            SoundManager.Instance.PlaySound(_smallLaserClip);
            float angle = Ship.Instance.transform.localRotation.eulerAngles.z;
            Vector2 direction = Vector2.up.Rotate(angle);

            for (int i = 0; i < 2; i++)
            {
                _spawnAngle.eulerAngles = new Vector3(0, 0, angle);
                Bullet bullet = _pool.Get();
                var firePoint = i % 2 == 0 ? _firePointLeft : _firePointRight;
                bullet.transform.SetPositionAndRotation(firePoint.position, _spawnAngle);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.velocity = (direction * _gameParams.BulletVelocity);
            }
        }

        public void TripleShoot()
        {
            SoundManager.Instance.PlaySound(_smallLaserClip);
            float angle = Ship.Instance.transform.localRotation.eulerAngles.z;

            for (int i = 0; i < 3; i++)
            {
                Bullet bullet = _pool.Get();
                Transform firePoint;
                Vector2 direction;
                switch (i)
                {
                    case 0:
                        firePoint = _firePointLeft;
                        direction = Vector2.up.Rotate(angle + 10);
                        _spawnAngle.eulerAngles = new Vector3(0, 0, angle + 10f);
                        break;
                    case 1:
                        firePoint = _firePointMiddle;
                        _spawnAngle.eulerAngles = new Vector3(0, 0, angle);
                        direction = Vector2.up.Rotate(angle);
                        break;
                    default:
                        firePoint = _firePointRight;
                        direction = Vector2.up.Rotate(angle - 10);
                        _spawnAngle.eulerAngles = new Vector3(0, 0, angle - 10f);
                        break;
                }
                bullet.transform.SetPositionAndRotation(firePoint.position, _spawnAngle);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.velocity = (direction * _gameParams.BulletVelocity);
            }
        }

        public void ToggleAutoshoot(bool autoshootOn)
        {
            _isAutoshooting = autoshootOn;
        }
    }
}