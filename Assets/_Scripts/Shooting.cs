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

    //[SerializeField] private static Transform GunPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;
    private Slider ammoSlider;

    private Quaternion spreadAngle = Quaternion.identity;

    private PlayerInputActions playerInputActions;

    private bool _canShoot = true;
    private bool _isAutomatic = true;
    private bool _isShooting = false;
    [SerializeField] private int _projectileNumber = 1;
    [SerializeField] private float _projectileSpread = 30f;
    private float _shootCooldown;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private float _bulletForce = 1f;

    private static IObjectPool<Bullet> _pool;
    //public static int remainingAmmo;
    //private static bool isReloading;
    //private static TMP_Text ammoText;
    private static Shooting Instance;

    //[SerializeField] float knockbackStrength = 0.05f, knockbackDuration = 0.02f;

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
        //playerInputActions.Player.Reload.performed += ReloadAction;

        _pool = new ObjectPool<Bullet>(() =>
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.SetPool(_pool);
            return bullet;
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
            //bullet.piercing = PlayerStats.piercingBonus.Value;
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, false, 600, 1000
        );
        //isReloading = false;
    }

    private void Start()
    {
        //ToggleLaserAim(WeaponSelect.Instance.hasLaserSight);
        //remainingAmmo = WeaponSelect.Instance.clipSize;
        //var AmmoBox = CanvasManager.Instance.transform.Find("ScreenCanvas").transform.Find("AmmoBox");
        //ammoText = AmmoBox.transform.Find("AmmoText").GetComponent<TMP_Text>();
        //ammoSlider = AmmoBox.GetComponent<Slider>();
        //ammoSlider.minValue = 0;
        //ammoSlider.maxValue = WeaponSelect.Instance.clipSize;
        //RenderAmmoText();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Fire.performed -= ShootAction;
        playerInputActions.Player.Fire.canceled -= ShootAction;
        //playerInputActions.Player.Reload.performed -= ReloadAction;
        playerInputActions.Player.Disable();
    }

    private void Update()
    {

        _shootCooldown += Time.deltaTime;
        if (_shootCooldown > _fireRate)
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
            //InvokeRepeating("Shoot", .01f, Mathf.Max(0.1f, PlayerStats.FireRate.Value));

        }
        else if (context.canceled)
        {
            _isShooting = false;
            // CancelInvoke("Shoot");
        }
    }

    private void FixedUpdate()
    {
        if (_canShoot && _isShooting)//  && remainingAmmo > 0 && !isReloading)
        {
            Shoot();
            _canShoot = false;
            //SoundManager.Instance.PlaySound(_activeWeaponClip);
            _isShooting = _isAutomatic;
        }
        //else if (remainingAmmo < 1 && !isReloading)
        //{
        //    Instance.StartCoroutine(Reload());
        //}

    }

    public void Shoot()
    {
        //StartCoroutine(Player.Instance.knockback(GunPivot.right * -knockbackStrength * PlayerStats.BulletForce.Value, knockbackDuration));
        //ShockEvent.Instance.Shock();
        for (int i = 0; i < _projectileNumber; i++)
        {
            Vector2 direction = Vector2.up;//Quaternion.AngleAxis((i - 0.5f * (_projectileNumber - 1)) * _projectileSpread, Vector3.forward) * firePoint.right;
            //spreadAngle.eulerAngles = new Vector3(0, 0, -90 + Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
            Bullet bullet = _pool.Get();
            bullet.transform.SetPositionAndRotation(firePoint.position, spreadAngle);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = (direction * _bulletForce);
            //rb.AddForce(direction * PlayerStats.BulletForce.Value, ForceMode2D.Impulse);
        }

        //remainingAmmo--;
        //RenderAmmoText();
    }

    //void ReloadAction(InputAction.CallbackContext context)
    //{
    //    if (context.performed && !isReloading)
    //    {
    //        StartCoroutine(Reload());
    //    }
    //}

    //public static IEnumerator Reload()
    //{
    //    isReloading = true;
    //    ammoSlider.DOValue(WeaponSelect.Instance.clipSize, WeaponSelect.Instance.reloadDuration).SetEase(Ease.Flash);
    //    var audioLength = WeaponSelect.Instance.activeReloadClip.length;
    //    if (audioLength < WeaponSelect.Instance.reloadDuration)
    //    {
    //        yield return new WaitForSeconds(WeaponSelect.Instance.reloadDuration - audioLength);
    //        SoundManager.Instance.PlaySound(WeaponSelect.Instance.activeReloadClip);
    //        yield return new WaitForSeconds(audioLength);

    //    }
    //    else
    //    {
    //        SoundManager.Instance.PlaySound(WeaponSelect.Instance.activeReloadClip);
    //        yield return new WaitForSeconds(WeaponSelect.Instance.reloadDuration);
    //    }
    //    remainingAmmo = WeaponSelect.Instance.clipSize;
    //    isReloading = false;
    //    shootCooldown = 0f;
    //    canShoot = true;
    //    RenderAmmoText();
    //}

    //private static void RenderAmmoText()
    //{
    //    ammoSlider.value = remainingAmmo;
    //    ammoText.text = remainingAmmo + "/" + WeaponSelect.Instance.clipSize;
    //    ammoText.color = remainingAmmo < 0.2f * WeaponSelect.Instance.clipSize ? Color.red : Color.white;
    //}
    //public static void InstantReload()
    //{
    //    remainingAmmo = WeaponSelect.Instance.clipSize;
    //    if (isReloading)
    //    {
    //        Instance.StopAllCoroutines();
    //        isReloading = false;
    //    }
    //    shootCooldown = 0f;
    //    canShoot = true;
    //    if (ammoSlider != null)
    //        RenderAmmoText();
    //}

    //public static void AddAmmo(int ammo)
    //{
    //    if (remainingAmmo >= WeaponSelect.Instance.clipSize || isReloading)
    //        return;
    //    remainingAmmo += ammo;
    //    RenderAmmoText();
    //}

    //public static void ToggleLaserAim(bool hasLaserSight)
    //{
    //    if (firePoint != null)
    //        firePoint.GetComponent<LineRenderer>().enabled = hasLaserSight;
    //}

    //public static void UpdateWeapon()
    //{
    //    if (ammoSlider != null)
    //        ammoSlider.maxValue = WeaponSelect.Instance.clipSize;
    //}
}