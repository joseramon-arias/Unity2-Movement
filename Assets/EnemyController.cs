using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _attackDistance = 5f;
    [SerializeField] private LayerMask _occlusionMask;
    [SerializeField] private Vector3 _lookOffset = new Vector3(0f, 1.5f, 0f);

    private CharacterMovement _characterMovement;
    private GameObject _player;

    private void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();

        // very hacky - replace later
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        float targetDistance = Vector3.Distance(transform.position, _player.transform.position);
        bool targetVisible = TestVisibility(_player.transform.position + _lookOffset);

        if (targetDistance > _attackDistance || !targetVisible)
        {
            _characterMovement.MoveTo(_player.transform.position);
        }
        else
        {
            _characterMovement.StopMovement();
            Vector3 targetDirection = (_player.transform.position - transform.position).normalized;
            _characterMovement.SetLookDirection(targetDirection);
        }
    }

    private bool TestVisibility(Vector3 targetPosition)
    {
        Vector3 start = transform.position + _lookOffset;
        Vector3 end = targetPosition;

        // linecast to test for occlusion, if we hit something, the target position isn't visible
        if (Physics.Linecast(start, end, _occlusionMask))
        {
            Debug.DrawLine(start, end, Color.red);
            return false;
        }

        Debug.DrawLine(start, end, Color.green);
        return true;
    }
}
