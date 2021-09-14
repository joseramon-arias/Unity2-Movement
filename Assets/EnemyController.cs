using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
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
        _characterMovement.MoveTo(_player.transform.position);
    }
}
