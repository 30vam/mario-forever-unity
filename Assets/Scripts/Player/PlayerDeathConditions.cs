using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerDeathConditions : MonoBehaviour
{

    [SerializeField] GameObject _deadPlayerPrefab;
    [SerializeField] GameObject _smallMarioPrefab;
    [SerializeField] GameObject _superMarioPrefab;
    [SerializeField] AudioClip _powerdownSfx;
    private bool _wasAlreadyDamaged = false;

    MarioMovement _marioMovementScript;
    AudioSource _marioAudioSource;

    private void Awake()
    {
        _marioMovementScript = GetComponent<MarioMovement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        //The enemy damage hitbox is defferent than its physics hitbox
        if (collision.gameObject.tag == "KillPlayer")
            KillPlayer();
        if (collision.gameObject.tag == "EnemyActivator" && CanBeHurtByEnemy(gameObject, collision.gameObject))
            DamagePlayer();
    }

    private bool CanBeHurtByEnemy(GameObject player, GameObject enemyInstance)
    {
        if (!_wasAlreadyDamaged && !_marioMovementScript._isPoweringDown)
        {
            //need this bool, otherwise sometimes 2 players can spawn
            _wasAlreadyDamaged = true;
            return true;
        }
            
        else
            return false;
    }

    private void DamagePlayer()
    {
            switch (_marioMovementScript._powerupState)
            {
                case MarioMovement.PowerupStates.Small:
                    KillPlayer();
                    break;
                case MarioMovement.PowerupStates.Super:
                    {
                    //spawn it a little above small mario because super mario has a different size
                    InstantiateNewPlayer(gameObject, _smallMarioPrefab, new Vector2(transform.position.x, transform.position.y - 0.5f), transform.rotation);
                    }
                    break;
                case MarioMovement.PowerupStates.BiggerThanSuper:
                    InstantiateNewPlayer(gameObject, _superMarioPrefab, transform.position, transform.rotation);
                    break;
                default:
                    break;
            }
    }

    void InstantiateNewPlayer(GameObject previousPlayer, GameObject powereddownPlayer, Vector3 position, Quaternion rotation)
    {
        //instantiate new player and destroy the previous player,and move the variables to the new one
        GameObject newPlayerInstance = Instantiate(powereddownPlayer, position, rotation);
        MarioMovement previousMovementScript = previousPlayer.GetComponent<MarioMovement>();
        MarioMovement newMovementScript = newPlayerInstance.GetComponent<MarioMovement>();

        newMovementScript._isSpawnedByPowerup = true;
        newMovementScript._movingDirection = previousMovementScript._movingDirection;
        newMovementScript._movingSpeed = previousMovementScript._movingSpeed;
        newMovementScript._currentSpeedModifier = previousMovementScript._currentSpeedModifier;
        newMovementScript._movementState = previousMovementScript._movementState;
        newMovementScript._groundedState = previousMovementScript._groundedState;
        //without doing this mario will immediately fall after hitting enemy in air
        Rigidbody2D previousRigidbody2D = previousPlayer.GetComponent<Rigidbody2D>();
        Rigidbody2D newRigidbody2D = newPlayerInstance.GetComponent<Rigidbody2D>();
        newRigidbody2D.velocity = previousRigidbody2D.velocity;

        Destroy(previousPlayer);

        //starts the powerup animation coroutine
        newMovementScript.StartACoroutineLocally(newMovementScript.PowerMarioDown());
        //change virtual camera's target to the new instantiated player
        CinemachineVirtualCamera activeCamera = FindObjectOfType<CinemachineVirtualCamera>();
        activeCamera.m_Follow = newPlayerInstance.transform;
        //play sound
        _marioAudioSource = newPlayerInstance.GetComponentInChildren<AudioSource>();
        _marioAudioSource.PlayOneShot(_powerdownSfx, 1);
    }

    void KillPlayer()
    {
        Instantiate(_deadPlayerPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
