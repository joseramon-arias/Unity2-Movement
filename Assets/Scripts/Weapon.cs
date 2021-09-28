using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _roundsPerMinute = 300f;
    [SerializeField] private float _range = 20f;
    [SerializeField] private LayerMask _hitMask;

    private float _lastFireTime;

    public void TryFire(Vector3 aimPosition, int myTeam)
    {
        if (Time.time < _lastFireTime + 1f / _roundsPerMinute) return;
        _lastFireTime = Time.time;

        Vector3 aimDirection = (aimPosition - transform.position).normalized;
        Vector3 start = transform.position;

        if (Physics.Raycast(start, aimDirection, out RaycastHit hit, _range, _hitMask))
        {
            if (hit.collider.TryGetComponent(out Targetable target) &&
                target.Team != myTeam &&
                hit.collider.TryGetComponent(out Health health))
            {
                health.Damage(_damage);
                Debug.DrawLine(start, hit.point, Color.red, 0.25f);
            }
        }
    }
}
