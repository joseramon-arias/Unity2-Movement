using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLockState : MonoBehaviour
{
    public void SetLocked()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetUnlocked()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetConfined()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
}
