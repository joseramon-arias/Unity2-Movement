using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    [SerializeField] private int _team = 1;
    [SerializeField] private bool _isTargetable = true;
    [SerializeField] private Transform _aimPosition;

    public int Team => _team;
    public bool IsTargetable { get => _isTargetable; set => _isTargetable = value; }
    public Transform AimPosition { get => _aimPosition; }
}
