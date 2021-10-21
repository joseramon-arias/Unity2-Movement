using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _turnSpeed = 10f;
    [SerializeField] private float _acceleration = 10f;

    [Header("Jumping")]
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _gravity = 20f;
    [SerializeField] private float _airControl = 0.1f;
    [SerializeField] private int _totalJumps = 2;
    private int _jumpCounter = 0;

    [Header("Grounding")]
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private float _groundCheckOffset = 0.5f;
    [SerializeField] private float _groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask _groundCheckMask;

    [Header("Dashing")]
    [SerializeField] private float _dashSpeed = 1000f;
    [SerializeField] private float _dashCooldown = 0.5f;
    private bool _dashReady = true;

    [Header("Sprinting")]
    [SerializeField] private float _sprintSpeed = 8f;
    public bool IsSprinting { private get; set; }

    private Rigidbody _rb;
    private Vector3 _moveInput;
    private Vector3 _lookDirection;
    private NavMeshAgent _navMeshAgent;

    public bool IsGrounded { get; private set; }
    public Vector3 MoveInput => _moveInput;
    // Converts world-space _moveInput to local space direction
    public Vector3 LocalMoveInput => transform.InverseTransformDirection(_moveInput);


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;

        _lookDirection = transform.forward;
    }

    public void TryJump()
    {
        // If the player is grounded or still have jumps available, jump
        if (IsGrounded || _jumpCounter < _totalJumps)
        {
            float jumpVelocity = Mathf.Sqrt(2f * _gravity * _jumpHeight);
            // override vertical velocity
            Vector3 velocity = _rb.velocity;
            velocity.y = jumpVelocity;
            _jumpCounter += 1;
            _rb.velocity = velocity;
        }
    }

    public void TryDash()
    {
        if (_dashReady)
        {
            // If the player is not moving, dash forward
            if (_moveInput == Vector3.zero)
            {
                _rb.AddForce(transform.forward * _dashSpeed);
            }
            // if the player is moving, dash towards the moving direction
            else
            {
                _rb.AddForce(_moveInput.normalized * _dashSpeed);
            }

            StartCoroutine(CountDashCooldown());
        }
    }

    private IEnumerator CountDashCooldown()
    {
        // Deactivate dash until _dashCooldown time is up, and then reactivate it again
        float currentCooldown = _dashCooldown;
        _dashReady = false;
        while (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            yield return null;
        }

        _dashReady = true;
    }

    public void SetMoveInput(Vector3 input)
    {
        _moveInput = input;
    }

    public void MoveTo(Vector3 position, float stoppingDistance = 0.5f)
    {
        float distance = Vector3.Distance(position, transform.position);
        if (distance < stoppingDistance) StopMovement();
        else _navMeshAgent.SetDestination(position);
    }

    public void StopMovement()
    {
        _navMeshAgent.ResetPath();
        SetMoveInput(Vector3.zero);
    }

    private void Update()
    {
        bool WasNotGrounded = !IsGrounded;
        IsGrounded = CheckGrounded();

        if (WasNotGrounded && IsGrounded) _jumpCounter = 0;

        // check if NavMeshAgent has a path
        if (_navMeshAgent.hasPath)
        {
            // get the next point on the path
            Vector3 next = _navMeshAgent.path.corners[1];
            Vector3 direction = (next - transform.position).normalized;
            // set move/look direction towards next point
            SetMoveInput(direction);
            SetLookDirection(direction);
        }

        // Quaternion.LookRotation turns a Vector3 (direction) into a Quaternion (rotation)
        Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
        Quaternion currentRotation = transform.rotation;

        Quaternion rotation = Quaternion.Slerp(currentRotation, targetRotation, _turnSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }

    private void FixedUpdate()
    {
        // If player is sprinting, use _sprintSpeed; else, use speed
        float newSpeed = IsSprinting ? _sprintSpeed : _speed;
        Vector3 targetVelocity = _moveInput * newSpeed;
        Vector3 currentVelocity = _rb.velocity;
        // calculate desired and current velocities, and difference between them
        Vector3 velocityDiff = targetVelocity - currentVelocity;
        velocityDiff.y = 0;

        // accelerate up to target velocity
        float airModifier = IsGrounded ? 1f : _airControl;
        Vector3 moveForce = velocityDiff * _acceleration * airModifier;
        moveForce += Vector3.down * _gravity;
        _rb.AddForce(moveForce);
    }

    public void SetLookDirection(Vector3 forward)
    {
        // flatten and normalized look direction
        forward.y = 0;
        forward.Normalize();
        _lookDirection = forward;
    }

    private bool CheckGrounded()
    {
        // find starting point for spherecast
        Vector3 start = transform.position + transform.up * _groundCheckOffset;
        if (Physics.SphereCast(start, _groundCheckRadius, -transform.up, out RaycastHit hit, _groundCheckDistance, _groundCheckMask))
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 start = transform.position + transform.up * _groundCheckOffset;
        Vector3 end = start - transform.up * _groundCheckDistance;

        Gizmos.DrawWireSphere(start, _groundCheckRadius);
        Gizmos.DrawWireSphere(end, _groundCheckRadius);
    }
}
