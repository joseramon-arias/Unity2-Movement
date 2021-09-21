using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Attacking")]
    [SerializeField] private float _attackDistance = 5f;

    [Header("Visibility")]
    [SerializeField] private LayerMask _occlusionMask;
    [SerializeField] private LayerMask _targetingMask;
    [SerializeField] private float _viewDistance = 10f;
    [SerializeField] private float _viewHalfAngle = 45;

    private CharacterMovement _characterMovement;
    private Targetable _target;
    private Targetable _myTargetable;

    public bool IsTargetValid => _target != null && _target.IsTargetable;

    private void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _myTargetable = GetComponent<Targetable>();
    }

    private void Update()
    {
        if (!IsTargetValid) TryFindTarget();

        if (!IsTargetValid)
        {
            _characterMovement.StopMovement();
            return;
        }

        float targetDistance = Vector3.Distance(transform.position, _target.transform.position);
        bool targetVisible = TestVisibility(_target.AimPosition.position);

        if (targetDistance > _attackDistance || !targetVisible)
        {
            _characterMovement.MoveTo(_target.transform.position);
        }
        else
        {
            _characterMovement.StopMovement();
            Vector3 targetDirection = (_target.transform.position - transform.position).normalized;
            _characterMovement.SetLookDirection(targetDirection);
        }
    }

    private bool TestVisibility(Vector3 targetPosition)
    {
        Vector3 start = _myTargetable.AimPosition.position;
        Vector3 end = targetPosition;

        // Vector to target
        Vector3 vectorToTarget = end - start;
        float targetDistance = vectorToTarget.magnitude;
        if (targetDistance > _viewDistance) return false;

        // check view angle of target
        Vector3 targetDirection = vectorToTarget.normalized;
        float targetViewHalfAngle = Vector3.Angle(targetDirection, transform.forward);
        if (targetViewHalfAngle > _viewHalfAngle) return false;

        // linecast to test for occlusion, if we hit something, the target position isn't visible
        if (Physics.Linecast(start, end, _occlusionMask))
        {
            Debug.DrawLine(start, end, Color.red);
            return false;
        }

        Debug.DrawLine(start, end, Color.green);
        return true;
    }

    private void TryFindTarget()
    {
        // Find all colliders within radius, within layerMask
        Collider[] hits = Physics.OverlapSphere(_myTargetable.AimPosition.position, _viewDistance, _targetingMask);

        // Iterate through all possible targets
        foreach (Collider hit in hits)
        {
            Debug.DrawLine(_myTargetable.AimPosition.position, hit.transform.position, Color.white);

            // Check for valid target
            if (hit.TryGetComponent(out Targetable possibleTarget) &&
                possibleTarget.Team != _myTargetable.Team &&
                possibleTarget.IsTargetable &&
                TestVisibility(possibleTarget.AimPosition.position))
            {
                _target = possibleTarget;
                break;
            }
        }
    }
}
