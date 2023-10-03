using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioAnimationController : MonoBehaviour
{

    [SerializeField] float _defaultAnimationSpeed = 2;
    [SerializeField] float _runningAnimationSpeed = 3;
    [SerializeField] float _currentAnimationSpeed;
    [SerializeField] string _idleAnimationNameInAnimator = "mario_small_idle";
    [SerializeField] string _movingAnimationNameInAnimator = "mario_small_walking";
    [SerializeField] string _jumpingAnimationNameInAnimator = "mario_small_jumping";
    [SerializeField] string _powerupAnimationNameInAnimator = "mario_super_powerup";

    MarioMovement _marioMovementScript;
    Animator _animator;
    MarioMovement.MovementStates _movementState;
    MarioMovement.GroundedStates _groundedState;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _marioMovementScript = GetComponent<MarioMovement>();
    }

    void Update()
    {
        //Be aware that your animation name should be similar to "Layer_Name"."State_Name" but if you use a "sub-state machine" in mechanim then
        //the animation name must be "Sub-StateMachine_Name"."State_Name".
        _movementState = _marioMovementScript._movementState;
        _groundedState = _marioMovementScript._groundedState;

        //powerup animation should be checked firt
        if (_marioMovementScript._isPoweringUp)
            ChangeAnimation(_powerupAnimationNameInAnimator, _defaultAnimationSpeed);

        else if (_groundedState == MarioMovement.GroundedStates.Grounded)
        {
            //running
            if (_movementState == MarioMovement.MovementStates.Running)
                ChangeAnimation(_movingAnimationNameInAnimator, _runningAnimationSpeed);

            //walking
            else if (_movementState == MarioMovement.MovementStates.Walking)
                ChangeAnimation(_movingAnimationNameInAnimator, _defaultAnimationSpeed);

            //idle
            else if (_movementState == MarioMovement.MovementStates.Idle)
                ChangeAnimation(_idleAnimationNameInAnimator, 0);
        }

        //jump (in air)
        else if (_groundedState == MarioMovement.GroundedStates.InAir)
            ChangeAnimation(_jumpingAnimationNameInAnimator, 0);

    }

    private void ChangeAnimation(string animationName, float animationSpeed)
    {
        //if the animation isnt already playing, the animation is played.
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            _animator.Play(animationName);

        //set the animation speed if its already not set
        if(_currentAnimationSpeed != animationSpeed)
        {
            _currentAnimationSpeed = animationSpeed;
            _animator.speed = _currentAnimationSpeed;
        }
    }
}
