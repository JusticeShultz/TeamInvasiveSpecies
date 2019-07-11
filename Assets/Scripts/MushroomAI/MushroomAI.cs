using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MushroomAI : MonoBehaviour
{
    public float health = 10f;
    public float ChaseDistance = 10.0f;
    public float ExplodeDistance = 2.0f;
    public float SelfKnockbackScale = 8.0f;
    public GameObject poisonCloud;
    public GameObject renderModel;
    public NavMeshAgent navAgent;
    public Animator animator;
    public Rigidbody _rigidbody;
    [ReadOnlyField] public bool Exploded = false;

    void Update()
    {
        if (!PlayerController.Player) return;

        if (Vector3.Distance(PlayerController.Player.transform.position, transform.position) < ExplodeDistance && !Exploded)
        {
            Exploded = true;
            StartCoroutine(Explode());
        }

        if (Vector3.Distance(PlayerController.Player.transform.position, transform.position) < ChaseDistance)
        {
            animator.SetBool("Moving", true);
            if (NavMeshValid()) {
                navAgent.isStopped = false;
                navAgent.destination = PlayerController.Player.transform.position;
            }
        }
        else
        {
            if (NavMeshValid()) {
                navAgent.destination = transform.position;
                navAgent.isStopped = true;
            }
            animator.SetBool("Moving", false);
        }
    }

    public void Damage(float damage, Vector3 direction)
    {
        BloodPool.Splatter(transform.position + direction, Mathf.FloorToInt(damage), BloodPool.BloodColor.Red);

        health -= damage;

        if (health <= 0)
        {
            navAgent.enabled = false;
            _rigidbody.isKinematic = false;
            direction.y = 0;
            _rigidbody.AddForce(direction * -SelfKnockbackScale, ForceMode.Impulse);
            StartCoroutine(delayedDeath());
        }
    }

    IEnumerator delayedDeath()
    {
        yield return new WaitForSeconds(1);

        foreach (Transform cc in transform)
        {
            if (cc.GetComponent<BloodModule>() != null)
            {
                cc.parent = null;
                cc.gameObject.SetActive(false);
            }
        }

        Destroy(gameObject);
    }

    IEnumerator Explode()
    {
        animator.SetBool("Moving", false);
        animator.SetTrigger("Explode");
        navAgent.enabled = false;
        yield return new WaitForSeconds(1.1f);
        GetComponent<BoxCollider>().enabled = false;
        _rigidbody.isKinematic = true;
        poisonCloud.SetActive(true);
        renderModel.SetActive(false);
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
    private bool NavMeshValid()
    {
        return navAgent.isOnNavMesh && navAgent.enabled;
    }
}