using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBlock : BaseBlockScript
{
    [SerializeField] GameObject _powerupPrefab;
    [SerializeField] int _maxPowerupSpawns = 1;

    private int _currentPowerupSpawns = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
    }

    //is called when mario's head hits the block from under
    public override void PopTheBlock()
    {
        //only spawn powerup if mario has hit the  block AND the question box isn't used more than maximum times.
        if (_currentPowerupSpawns < _maxPowerupSpawns  && _blockState == BlockStates.Idle)
        {
            _audioSource.PlayOneShot(_poppingSfx);
            _blockState = BlockStates.IsBeingHit;
            Vector2 powerupSpawnPosition = new Vector2(transform.position.x, transform.position.y + transform.localScale.y/2);
            Instantiate(_powerupPrefab, powerupSpawnPosition, transform.rotation);
            _currentPowerupSpawns++;
        }
            
    }

    protected override void FinishIsBeingHitState()
    {
        //if there are no more power ups/coins left, change sprite to empty question mark.
        if (_currentPowerupSpawns < _maxPowerupSpawns)
            _blockState = BlockStates.Idle;
        else
            _blockState = BlockStates.Empty;

    }
}
