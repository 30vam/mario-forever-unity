using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAndDeactivate : MonoBehaviour
{

    [SerializeField] private Camera _mainCamera;
    private Vector3 _position;

    private void Awake()
    {
        _position = (new Vector3(0.5f, 0.5f, 0));
    }

    void Update()
    {
        transform.position = _mainCamera.ViewportToWorldPoint(_position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //enables enemy after its not inside activator box collider
        if (collision.gameObject.CompareTag("EnemyActivator"))
        {
            GameObject parentObject = collision.transform.parent.gameObject; //find colliders parent object (main enemy object)
            parentObject.GetComponent<Rigidbody2D>().WakeUp(); //enable parent rigidbody
            parentObject.GetComponent<MonoBehaviour>().enabled = true; //enable parent scripts
            parentObject.transform.Find("Animation").gameObject.SetActive(true); //enable animation object

            Debug.Log("Activated", gameObject); //debug message to show which instance is activated

            //when ACTIVATING an enemy, make its moving direction towards mario (it's actually towards the activator zone not mario)
            EnemyController enemyControllerScript = parentObject.GetComponent<EnemyController>();
            enemyControllerScript._movingDirection = (int)Mathf.Sign(transform.position.x - parentObject.transform.position.x);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //disables enemy after its not inside activator box collider
        if (collision.gameObject.CompareTag("EnemyActivator"))
        {
            GameObject parentObject = collision.transform.parent.gameObject; //find colliders parent object
            parentObject.GetComponent<Rigidbody2D>().Sleep(); //disable parent rigidbody
            parentObject.GetComponent<MonoBehaviour>().enabled = false; //disable parent scripts
            parentObject.transform.Find("Animation").gameObject.SetActive(false);//disable animation child object

            Debug.Log("Deactivated", gameObject); //debug message to show which instance is deactivated
        }
    }
}
