using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class SnailWander : MonoBehaviour
{
    public Rigidbody _Rigidbody;
    public float SelfKnockbackScale;
    public float wanderRadius;
    public float wanderTimer;
    public float health;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            if(NavMeshValid()) agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);



        return navHit.position;
    }

    public void Damage(float damage, Vector3 direction)
    {
        BloodPool.Splatter(transform.position + direction, Mathf.FloorToInt(damage), BloodPool.BloodColor.Green);

        health -= damage;

        if (health <= 0)
        {
            agent.enabled = false;
            _Rigidbody.isKinematic = false;
            direction.y = 0;
            _Rigidbody.AddForce(direction * -SelfKnockbackScale, ForceMode.Impulse);
            StartCoroutine(delayedDeath());
        }
    }

    IEnumerator delayedDeath()
    {

        yield return new WaitForSeconds(1);

        foreach (Transform cc in transform)
        {
            if (cc.name != "pSphere1" && cc.name != "pSphere2" && cc.name != "pSphere3")
                cc.parent = null;
        }

        Destroy(gameObject);
    }
    private bool NavMeshValid()
    {
        return agent.isOnNavMesh && agent.enabled;
    }
}