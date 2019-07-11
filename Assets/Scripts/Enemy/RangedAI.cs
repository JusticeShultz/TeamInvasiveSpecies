using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RangedAI : MonoBehaviour
{
    //don't touch these unless your name is justice
    NavMeshAgent pf;
    Vector3 wanderPoint;
    float wanderTimer = 0;
    float AttackTimer = 0;
    Vector3 lastAttackPos;
    float health = 0;
    delegate void State();
    State stateUpdate;
    Rigidbody rb;
    Collider c;

    [SerializeField] PlayerController playerReference;
    //anyone can change these
    public Animator animator;
    [SerializeField] float attackTimerMax = 1;
    [SerializeField] float wanderTimerMax = 2.5f;
    [SerializeField] float attackRadius = 1;
    [SerializeField] float attackRange = 1;
    [SerializeField] float attackDamage = 1;
    [SerializeField] float attackKnockBack = 1;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float selfKockBackScale = 100;
    [SerializeField] float seekRadius = 2.5f;
    [SerializeField] float wanderVarience = 1;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject[] giblets;
    

    void Start()
    {
        health = maxHealth;
        pf = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        c = GetComponent<Collider>();
        //pf.updateRotation = false;
        //transform.rotation = Quaternion.LookRotation(pf.velocity);
        SwitchToHide();
        animator.SetBool("Moving", true);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!playerReference && MenuStart.GameStarted)
            playerReference = PlayerController.Player_Controller;

        if (health > 0 && MenuStart.GameStarted)
        {
            stateUpdate();
        }
    }
    public void Damage(float damage, Vector3 direction, bool IsLargeDeathCauser = false)
    {
        if (!IsLargeDeathCauser)
            BloodPool.Splatter(transform.position + direction, Mathf.FloorToInt(damage), BloodPool.BloodColor.Red);
        else BloodPool.Splatter(transform.position + direction, 4, BloodPool.BloodColor.Red);

        health -= damage;
        if (health <= 0)
        {
            pf.enabled = false;
            direction.y = 0;
            c.enabled = false;

            if (!IsLargeDeathCauser)
                foreach (GameObject go in giblets)
                    GibletSpawner.SpawnStack.Add(new GibletSpawner.SpawnedObject(go, transform.position, Quaternion.identity));

            foreach (Transform cc in transform)
            {
                if (cc.GetComponent<BloodModule>() != null)
                {
                    cc.gameObject.SetActive(false);
                    cc.parent = null;
                }
            }
            Destroy(gameObject);
        }

    }

    //WANDER
    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        WanderOutGoingTransitions();
        if (stateUpdate == Wander)
        {
            if (wanderTimer <= 0)
            {
                NavMeshPath path = new NavMeshPath();
                int i = 0;
                do
                {
                    var randomInCircle = Random.insideUnitCircle;
                    wanderPoint = ((new Vector3(randomInCircle.x, 0, randomInCircle.y)) * 5) + transform.position;
                    NavMesh.CalculatePath(transform.position, wanderPoint, NavMesh.AllAreas, path);
                    Debug.DrawLine(transform.position, wanderPoint, Color.red);
                    i++;
                } while (path.status != NavMeshPathStatus.PathComplete && i < 100);
                if (i >= 100)
                {
                    print("FAILED");
                }
                else
                {
                    wanderTimer = wanderTimerMax + Random.value * wanderVarience;
                }
                pf.SetPath(path);
            }
        }
    }
    void WanderOutGoingTransitions() //Designer Freindly //MUST BE CALLED IN WANDER
    {
        if (CanSeePlayer())
        {
            SwitchToRangedAttack();
            return;
        }
        TreeController closestTree = null;
        float closestTreeDistance = -1;
        foreach (var tree in TreeList.Trees) //might need to optimize
        {
            if (closestTreeDistance == -1 || Vector3.Distance(tree.transform.position, transform.position) < closestTreeDistance)
            {
                closestTree = tree;
                closestTreeDistance = Vector3.Distance(closestTree.transform.position, transform.position);
            }

        }
        if (closestTreeDistance >= 10 && wanderTimer<=0)
        {
            SwitchToGetToTree();
        }
    }
    void SwitchToWander()
    {
        stateUpdate = Wander;
        wanderTimer = 0.5f;
    }
    void Hide()
    {
        TreeController closestTree = null;
        float closestTreeDistance = -1;
        Vector3 dirAwayFromPlayer = (transform.position - playerReference.transform.position).normalized * 4;
        foreach(var tree in TreeList.Trees) //might need to optimize
        {
            if(closestTreeDistance == -1 || Vector3.Distance(tree.transform.position + dirAwayFromPlayer, transform.position) < closestTreeDistance)
            {
                closestTree = tree;
                closestTreeDistance = Vector3.Distance(closestTree.transform.position + dirAwayFromPlayer, transform.position);
            }

        }
        pf.SetDestination(closestTree.transform.position + dirAwayFromPlayer);
        Vector3 dirAwayFromTree = (transform.position - closestTree.transform.position).normalized;
        HideOutGoingTransitions(Vector3.Dot(dirAwayFromPlayer.normalized, dirAwayFromTree));
    }
    void HideOutGoingTransitions(float dot)
    {
        if(dot >= 0.9f)
        {
            SwitchToWander();
        }
    }
    void SwitchToHide()
    {
        stateUpdate = Hide;
    }
    void GetToTree()
    {
        TreeController closestTree = null;
        float closestTreeDistance = -1;
        foreach (var tree in TreeList.Trees) //might need to optimize
        {
            if (closestTreeDistance == -1 || Vector3.Distance(tree.transform.position, transform.position) < closestTreeDistance)
            {
                closestTree = tree;
                closestTreeDistance = Vector3.Distance(closestTree.transform.position, transform.position);
            }

        }
        pf.SetDestination(closestTree.transform.position);
        GetToTreeOutGoingTransitions(closestTreeDistance);
    }
    void GetToTreeOutGoingTransitions(float closestTreeDisntace)
    {
        if (closestTreeDisntace<=10)
        {
            SwitchToHide();
        }

    }
    void SwitchToGetToTree()
    {
        stateUpdate = GetToTree;
    }
    void RangedAttack()
    {
        
        pf.isStopped = true;
        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(playerReference.transform.position- transform.position),Time.deltaTime);
        AttackTimer -= Time.deltaTime;
        if (AttackTimer <= 0)
        {
            Instantiate(bullet,
                transform.position, 
                Quaternion.LookRotation((FirstOrderIntercept(transform.position, Vector3.zero, 60, playerReference.transform.position, playerReference.controllerRigidbody.velocity)) - transform.position)
                );
        }
        RangedAttackOutgoingTransitions();
        if (stateUpdate != RangedAttack)
        {
            pf.isStopped = false;
        }
        //Debug.draw WHY IS THEIR NO DEBUG DRAW SPHERE OUTSIDE OF DRAW GIZMOS
    }
    void RangedAttackOutgoingTransitions()
    {
        if (AttackTimer <= 0)
        {
            animator.SetBool("Moving", true);
            SwitchToGetToTree();
        }
    }
    void SwitchToRangedAttack()
    {
        animator.SetBool("Moving",false);
        animator.SetTrigger("Attack");
        AttackTimer = attackTimerMax;
        stateUpdate = RangedAttack;
    }
    #region
    /*
     * The MIT License (MIT)

Copyright (c) 2008 Daniel Brauer

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.*/
    //first-order intercept using absolute target position
    public static Vector3 FirstOrderIntercept
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t * (targetRelativeVelocity);
    }
    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
    #endregion LeadShot LeadShot

    bool CanSeePlayer()
    {

        RaycastHit hit;
        Ray ray = new Ray(transform.position, playerReference.transform.position - transform.position);
        Physics.Raycast(ray, out hit);
        return hit.transform == playerReference.transform;

    }
}
