using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _current = 100f;
    [SerializeField] private float _max = 100f;

    public float Percentage => _current / _max;
    public bool IsAlive => _current > 0f;

    public void Damage(float amount)
    {
        if (!IsAlive) return;

        if (amount < 0) Debug.LogWarning("Write a Heal() function, you dummy.");

        _current = Mathf.Clamp(_current - amount, 0f, _max);
    }
}
