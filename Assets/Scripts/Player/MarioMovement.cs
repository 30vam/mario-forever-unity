using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioMovement : FlippingMethods
{
    //Inspector variables
    [SerializeField] public float _maxWalkingSpeed = 5.0f;
    [SerializeField] public float _accelerationAmount = 15.0f;
    [SerializeField] float _deAccelerationAmount = 50.0f;
    [SerializeField] public float _runningSpeedModifier = 3.0f;
    [SerializeField] internal float _jumpForce = 30000.0f;
    [SerializeField] float _jumpForceDecayRate = 2000.0f;
    [SerializeField] float _maxFallingSpeed = 16;
    [SerializeField] float _maxJumpingSpeed = 13;
    [SerializeField] float _powerupDuration = 1.0f;
    [SerializeField] float _powerdownInvincibilityDuration = 3.0f;
    [SerializeField] float _fadingRate = 0.1f;
    [SerializeField] internal float _raycastCollisionLength = 0.25f;
    [SerializeField] bool _stopsWhenRunningIntoWall = true;
    [SerializeField] LayerMask _groundLayerMask;
    private LayerMask _enemyPhysicsLayer;
    

    //Enums/States
    public enum PowerupStates { Small, Super, BiggerThanSuper };
    public enum MovementStates { Idle, Walking, Running };
    public enum GroundedStates { Grounded, InAir };

    public bool _isPoweringUp = false;
    public bool _isPoweringDown = false;
    public bool _isSpawnedByPowerup = false;

    // variables that shouldnt be changed, probably, only visible in inspector to check for bugs
    float _horizontalInput;
    public float _movingSpeed;
    public float _currentSpeedModifier;
    
    public MovementStates _movementState;
    public GroundedStates _groundedState;
    public PowerupStates _powerupState;

    //Components
    Rigidbody2D _rigidbody2D;
    CapsuleCollider2D _bodyCollider;
    AudioSource _audioSource;
    SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _bodyCollider = transform.Find("PhysicsCollider").GetComponent<CapsuleCollider2D>();
        _audioSource = GetComponentInChildren<AudioSource>();
        _animationTransform = GetComponentInChildren<Animator>().gameObject.transform;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _enemyPhysicsLayer = LayerMask.NameToLayer("EnemyPhysics");
    }

    void Start()
    {
        if (!_isSpawnedByPowerup)
        {
            _movementState = MovementStates.Idle;
            _groundedState = GroundedStates.Grounded;

            _movingSpeed = 0.0f;
            _currentSpeedModifier = 1.0f;
        }

        ResetAnimationScale();//flips the player if we want the player to start by looking into the left direction (it has a boolean)
    }

    void Update()
    {
        MovementStateCheck(); //Idle, Walking, Running
        
        GroundedStateCheck(); //Grounded, InAir

        //movement speed in each movement state
        switch (_movementState)
        {
            case MovementStates.Idle:
                Move(0, 0, 0); StartJump(true, _jumpForce);
                break;
            //the difference between walking and running state is in animation speed (see MarioAnimationController)
            case MovementStates.Walking:
            case MovementStates.Running:
                Move(_accelerationAmount, _deAccelerationAmount, _maxWalkingSpeed); StartJump(true, _jumpForce);
                break;
            default:
                break;
        }

        if (_groundedState == GroundedStates.Grounded && _rigidbody2D.velocity.y != 0)
        {
            //reset Y velocity to 0 if on ground
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        }

        else if (_groundedState == GroundedStates.InAir)
        {
            //mario's max speed in air when falling and jumping
            if (_rigidbody2D.velocity.y > _maxJumpingSpeed)
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _maxJumpingSpeed);
            else if (_rigidbody2D.velocity.y < -_maxFallingSpeed)
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -_maxFallingSpeed);
        }
    }

    private void Move(float acceleration, float deAcceleration, float maxSpeed)
    {
        //moving and acceleration (make a seperate function for them)
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        //hold X to be able to run(a.k.a change _runningSpeedModifier)
        if (Input.GetButton("Fire1"))
        {
            if (_currentSpeedModifier == 1 && !isRunningIntoWall())
                _currentSpeedModifier = _runningSpeedModifier;
        }

        else if (_currentSpeedModifier != 1 && (_groundedState == GroundedStates.Grounded || (_groundedState == GroundedStates.InAir && isRunningIntoWall())))
            _currentSpeedModifier = 1;

        //lower speed to walking speed if player is running into walls
        if (isRunningIntoWall())
        {
            //cant run when falling and running into wall, but can achive max walking speed
            if (_rigidbody2D.velocity.y < 0)
                _currentSpeedModifier = 1;
            //when on ground or jumping and running into walls, moving speed is (almost) 0
            else
                _movingSpeed = 0;
        }

        if (acceleration != 0)
        {
            ManageAcceleration(acceleration, deAcceleration, maxSpeed);

            //always sets the velocity to current calculated speed
            _rigidbody2D.velocity = new Vector2(_movingSpeed, _rigidbody2D.velocity.y);
        }

        else
            _rigidbody2D.velocity = new Vector2(_horizontalInput * maxSpeed, _rigidbody2D.velocity.y);

        //flip the character according to its direction of speed
        //if (_groundedState == GroundedStates.Grounded && _rigidbody2D.velocity.y >)
            //Flip();

        //flip the character according to its direction of speed
        if ((_movingSpeed < 0 && _movingDirection > 0) || (_movingSpeed > 0 && _movingDirection < 0))
            Flip();
    }

    private void ManageAcceleration(float acceleration, float deAcceleration, float maxSpeed)
    {
        //accelerating to the right AND deaccelerating when the run button is no longer held while moving (the currentspeed will be bigger
        //than maxspeed without this second check)
        if (_horizontalInput > 0)
        {
            if (_movingSpeed < maxSpeed * _currentSpeedModifier && _movingSpeed >= 0)
                _movingSpeed += acceleration * Time.deltaTime;
            else if (_movingSpeed < 0) //when mario is moving left but the input is pressing right, use deacceleration not acceleration
                _movingSpeed += deAcceleration * Time.deltaTime;
            else if (_movingSpeed > maxSpeed * _currentSpeedModifier && _currentSpeedModifier == 1)  //check if the player is because otherwise it will change the speed in small amounts even when not needed
                _movingSpeed -= deAcceleration * Time.deltaTime;
        }
        //same thing but for left direction
        else if (_horizontalInput < 0)
        {
            if (_movingSpeed > -maxSpeed * _currentSpeedModifier && _movingSpeed <= 0)
                _movingSpeed -= acceleration * Time.deltaTime;
            else if (_movingSpeed > 0)
                _movingSpeed -= deAcceleration * Time.deltaTime;
            else if (_movingSpeed < -maxSpeed * _currentSpeedModifier && _currentSpeedModifier == 1)
                _movingSpeed += deAcceleration * Time.deltaTime;
        }

        //deaccelerate when there is NO input
        else if (_horizontalInput == 0)
        {
            if (_movingSpeed > 1)
                _movingSpeed -= deAcceleration * Time.deltaTime;
            else if (_movingSpeed < 1)
                _movingSpeed += deAcceleration * Time.deltaTime;
            if (_horizontalInput == 0 && (_movingSpeed < 1 && _movingSpeed > -1 && _movingSpeed != 0))
                _movingSpeed = 0;
        }
    }

    public void StartJump(bool playSound, float jumpForce)
    {
        if (Input.GetButtonDown("Jump") && !isTouchingCeiling() && _groundedState == GroundedStates.Grounded)
        {
            //what happens when you press and/or hold the jump button
            //the initial jump when you tap the jump button
            _rigidbody2D.AddForce(new Vector2(0, jumpForce * Time.deltaTime));

            if (playSound)
                _audioSource.PlayOneShot(_audioSource.clip); //plays jump sound

            StartCoroutine(CalculateLongJump(jumpForce));
        }
    }

    public IEnumerator CalculateLongJump(float jumpForce)
    {
        float currentJumpForce = jumpForce;
        
        //applies a decreasing jumping force until the force runs out or until the button is hold/ceiling is hit.
        while (Input.GetButton("Jump") && currentJumpForce > 0)
        {
            if (isTouchingCeiling())
                yield break;
                
            else
            {
                _rigidbody2D.AddForce(new Vector2(0, currentJumpForce * Time.deltaTime));
                currentJumpForce -= _jumpForceDecayRate;

                yield return null;
            }
        }
    }

    private void MovementStateCheck()
    {
        //Movement state checks (should learn state machines and/or events later because everytime a new state is added you need a new condition for each)
        if (_horizontalInput != 0)
        {
            //running state conditions (the 0.5f is because unity doesnt calculate speed as the exact same amount of maxSpeed)
            if ((_movingSpeed > _maxWalkingSpeed + 0.5f || _movingSpeed < -_maxWalkingSpeed - 0.5f) && _movementState != MovementStates.Running)
                _movementState = MovementStates.Running;
            //walking state condition
            else if ((_movingSpeed < _maxWalkingSpeed  && _movingSpeed > -_maxWalkingSpeed) && _movementState != MovementStates.Walking)
                _movementState = MovementStates.Walking;
        }

        //idle state condition(switches only when all speeds (Y and X speed) are 0)
        else if (_rigidbody2D.velocity == new Vector2(0, 0) && _movementState != MovementStates.Idle)
            _movementState = MovementStates.Idle;
    }

    private void GroundedStateCheck()
    {
        //uses the simple IsTouchingLayers method, might change later if movement doesnt feel right
        if (isTouchingGround())
            _groundedState = GroundedStates.Grounded;

        //InAir state
        else if (_groundedState != GroundedStates.InAir)
            _groundedState = GroundedStates.InAir;
    }

    private bool isTouchingGround()
    {
        Vector3 colliderCenter = _bodyCollider.transform.position;
        return Physics2D.CapsuleCast(colliderCenter, _bodyCollider.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.down, _raycastCollisionLength, _groundLayerMask);
    }

    private bool isRunningIntoWall()
    {
        //check if player is running into wall on ground AND in air ONLY IF the player is falling
        if (_stopsWhenRunningIntoWall && (_groundedState == GroundedStates.Grounded || _groundedState == GroundedStates.InAir && _rigidbody2D.velocity.y < 0))
        {
            Vector3 colliderCenter = _bodyCollider.transform.position;
            int movingDirection = Math.Sign(_movingSpeed);

            return Physics2D.CapsuleCast(colliderCenter, _bodyCollider.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.right * movingDirection, _raycastCollisionLength, _groundLayerMask);
        }
        else
            return false;
    }

    private bool isTouchingCeiling()
    {
        Vector3 colliderCenter = _bodyCollider.transform.position;
        return Physics2D.CapsuleCast(colliderCenter, _bodyCollider.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.up, 0.1f, _groundLayerMask);
    }

    public void StartACoroutineLocally(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    //turns on the boolean that controlls powerup animation
    public IEnumerator PowerMarioUp()
    {
        _isPoweringUp = true;

        yield return new WaitForSecondsRealtime(_powerupDuration);
        _isPoweringUp = false;

        yield break;
    }

    public IEnumerator PowerMarioDown()
    {
        _isPoweringDown = true;
        float timer = _powerdownInvincibilityDuration;
        float alpha = 1.0f;
        int positiveOrNegative = -1; //used to determine wether sprite should fade IN or OUT

        while (_isPoweringDown)
        {
            timer -= Time.deltaTime;

            //flash sprite function:
            if (_spriteRenderer.color.a <= 0 && positiveOrNegative != 1)
                positiveOrNegative = 1;
            else if (_spriteRenderer.color.a >= 1 && positiveOrNegative != -1)
                positiveOrNegative = -1;

            alpha += _fadingRate * positiveOrNegative;

            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, alpha);
            //end the coroutine if timer has ended
            if (timer <= 0)
            {
                _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1);
                _isPoweringDown = false;
            }

            else
                yield return null;
        }

        yield break;
    }
}

