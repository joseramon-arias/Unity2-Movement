using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2 _moveInput;
    private CharacterMovement _movement;
    private Weapon _weapon;
    private bool _isFiring;
    private Targetable _targetable;

    private void Awake()
    {
        _movement = GetComponent<CharacterMovement>();

        _weapon = GetComponentInChildren<Weapon>();

        _targetable = GetComponent<Targetable>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        _movement.TryJump();
    }

    public void OnFireStart()
    {
        _isFiring = true;
    }

    public void OnFireStop()
    {
        _isFiring = false;
    }

    public void OnDash()
    {
        _movement.TryDash();
    }

    public void OnSprintStart()
    {
        _movement.IsSprinting = true;
    }

    public void OnSprintStop()
    {
        _movement.IsSprinting = false;
    }

    private void Update()
    {
        // cross product to find out where the camera is facing
        Vector3 right = Camera.main.transform.right;
        Vector3 up = Vector3.up;
        // cross product returns the orthogonal vector to input directions
        Vector3 forward = Vector3.Cross(right, up);

        // combine right and forward vectors with input magnitudes to determine final move input
        Vector3 moveInput = forward * _moveInput.y + right * _moveInput.x;

        _movement.SetMoveInput(moveInput);

        // look in forward direction
        _movement.SetLookDirection(forward);

        // aim at a point 20 meters in front of camera
        Vector3 aimPosition = Camera.main.transform.position + Camera.main.transform.forward * 20f;

        // attempt to fire weapon
        if (_isFiring) _weapon.TryFire(aimPosition, _targetable.Team);
    }
}
