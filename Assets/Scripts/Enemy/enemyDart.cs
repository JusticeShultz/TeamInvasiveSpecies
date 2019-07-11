using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDart : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float damage = 1;
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

    }
    void OnTriggerEnter(Collider collision)
    {
        if (MenuStart.GameStarted) {
            print("why no work");
            if (collision.transform == PlayerController.Player.transform)
        {
            PlayerController.Player_Controller.TakeDamage(damage, collision.transform.position);
        }
        }
        
        Destroy(gameObject);
    }
}
