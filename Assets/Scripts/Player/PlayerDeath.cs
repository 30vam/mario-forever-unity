using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    AudioSource _audioSource;
    Rigidbody2D _rigidbody2D;
    GameObject[] _musicPlayerObjects;

    [SerializeField] float _deathForce = 700;
    [SerializeField] float _animationDuration = 3.0f;
    [SerializeField] float _deathSfxVolume = 0.5f;

    private void Awake()
    {
         _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _musicPlayerObjects = GameObject.FindGameObjectsWithTag("Music");
    }
    
    void Start()
    {
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        foreach (GameObject musicPlayerObject in _musicPlayerObjects) //stop bgm
            musicPlayerObject.SetActive(false);

        _audioSource.PlayOneShot(_audioSource.clip, _deathSfxVolume); //play death sfx and wait 1 sec
        yield return new WaitForSeconds(0.5f);

        //Start the animation:
        _rigidbody2D.simulated = true;
        _rigidbody2D.AddForce(new Vector2(0, _deathForce));
        yield return new WaitForSeconds(_animationDuration);

        //restart scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        yield break;
    }
}
