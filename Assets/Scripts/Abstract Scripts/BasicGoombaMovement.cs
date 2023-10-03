using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicGoombaMovement : FlippingMethods
{

    protected Rigidbody2D _rigidbody2D;
    protected BoxCollider2D _enemyCollider;

    [SerializeField] protected float _maxMovingSpeed = 4.0f;
    [SerializeField] protected float _raycastCollisionLength = 0.15f;
    [SerializeField] protected float _turningAcceleration = 10.0f;
    [SerializeField] protected LayerMask _staticLayerMask;
    [SerializeField] protected float _currentMovingSpeed; // dont change, visible in inspector just for debug

    protected override void Awake()
    {
        base.Awake();

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _enemyCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Start()
    {
        _currentMovingSpeed = _maxMovingSpeed;
    }


    //simple movement like Goomba enemies.
    protected virtual void SimpleMove()
    {
        _rigidbody2D.velocity = new Vector2(_currentMovingSpeed * _movingDirection, _rigidbody2D.velocity.y);

        if ((_movingDirection > 0 && CheckBoxCast(Vector2.right, _staticLayerMask)) || (_movingDirection < 0 && CheckBoxCast(Vector2.left, _staticLayerMask)))
        {
            Flip();
            StartCoroutine(Turn());
        }
    }

    //slowly build up speed from 0 to maxspeed when turning after hitting an object
    protected virtual IEnumerator Turn()
    {
        _currentMovingSpeed = 0;

        while (_currentMovingSpeed < _maxMovingSpeed)
        {
            //non-linear acceleration
            _currentMovingSpeed += Mathf.Pow(_turningAcceleration * Time.deltaTime, 2);
            //_rigidbody2D.velocity = new Vector2(_currentMovingSpeed * _movingDirection, _rigidbody2D.velocity.y);
            yield return null;
        }
        _currentMovingSpeed = _maxMovingSpeed;
        yield break;
    }

    protected virtual bool CheckBoxCast(Vector2 direction, LayerMask layerToCheck)
    {
        Vector3 colliderCenter = _enemyCollider.transform.position;
        return Physics2D.BoxCast(colliderCenter, _enemyCollider.bounds.size, 0f, direction, _raycastCollisionLength, layerToCheck);
    }

}
