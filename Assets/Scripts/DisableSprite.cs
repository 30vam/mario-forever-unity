using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSprite : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
