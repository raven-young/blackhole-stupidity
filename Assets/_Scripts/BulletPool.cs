using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace BlackHole
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private int defaultBulletCapacity = 800;
        [SerializeField] private int maxBulletCapacity = 1000;

        public IObjectPool<Bullet> BulletPoolInstance;

        private void Awake()
        {
            BulletPoolInstance = new ObjectPool<Bullet>(() =>
            {
                Bullet bullet = Instantiate(bulletPrefab);
                bullet.SetPool(BulletPoolInstance);
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
            }, false, defaultBulletCapacity, maxBulletCapacity
            );

        }
    }
}