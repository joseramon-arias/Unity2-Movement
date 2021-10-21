using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class InputActionListener : MonoBehaviour
{
    // Specific input from InputAction asset
    [SerializeField] private InputActionReference _action;

    public UnityEvent OnPerformed;

    private void OnEnable()
    {
        _action.action.performed += Performed;
    }

    public void InvokeOnPerformed()
    {
        OnPerformed.Invoke();
    }

    private void Performed(InputAction.CallbackContext obj)
    {
        OnPerformed.Invoke();
    }

    private void OnDisable()
    {
        _action.action.performed -= Performed;   
    }
}
