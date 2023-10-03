using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PowerupController : BasicGoombaMovement
{
    //enums
    public enum PowerupType { RedMushroom, Enemy, FireFlower }
    public enum PowerupMovement { Static, Goomba }

    [SerializeField] float _spawnForce = 100;
    [SerializeField] float _spawnTime = 1;
    [SerializeField] GameObject _poweredupPlayer;
    [SerializeField] private PowerupMovement _powerupMovement = PowerupMovement.Static;
    public PowerupType _powerupType;
    private bool _wasAlreadyDamaged = false;

    public AudioClip _powerupSfx;
    private AudioSource _marioAudioSource;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(Spawn());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MarioMovement _marioMovementScript = collision.transform.parent.gameObject.GetComponent<MarioMovement>();
            GameObject _mariosParent = collision.transform.parent.gameObject;

            switch (_marioMovementScript._powerupState)
            {
                case MarioMovement.PowerupStates.Small:
                    {
                      //spawn it a little above small mario because super mario has a different size
                      InstantiateNewPlayer(_mariosParent, _poweredupPlayer, new Vector2 (_mariosParent.transform.position.x, _mariosParent.transform.position.y + 0.5f), _mariosParent.transform.rotation);
                      Destroy(gameObject); 
                    }
                    break;
                case MarioMovement.PowerupStates.Super:
                    {
                        InstantiateNewPlayer(_mariosParent, _poweredupPlayer,_mariosParent.transform.position, _mariosParent.transform.rotation);
                        Destroy(gameObject);
                    }
                    break;
                case MarioMovement.PowerupStates.BiggerThanSuper:
                    {
                        //this if is here to avoid downgrading mario
                        if (_powerupType != PowerupType.RedMushroom)
                        {
                            InstantiateNewPlayer(_mariosParent, _poweredupPlayer, _mariosParent.transform.position, _mariosParent.transform.rotation);
                            Destroy(gameObject);
                        }
                    }
                    break;
                default:
                    break;
            }
            
        }
    }

    void InstantiateNewPlayer(GameObject previousPlayer, GameObject poweredupPlayer, Vector3 position, Quaternion rotation)
    {
        if (!_wasAlreadyDamaged)
        {
            //need this bool, otherwise sometimes 2 players can spawn if u hit 2 mushrooms at the same time for example
            _wasAlreadyDamaged = true;
            //instantiate new player and destroy the previous player,and move the variables to the new one
            GameObject newPlayerInstance = Instantiate(poweredupPlayer, position, rotation);
            MarioMovement previousMovementScript = previousPlayer.GetComponent<MarioMovement>();
            MarioMovement newMovementScript = newPlayerInstance.GetComponent<MarioMovement>();

            newMovementScript._isSpawnedByPowerup = true;
            newMovementScript._movingDirection = previousMovementScript._movingDirection;
            newMovementScript._movingSpeed = previousMovementScript._movingSpeed;
            newMovementScript._currentSpeedModifier = previousMovementScript._currentSpeedModifier;
            newMovementScript._movementState = previousMovementScript._movementState;
            newMovementScript._groundedState = previousMovementScript._groundedState;

            //if the powerup can move (like a mushroom) also set the y speed after the previous player, because its possible the player will catch it in air.
            if (_powerupMovement != PowerupMovement.Static)
            {
                Rigidbody2D previousRigidbody2D = previousPlayer.GetComponent<Rigidbody2D>();
                Rigidbody2D newRigidbody2D = newPlayerInstance.GetComponent<Rigidbody2D>();

                newRigidbody2D.velocity = previousRigidbody2D.velocity;
            }

            Destroy(previousPlayer);

            //starts the powerup animation coroutine
            newMovementScript.StartACoroutineLocally(newMovementScript.PowerMarioUp());
            //change virtual camera's target to the new instantiated player
            CinemachineVirtualCamera activeCamera = FindObjectOfType<CinemachineVirtualCamera>();
            activeCamera.m_Follow = newPlayerInstance.transform;
            //play sound
            _marioAudioSource = newPlayerInstance.GetComponentInChildren<AudioSource>();
            _marioAudioSource.PlayOneShot(_powerupSfx, 1);
        }
       
    }

    IEnumerator Spawn()
    {
        _rigidbody2D.AddForce(Vector2.up * _spawnForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(_spawnTime);

        if (_powerupMovement == PowerupMovement.Goomba)
            StartCoroutine(MovePowerup());

        yield break;
    }

    IEnumerator MovePowerup()
    {
        while (true)
        {
            base.SimpleMove();
            yield return null;
        }
    }

}
