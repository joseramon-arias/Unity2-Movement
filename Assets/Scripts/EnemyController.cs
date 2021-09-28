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

    [Header("States")]
    [SerializeField] private float _patrolPointReachedDistance = 1f;
    [SerializeField] private string _currentStateName;

    private CharacterMovement _characterMovement;
    private Targetable _target;
    private Targetable _myTargetable;
    private Weapon _weapon;
    private PatrolPoint[] _patrolPoints;
    private IEnumerator _currentState;

    public bool IsTargetValid => _target != null && _target.IsTargetable;
    public float TargetDistance => Vector3.Distance(_target.AimPosition.position, _myTargetable.AimPosition.position);

    private void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _myTargetable = GetComponent<Targetable>();
        _weapon = GetComponentInChildren<Weapon>();

        _patrolPoints = FindObjectsOfType<PatrolPoint>();
        NextState(PatrolState());
    }

    private PatrolPoint GetRandomPatrolPoint()
    {
        return _patrolPoints[Random.Range(0, _patrolPoints.Length)];
    }

    private void NextState(IEnumerator nextState)
    {
        if (_currentState != null) StopCoroutine(_currentState);

        _currentState = nextState;
        _currentStateName = nextState.ToString();
        StartCoroutine(_currentState);
    }

    private IEnumerator PatrolState()
    {
        PatrolPoint patrolPoint = GetRandomPatrolPoint();

        while(true)
        {
            float patrolDistance = Vector3.Distance(transform.position, patrolPoint.transform.position);
            if (patrolDistance < +_patrolPointReachedDistance) patrolPoint = GetRandomPatrolPoint();

            _characterMovement.MoveTo(patrolPoint.transform.position);
            Debug.DrawLine(transform.position, patrolPoint.transform.position);

            // Find target and chase
            TryFindTarget();
            if (IsTargetValid) NextState(ChaseState());

            yield return null;
        }
    }

    private IEnumerator ChaseState()
    {
        while(IsTargetValid)
        {
            // chase target while out of range or LoS (line of sight)
            if (TargetDistance > _attackDistance || !TestVisibility(_target.AimPosition.position))
            {
                _characterMovement.MoveTo(_target.AimPosition.position);
            }
            else
            {
                // enter attack state
                NextState(AttackState());
            }

            yield return null;
        }

        NextState(PatrolState());
    }

    private IEnumerator AttackState()
    {
        while(IsTargetValid)
        {
            if(TargetDistance < _attackDistance && TestVisibility(_target.AimPosition.position))
            {
                Vector3 dirToTarget = (_target.AimPosition.position - _myTargetable.AimPosition.position).normalized;
                _characterMovement.SetLookDirection(dirToTarget);
                _characterMovement.StopMovement();
                Debug.DrawLine(_target.AimPosition.position, _myTargetable.AimPosition.position, Color.red);

                _weapon.TryFire(_target.AimPosition.position, _myTargetable.Team);
            }
            else
            {
                NextState(ChaseState());
            }

            yield return null;
        }

        // fallback to patrol state
        NextState(PatrolState());
    }

    private void UpdatedaaergsergsetawWFqrCQEf()
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
