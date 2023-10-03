using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompingEnemies : MonoBehaviour
{
    [SerializeField] float _raycastCollisionLength = 0.3f;
    [SerializeField] float _smallJumpForce = 500;
    [SerializeField] float _continuosJumpForce = 25000;
    [SerializeField] AudioClip _stompingSound;
    [SerializeField] LayerMask _enemyLayerMask;

    MarioMovement _marioMovementScript;
    CapsuleCollider2D _bodyCollider;
    Rigidbody2D _rigidbody2D;
    AudioSource _audioSource;

    RaycastHit2D _raycastDetectedHit;

    private void Awake()
    {
        _marioMovementScript = GetComponent<MarioMovement>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _bodyCollider = GetComponentInChildren<CapsuleCollider2D>();
        _audioSource = GetComponentInChildren<AudioSource>();
    }
  
    void Update()
    {
        //if mario is in air:
        if (_marioMovementScript._groundedState != MarioMovement.GroundedStates.Grounded)
        {

            //if any enemy was hit by raycast then:
            if (DetectStomp() && !_marioMovementScript._isPoweringDown)
            {
                EnemyController enemyControllerScript = _raycastDetectedHit.collider.gameObject.GetComponentInParent<EnemyController>();

                switch (enemyControllerScript._enemyType)
                {
                    case EnemyController.EnemyType.Goomba:
                    case EnemyController.EnemyType.Koopa:
                        { Stomp(); enemyControllerScript.DamageEnemy(false); }
                        break;
                    case EnemyController.EnemyType.Spiny:
                        break;
                    default:
                        break;  
                }
            }
        }
    }
    private bool DetectStomp()
    {
        Vector3 colliderCenter = _bodyCollider.transform.position;
        //check for collision with any enemy below mario's feet
        return _raycastDetectedHit = Physics2D.BoxCast(colliderCenter, _bodyCollider.bounds.size, 0f, Vector2.down, _raycastCollisionLength, _enemyLayerMask);
    }


    private void Stomp()
    {
        _audioSource.PlayOneShot(_stompingSound, 1.0f); //play stomping sound
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);//reset Y velocity
        _rigidbody2D.AddForce(new Vector2(0, _smallJumpForce), ForceMode2D.Impulse); //do a small jump after stomping

        //do big jump if jump button is held when stomping
        if (Input.GetButton("Jump"))
             StartCoroutine(_marioMovementScript.CalculateLongJump(_continuosJumpForce));
    }

    //Long jump after stomping enemy (UNUSED)
    /*IEnumerator WaitForJumpPress(float time)
    {
        while (time > 0)
        {
            //finish the loop if the jump button is not pressed in a certain time
            time -= Time.deltaTime;
            
            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine(_marioMovementScript.CalculateLongJump(_continuosJumpForce));
                yield break;
            }
            else
                yield return null;
        }
        yield break;
    }*/
   
}
