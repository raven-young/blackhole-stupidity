using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour
{
    public static LaserEffect Instance;
    private LineRenderer _laserAimLineRenderer;
    private Vector3[] _laserPositions = new Vector3[2];
    [SerializeField] private Transform _target; // the question asteroid

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _laserAimLineRenderer = GetComponent<LineRenderer>();
    }

    public IEnumerator ActivateLaser(float duration)
    {
        _laserPositions[0] = transform.position;
        _laserPositions[1] = _target.position;
        _laserAimLineRenderer.SetPositions(_laserPositions);
        gameObject.GetComponent<LineRenderer>().enabled = true;
        yield return new WaitForSeconds(duration);
        gameObject.GetComponent<LineRenderer>().enabled = false;
    }
}