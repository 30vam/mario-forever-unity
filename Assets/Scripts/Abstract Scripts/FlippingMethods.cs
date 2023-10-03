using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlippingMethods : MonoBehaviour
{
    [Range(-1, 1)]
    public int _movingDirection = 1; //should only be 1 and -1
    [SerializeField] protected float _flippingRate = 5.0f;
    protected Transform _animationTransform;

    protected virtual void Awake()
    {
        _animationTransform = GetComponentInChildren<SpriteRenderer>().gameObject.transform;
    }

    //starts flipping animation
    protected virtual void Flip()
    {
        //animation flips when enemy hits a wall or other enemy
        _movingDirection *= -1;

        //simple one
        if (_flippingRate == 0)
        {
            Vector3 theScale = _animationTransform.localScale;
            theScale.x *= -1;

            //if flipping rate is 0, turn immediately.
            _animationTransform.localScale = theScale;
        }
        //complicated one if _flippingRate != 0.
        else
            StartCoroutine(FlippingAnimation());
    }

    //"fancy" flipping animation
    protected virtual IEnumerator FlippingAnimation()
    {
        Vector3 currentScale = _animationTransform.localScale;
        float directionalFlippingRate = _flippingRate * -Mathf.Sign(currentScale.x);

        while (currentScale.x >= -1 && currentScale.x <= 1)
        {
            currentScale.x += directionalFlippingRate * Time.deltaTime;
            _animationTransform.localScale = currentScale;
            yield return null;
        }

        //set the scale to exactly 1 or -1 if unity calculates them abit too exact.the 0.1f is there to avoid changing the foot collider size
        if (currentScale.x <= -1 || currentScale.x >= 1)
        {
            ResetAnimationScale();
            yield break;
        }
    }

    //resets animation CHILD objects scale to 1, 1, 1 or -1, 1, 1 depending on mario's looking direction
    protected virtual void ResetAnimationScale()
    {
        if (_movingDirection > 0)
            _animationTransform.localScale = new Vector3(1, 1, 1);

        else if (_movingDirection < 0)
            _animationTransform.localScale = new Vector3(-1, 1, 1);
    }
}
