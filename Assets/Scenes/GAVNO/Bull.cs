using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bull : MonoBehaviour
{
    [SerializeField] private KeyCode _castRayBind = KeyCode.S;
    [SerializeField] private LayerMask _hittableLayers;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    [SerializeField] private bool _enableGizmos;
    [SerializeField] private bool _transformForwardTest;
    [SerializeField] private Color _gizmosColor = Color.cyan;

    private Ray? _r;


    private void OnDrawGizmos()
    {
        if (!_enableGizmos)
            return;

        if (_startPoint == null)
            return;


        Gizmos.color = _gizmosColor;

        if (_transformForwardTest)
        {
            Gizmos.DrawSphere(_startPoint.position, 0.5f);
            Gizmos.DrawCube(_startPoint.position + _startPoint.forward, Vector3.one * 0.5f);
        }


        if (_endPoint == null)
            return;

        if (_r.HasValue)
            Gizmos.DrawRay(_r.Value);

    }

    private void Update()
    {
        if (Input.GetKeyDown(_castRayBind))
            CastRay();
    }


    private void CastRay()
    {
        if (_hittableLayers == 0)
            throw new System.NullReferenceException("Layer Mask is empty.");

        _r = new Ray(_startPoint.position, _endPoint.position - _startPoint.position);
        var hits = Physics.RaycastAll(_r.Value, Vector3.Distance(_startPoint.position, _endPoint.position), _hittableLayers, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            var n = h.collider.gameObject.name;
            Debug.Log(n);
        }

    }
}
