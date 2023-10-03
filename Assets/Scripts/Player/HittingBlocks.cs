using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittingBlocks : MonoBehaviour
{
    //CapsuleCollider2D _bodyCollider;
    Rigidbody2D _rigidbody2D;
    MarioMovement _marioMovementScript;
    AudioSource _audioSource;

    [SerializeField] private AudioClip _breakingSfx;

    public delegate void OnBlockBump(GameObject bumpedBlock);
    public static event OnBlockBump onBlockBump;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _marioMovementScript = GetComponent<MarioMovement>();
        _audioSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Interactive Block Trigger" && transform.position.y < collision.transform.position.y)
        {
            RaiseOnBlockBump(collision.gameObject);
            BaseBlockScript baseBlockScript = collision.transform.parent.GetComponent<BaseBlockScript>();
            if(_marioMovementScript._powerupState != MarioMovement.PowerupStates.Small && baseBlockScript._blockType == BaseBlockScript.BlockTypes.BrownBrick)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
                StopCoroutine(_marioMovementScript.CalculateLongJump(_marioMovementScript._jumpForce));
                _audioSource.PlayOneShot(_breakingSfx, 1);
                baseBlockScript.BreakTheBlock();
            }
            else
                baseBlockScript.PopTheBlock();

        }
    }

    //event that happens when mario bumps a block
    public void RaiseOnBlockBump(GameObject bumpedBlock)
    {
        if (onBlockBump != null)
            onBlockBump(bumpedBlock);
    }
}
