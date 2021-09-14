using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    [SerializeField] private float _dampTime = 0.1f;
    private CharacterMovement _movement;
    private Animator _animator;

    private void Start()
    {
        _movement = GetComponent<CharacterMovement>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetFloat("Right", _movement.LocalMoveInput.x, _dampTime, Time.deltaTime);
        _animator.SetFloat("Forward", _movement.LocalMoveInput.z, _dampTime, Time.deltaTime);
    }
}
