using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutofBoundObjects : MonoBehaviour
{
    //destroy objects if they fall off the screen
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyActivator")
            Destroy(collision.gameObject.transform.parent.gameObject);
    }
}
