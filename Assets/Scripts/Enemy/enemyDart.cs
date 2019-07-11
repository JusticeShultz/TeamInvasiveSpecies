using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDart : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float damage = 1;
    bool hasHitThing = false;
    void Update()
    {
        Vector3 oldPos = transform.position;
        transform.position += transform.forward * speed * Time.deltaTime;
        if (!hasHitThing)
        {
            Debug.DrawLine(oldPos,transform.position, Color.blue, 10);
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("RangedEnemy") || collision.CompareTag("Enemy")) return;
        hasHitThing = true;
        if (MenuStart.GameStarted) {
            if (collision.transform == PlayerController.Player.transform)
            {
                PlayerController.Player_Controller.TakeDamage(damage, collision.transform.position);
            }
        }
        Debug.DrawRay(transform.position, Vector3.up, Color.red, 10);
        Destroy(gameObject);
    }
}
