using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DamageNumbersPro;
using UnityEngine.UI;
using DG.Tweening;

namespace BlackHole
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] private GameParams _gameParams;
        private PlayerInputActions _playerInputActions;
        public static Ship Instance;

        [Header("Movement")]
        private float _movementX;
        [SerializeField] private Rigidbody2D _rb;

        private float _theta = Mathf.PI / 2;
        public float ShipPositionRadius; // distance from black hole
        private float _angularVelocity;
        private float _dragAngularVelocityMultiplier = 2f;
        private delegate void HandleMovement();
        private HandleMovement _handleMovement;
        [SerializeField] private Camera _cam;

        [Header("Logic")]
        [SerializeField] private GameObject _itemMagnet;
        private Material _itemMagnetMaterial;
        [SerializeField] public float InitialFuel = 100;
        public float CurrentHealth;
        public float CurrentFuel;
        private float _burnRate;
        public bool TakenDamage = false; // for no damage taken achievement

        public bool CannotMove = false;
        public bool IsInvincible = false;
        public bool IsOverdriveActive = false;

        [Header("Effects")]
        [SerializeField] private GameObject deathEffect;
        [SerializeField] private GameObject PlayerHitEffect;
        [SerializeField] private DamageNumber dodgePrefab;
        [SerializeField] private FlashColor flashEffect;
        [SerializeField] private ParticleSystem _exhaustParticles;
        [SerializeField] private AudioClip _explosionClip;
        private float _exhaustEmissionRate;
        private float _exhaustSpeed;


        [Header("Debug")]
        [SerializeField] private bool _gravityOn = false;
        [Range(-5, 5)]
        [SerializeField] private float _gravityScale;
        private static bool _debugVelocityMultiplierOn = false;

        private void Awake()
        {
            Instance = this;
            _playerInputActions = new PlayerInputActions();
            CurrentHealth = _gameParams.MaxHealth / 2;
            CurrentFuel = InitialFuel;
        }

        private void OnEnable()
        {
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Special.performed += ActivateOverdrive;
        }

        void OnDisable()
        {
            _playerInputActions.Player.Disable();
            _playerInputActions.Player.Special.performed -= ActivateOverdrive;
        }

        public void InitializeShip()
        {
            ShipPositionRadius = transform.position.y;
            _burnRate = SettingsManager.BurnRate;

            _exhaustEmissionRate = 1.5f * _exhaustParticles.emission.rateOverTime.constant;
            _exhaustSpeed = 1.5f * _exhaustParticles.main.startSpeed.constant;

            _itemMagnet.transform.localScale = new Vector3(SettingsManager.MagnetScale, SettingsManager.MagnetScale, SettingsManager.MagnetScale);
            _itemMagnetMaterial = _itemMagnet.transform.GetComponent<SpriteRenderer>().material;

            _handleMovement = HanldeMovementStickOrKeyboard;
            _angularVelocity = _gameParams.AngularVelocity;
        }

        private void Update()
        {
            if (CannotMove)
                return;

            if (ShipPositionRadius > _gameParams.WinRadius && !GameManager.GameHasEnded)
            {
                CannotMove = true;
                StartCoroutine(GameManager.Instance.GameOver(true));
                return;
            }

            if (CurrentHealth <= 0)
            {
                CannotMove = true;
                StartCoroutine(GameManager.Instance.GameOver(false));
                return;
            }

            CurrentFuel -= _burnRate * Time.deltaTime;
            CanvasManager.Instance.UpdateFuel(CurrentFuel);
            UpdateExhaustParticles(); // to do: update at lower framerate using invokerepeating

            /// Movement
            //_movementX = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
            _handleMovement();
            //Debug.Log(_movementX);

            if (_gravityOn)
                ShipPositionRadius -= _gravityScale * Time.deltaTime;

            // calculate radius based on ship health and black hole mass
            var ratio = CurrentHealth / BlackHoleObject.Instance.CurrentForce;
            var velocity = CurrentFuel <= 0 ? -40 : ratio > 1 ? ratio : ratio == 1 ? 0 : -1 / ratio;
            if (_debugVelocityMultiplierOn)
                velocity *= 100;

            ShipPositionRadius += _gameParams.RadialVelocityScale * velocity * Time.deltaTime;

            // rotate ship
            transform.rotation = Quaternion.Euler(0, 0, _theta * Mathf.Rad2Deg - 90);

        }

        private void HanldeMovementStickOrKeyboard()
        {
            _movementX = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
        }

        private void HandleMovementTouchDrag()
        {
            Vector2 pos = _playerInputActions.Player.Touch.ReadValue<Vector2>();

            // if touch position at bottom of screen, ignore
            if (pos.y < 0.2f * Screen.height)
            {
                _movementX = 0;
                return;
            }

            Vector3 pos3 = _cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 0));
            float theta = Mathf.Atan2(pos3.y, pos3.x);

            // prevent ship stuttering around last recorded touch position
            if (Mathf.Abs(theta - _theta) < 0.04) 
            { 
                _movementX = 0; 
                return; 
            }

            _movementX = theta > _theta ? -1 : 1;
        }

        public void ToggleDragMovement(bool activateDrag)
        {
            if (activateDrag)
            {
                _angularVelocity *= _dragAngularVelocityMultiplier;
                _handleMovement = HandleMovementTouchDrag;
            }
            else
            {
                _angularVelocity /= _dragAngularVelocityMultiplier;
                _handleMovement = HanldeMovementStickOrKeyboard;
            }
        }

        private void FixedUpdate()
        {
            if (CannotMove) { return; }

            _theta -= Time.fixedDeltaTime * _angularVelocity * _movementX;

            // clamp angle to screen bounds
            // if using flexible aspect ratio: max angle depends on radius and screenbounds:
            // float thetaMax = Mathf.Acos(screenBounds.x / ShipPositionRadius); //
            // else use fixed max theta
            float thetaMax = _gameParams.MaxTheta;
            _theta = Mathf.Clamp(_theta, thetaMax, Mathf.PI - thetaMax);

            Vector2 newpos = new Vector2(ShipPositionRadius * Mathf.Cos(_theta), ShipPositionRadius * Mathf.Sin(_theta));
            _rb.MovePosition(newpos);
        }

        public static void ToggleDebugVelocity(bool debugvelocityOn)
        {
            _debugVelocityMultiplierOn = debugvelocityOn;
        }

        private void UpdateExhaustParticles()
        {
            var emission = _exhaustParticles.emission;
            emission.rateOverTime = CurrentFuel <= 0 ? 0 : _exhaustEmissionRate * CurrentHealth / _gameParams.MaxHealth;
            var main = _exhaustParticles.main;
            main.startSpeed = _exhaustSpeed * CurrentHealth / _gameParams.MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (IsInvincible) { return; }

            TakenDamage = true;
            flashEffect.Flash();
            CurrentHealth -= damage;
            CanvasManager.Instance.UpdateHealth(CurrentHealth);
            StartCoroutine(ActivateInvincibility(0.2f));
        }

        IEnumerator ActivateInvincibility(float duration)
        {
            IsInvincible = true;
            yield return new WaitForSeconds(duration);
            if (!CannotMove) { IsInvincible = false; }
        }

        public IEnumerator Die()
        {
            // death animation
            Vector3 explosionOffset = new(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-1.2f, 1.2f), 0);
            GameObject effect1 = Instantiate(deathEffect, transform.position + explosionOffset, Quaternion.identity);
            Destroy(effect1, 1f);
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            yield return new WaitForSeconds(0.5f);

            Vector3 explosionOffset2 = new(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-1.2f, 1.2f), 0);
            GameObject effect2 = Instantiate(deathEffect, transform.position + explosionOffset2, Quaternion.identity);
            Destroy(effect2, 1f);
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            yield return new WaitForSeconds(0.6f);

            Vector3 explosionOffset3 = new(UnityEngine.Random.Range(-1.2f, 1.2f), UnityEngine.Random.Range(-1.2f, 1.2f), 0);
            GameObject effect3 = Instantiate(deathEffect, transform.position + explosionOffset3, Quaternion.identity);
            Destroy(effect3, 1f);
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            yield return new WaitForSeconds(0.4f);

            yield return new WaitForSeconds(1.5f);

            CannotMove = true;
            IsInvincible = true;
            gameObject.SetActive(false);
            GameObject finaleffect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            finaleffect.transform.localScale *= 2f;
            Destroy(finaleffect, 1f);
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.PlaySound(_explosionClip, 0.5f);
            yield return new WaitForSeconds(2f);
        }

        private void ActivateOverdrive(InputAction.CallbackContext context)
        {

            if (context.performed && CurrentHealth >= _gameParams.MaxHealth && !IsOverdriveActive)
            {
                SoundManager.Instance.PlayButtonPress(failed: false);
                StartCoroutine(OverDrive());
            }
            else
            {
                SoundManager.Instance.PlayButtonPress(failed: true);
            }
        }

        public void ResetShipPosition()
        {
            CannotMove = false;
            ShipPositionRadius -= 0.5f * GameManager.DistanceToEventHorizon;
        }

        private IEnumerator OverDrive()
        {
            TakeDamage((int)(0.8f * CurrentHealth));
            IsOverdriveActive = true;
            _itemMagnet.transform.localScale *= _gameParams.OverdriveMagnetScale;
            _itemMagnetMaterial.DOFloat(1, "_HologramBlend", 1f);
            _itemMagnetMaterial.DOFloat(8, "_Glow", 1f);
            //SoundManager.Instance.ChangeMusicSourcePitch(1.01f, 1f);
            ActivateInvincibility(_gameParams.OverdriveDuration);
            yield return new WaitForSecondsRealtime(_gameParams.OverdriveDuration);
            IsOverdriveActive = false;
            _itemMagnet.transform.localScale /= _gameParams.OverdriveMagnetScale;
            _itemMagnetMaterial.DOFloat(0, "_HologramBlend", 1f);
            _itemMagnetMaterial.DOFloat(0, "_Glow", 1f);
            SoundManager.Instance.ChangeMusicSourcePitch(1f, 1f);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject.layer == 11)
                Debug.Log("enter " + collision.gameObject.layer);

            if (collision.gameObject.layer == 10) // black hole
            {
                StartCoroutine(GameManager.Instance.GameOver(victorious: false));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 11)
                Debug.Log("exit " + collision.gameObject.layer);
        }
    }
}
