using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBalloonModule : MonoBehaviour
{
    public GameObject BombObject;
    public GameObject Smoke;

    public Vector3 SpawnOffset = new Vector3(0, 100, 0);
    public Rigidbody _rigidbody;

    bool Collided = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!Collided)
        {
            if (transform.tag != "Enemy" && transform.tag != "Snail" && transform.tag != "Mushroom" && transform.tag != "Tree" && transform.tag != "RangedEnemy")
            {
                transform.parent = collision.transform;
                _rigidbody.isKinematic = true;
                Collided = true;
                StartCoroutine(bombCall());
            }
        }
    }

    IEnumerator bombCall()
    {
        Smoke.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        Instantiate(BombObject, transform.position + SpawnOffset, Quaternion.identity);
        Smoke.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
