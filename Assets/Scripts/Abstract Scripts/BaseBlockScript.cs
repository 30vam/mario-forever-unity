using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBlockScript : MonoBehaviour
{
    public enum BlockStates { Empty, Idle, IsBeingHit };
    public enum BlockTypes { QuestionMark, BrownBrick, MetalBrick };

    public BlockStates _blockState;
    public BlockTypes _blockType;

    [SerializeField] protected string _idleAnimationName;
    [SerializeField] protected string _poppingAnimationName;
    [SerializeField] protected string _emptyAnimationName;
    [SerializeField] protected float _animationSpeed = 1.0f;
    [SerializeField] protected AudioClip _poppingSfx;
    protected float _currentAnimationSpeed;

    protected Animator _animator;
    protected AudioSource _audioSource;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _blockState = BlockStates.Idle;
        _currentAnimationSpeed = 1.0f;
        ChangeAnimation(_idleAnimationName, _animationSpeed);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        switch (_blockState)
        {
            case BlockStates.Empty:
                ChangeAnimation(_emptyAnimationName, _animationSpeed);
                break;
            case BlockStates.Idle:
                ChangeAnimation(_idleAnimationName, _animationSpeed);
                break;
            case BlockStates.IsBeingHit:
                ChangeAnimation(_poppingAnimationName, _animationSpeed);
                break;
            default:
                break;
        }
    }

    //is called when mario's head hits the block from under
    public virtual void PopTheBlock()
    {
        if (_blockState == BlockStates.Idle)
        {
            _audioSource.PlayOneShot(_poppingSfx);
            _blockState = BlockStates.IsBeingHit;
        }
    }

    //how to break the BRICKS (only happens if mario is not small)
    public virtual void BreakTheBlock()
    {
        Destroy(gameObject);
    }


    //uses animation event to change _blockState (which determines the objects current animation)
    protected virtual void FinishIsBeingHitState()
    {
         _blockState = BlockStates.Idle;
    }

   
    protected virtual void ChangeAnimation(string animationName, float animationSpeed)
    {
        //if the animation isnt already playing, the animation is played.
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            _animator.Play(animationName);

        //set the animation speed if its already not set
        if (_currentAnimationSpeed != animationSpeed)
        {
            _currentAnimationSpeed = animationSpeed;
            _animator.speed = _currentAnimationSpeed;
        }
    }
}
