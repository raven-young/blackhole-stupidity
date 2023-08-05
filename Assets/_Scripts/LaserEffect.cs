using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour
{
    private LineRenderer _laserAimLineRenderer;
    private Vector3[] _laserPositions = new Vector3[2];
    [SerializeField] private Transform _target; // the question asteroid

    private void Start()
    {
        _laserAimLineRenderer = GetComponent<LineRenderer>();
        _laserAimLineRenderer.GetComponent<LineRenderer>().enabled = false;
    }

    private void Update()
    {
        if (GameManager.Instance.GameHasEnded)
        {
            _laserAimLineRenderer.GetComponent<LineRenderer>().enabled = false;
            return;
        }

        _laserPositions[0] = transform.position;
        _laserPositions[1] = _target.position;
        _laserAimLineRenderer.SetPositions(_laserPositions);
    }

    public IEnumerator ActivateLaser(float duration)
    {
        _laserAimLineRenderer.GetComponent<LineRenderer>().enabled = true;
        yield return new WaitForSeconds(duration);
        _laserAimLineRenderer.GetComponent<LineRenderer>().enabled = false;
    }
}