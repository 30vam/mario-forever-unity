using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeoutObject : MonoBehaviour
{
    [SerializeField] float _fadingAcceleration = 0.1f;

    SpriteRenderer _spriteRenderer;
    float _alpha;
    float _fadingRate;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        _alpha = 1.0f;
        _fadingRate = 0;
    }

    void Update()
    {
        if (_spriteRenderer.color.a > 0)
        {
            _fadingRate += _fadingAcceleration;
            _alpha -= _fadingRate * Time.deltaTime;
            _spriteRenderer.color = new Color (_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _alpha);
        }

        else if (_spriteRenderer.color.a <= 0)
            Destroy(gameObject);
    }
   
}
