using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BasicGoombaMovement
{
    //STOMPING SCRIPT IS ATTACHED TO MARIO

    //enums
    public enum EnemyType { Goomba, Koopa, Spiny };

    //hidden variables
    GameObject _activatorObject;
    GameObject _collisionObject;
    GameObject _animationObject;
    GameObject _theBlockBelow;
    Animator _animator;
    


    //Inspector variables
    public EnemyType _enemyType = EnemyType.Goomba;

    [SerializeField] string _walkingAnimationName;
    [SerializeField] GameObject _deadEnemyObject;
    [SerializeField] GameObject _deadEnemyObjectByFireball;
    [SerializeField] LayerMask _playerLayerMask;
   
    protected override void Awake()
    {
        _activatorObject = transform.Find("TriggerCollider").gameObject;
        _collisionObject = transform.Find("PhysicsCollider").gameObject;
        _animationObject = transform.Find("Animation").gameObject;

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = _animationObject.GetComponent<Animator>();
        _enemyCollider = _collisionObject.GetComponent<BoxCollider2D>();
        _animationTransform = _animationObject.GetComponent<Transform>();
    }

    protected override void Start()
    {
        //disable almost all components at start (besides collider because that one is needed to activate the object again)
        gameObject.GetComponent<MonoBehaviour>().enabled = false;
        _rigidbody2D.Sleep();
        _activatorObject.SetActive(true);
        _animationObject.SetActive(false);

        _currentMovingSpeed = _maxMovingSpeed;
        //StartCoroutine(DamageEnemyAfterGettingBumped());
    }

    private void OnEnable()
    {
        HittingBlocks.onBlockBump += DamageEnemyAfterGettingBumped;
    }

    private void OnDisable()
    {
        HittingBlocks.onBlockBump -= DamageEnemyAfterGettingBumped;
    }

    void Update()
    {
        switch (_enemyType)
        {
            case EnemyType.Goomba:
            case EnemyType.Koopa:
                { SimpleMove(); PlayAnimation(_walkingAnimationName, _currentMovingSpeed / 2);}
                break;
            case EnemyType.Spiny:
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactive Block Trigger"))
            _theBlockBelow = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactive Block Trigger"))
            _theBlockBelow = null;
    }

    private void DamageEnemyAfterGettingBumped(GameObject theEventBlock)
    {
        if (_theBlockBelow == theEventBlock)
            DamageEnemy(true);
    }

    public void DamageEnemy(bool deathByFireball = false)
    {
        switch (_enemyType)
        {
            case EnemyType.Goomba:
            case EnemyType.Koopa:
                {
                    if (!deathByFireball)
                        Instantiate(_deadEnemyObject, transform.position, transform.rotation);
                    else
                        Instantiate(_deadEnemyObjectByFireball, transform.position, transform.rotation);
                    Destroy(gameObject);
                }
                break;
            case EnemyType.Spiny:
                break;
            default:
                break;
        }

    }

    private void PlayAnimation(string animationName, float animationSpeed)
    {
        //if the animation isnt already playing, the animation is played.
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            _animator.Play(animationName);

        _animator.speed = animationSpeed;
    }
}
