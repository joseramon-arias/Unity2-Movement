using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2 _moveInput;
    private CharacterMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        _movement.TryJump();
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
    }
}
