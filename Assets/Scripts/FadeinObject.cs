using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeinObject : MonoBehaviour
{
    [SerializeField] float _fadingAcceleration = 0.1f;
    [SerializeField] float _startingAlpha = 0.2f;

    SpriteRenderer _spriteRenderer;
    float _alpha;
    float _fadingRate;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _startingAlpha);
    }

    void Start()
    {
        _alpha = 0;
        _fadingRate = 0;
    }

    void Update()
    {
        if (_spriteRenderer.color.a >= 0)
        {
            _fadingRate += _fadingAcceleration;
            _alpha += _fadingRate * Time.deltaTime;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _alpha);
        }

        else if (_spriteRenderer.color.a >= 1)
            Destroy(this);
    }

}
