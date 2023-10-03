using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerSpawner : MonoBehaviour
{
    public enum SpawnState { Small, Super, Fire };

    [Range(-1, 1)]
    [SerializeField] int _movingDirection;
    [SerializeField] GameObject _playerToSpawn;

    private void Awake()
    {
        SpawnPlayer(_playerToSpawn);
    }

    private void Start()
    {
        
    }

    void SpawnPlayer(GameObject playerToSpawn)
    {
        //set new players moving direction through inspector
        GameObject newPlayer = Instantiate(playerToSpawn, transform.position, transform.rotation);
        MarioMovement _movementScript = newPlayer.GetComponent<MarioMovement>();
        _movementScript._movingDirection = _movingDirection;
        //change virtual camera's target to the new instantiated player
        CinemachineVirtualCamera activeCamera = FindObjectOfType<CinemachineVirtualCamera>();
        activeCamera.m_Follow = newPlayer.transform;
    }

}
