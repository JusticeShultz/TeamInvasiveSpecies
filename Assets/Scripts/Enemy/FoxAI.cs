using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class FoxAI : MonoBehaviour
{
    //don't touch these unless your name is justice
    public Animator animator;
    NavMeshAgent pf;
    Vector3 wanderPoint;
    float wanderTimer = 0;
    float AttackTimer = 0;
    Vector3 lastAttackPos;
    [ReadOnlyField] public float health = 0;
    delegate void State();
    State stateUpdate;
    Collider c;
    bool randomStrafeDir; //used to create variety in the way the enemy advances on the player
    float randomStrafeAmmount;
    [SerializeField] PlayerController playerReference;
    //anyone can change these
    [SerializeField] float wanderDistance = 5f;
    [SerializeField] float attackTimerMax = 1;
    [SerializeField] float wanderTimerMax = 2.5f;
    [SerializeField] float attackRadius = 1;
    [SerializeField] float attackRange = 1;
    [SerializeField] float attackDamage = 1;
    [SerializeField] float attackKnockBack = 1;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float selfKockBackScale = 100;
    [SerializeField] float seekRadius = 2.5f;
    [SerializeField] GameObject[] giblets;
    [SerializeField] GameObject renderObj;
    [SerializeField] GameObject deathPuff;
    [SerializeField] AnimationCurve strafeCurve;

    void Start()
    {
        randomStrafeAmmount = Random.value;
        randomStrafeDir = Random.value > 0.5f;
        health = maxHealth;
        pf = GetComponent<NavMeshAgent>();
        c = GetComponent<Collider>();
        SwitchToWander();
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerReference && MenuStart.GameStarted) {
            playerReference = PlayerController.Player_Controller;
        }
        else if (health > 0 && MenuStart.GameStarted && playerReference.CurrentHealth>=0) {
            stateUpdate();
        }
    }
    public void Damage(float damage, Vector3 direction, bool IsLargeDeathCauser = false)
    {
        if(!IsLargeDeathCauser)
            BloodPool.Splatter(transform.position + direction, Mathf.FloorToInt(damage), BloodPool.BloodColor.Red);
        else BloodPool.Splatter(transform.position + direction, 4, BloodPool.BloodColor.Red);

        health -= damage;
        if (health <= 0)
        {
            if(NavMeshValid())pf.enabled = false;
            direction.y = 0;
            c.enabled = false;

            if(!IsLargeDeathCauser)
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

            deathPuff.SetActive(true);
            deathPuff.transform.parent = null;

            Destroy(gameObject);
        }
    }
    private bool NavMeshValid()
    {
        return pf.isOnNavMesh && pf.enabled;
    }
    //state boiler plate.
    //TEMPLATE BEHAVIOUR
    /*
     *void Behaviour()
     * {
     *  BehaviourOutGoingTransisitons()
     * }
     *void BehaviourOutGoingTransisitons(){}
     *void SwitchToBehaviour(){
     *  StateUpdate = Behaviour;
     * }
     */
    //SEEK_PLAYER
    void SeekPlayer(){
        Vector3 goal = playerReference.transform.position;
        Vector3 dirToPlayer = (transform.position - goal).normalized;
        float distToPlayer = Vector3.Distance(goal, transform.position);
        float strafeAmmount = strafeCurve.Evaluate(distToPlayer/seekRadius)*randomStrafeAmmount;
        Vector3 strafePoint = (new Vector3(dirToPlayer.x * ((randomStrafeDir) ? (-1) : (1)), 0, dirToPlayer.y * ((randomStrafeDir) ? (-1) : (1)))*distToPlayer*strafeAmmount)+transform.position;
        float distToPlayerFromStrafePoint = Vector3.Distance(goal, strafePoint);
        Vector3.MoveTowards(strafePoint, goal, distToPlayerFromStrafePoint-distToPlayer);
        goal = (strafePoint + goal)/2;
        pf.SetDestination(goal);
        SeekPlayerOutGoingTransisitons();
    }
    void SeekPlayerOutGoingTransisitons(){
        if (!(Vector3.Distance(playerReference.transform.position, transform.position) < seekRadius || Vector3.Dot(transform.forward, playerReference.transform.position - transform.position) > 0.5))
        {
            SwitchToWander();
            return;
        }
        if(Vector3.Distance(playerReference.transform.position, transform.position) < (attackRange + attackRadius) && Vector3.Dot(transform.forward, playerReference.transform.position - transform.position) > 0.5f)
        {
            SwitchToMeleeAttack();
            return;
        }
    }
    void SwitchToSeekPlayer()
    {
        animator.SetBool("Moving", true);
        stateUpdate = SeekPlayer;
    }
    
    //WANDER
    void Wander()
    {
        
        if(NavMeshValid() && pf.remainingDistance < 0.25f)
            animator.SetBool("Moving", false);
        else animator.SetBool("Moving", true);

        WanderOutGoingTransitions();
        if(stateUpdate==Wander)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                NavMeshPath path = new NavMeshPath();
                int i = 0;
                do{
                    var randomInCircle = Random.insideUnitCircle;
                    wanderPoint = ((new Vector3(randomInCircle.x, 0, randomInCircle.y)) * wanderDistance) +transform.position;
                    NavMesh.CalculatePath(transform.position,wanderPoint, NavMesh.AllAreas, path );
                    Debug.DrawLine(transform.position, wanderPoint, Color.red);
                    i++;
                } while(path.status != NavMeshPathStatus.PathComplete && i<100);
                if(i >= 100)
                {
                    print("FAILED");
                } else
                {
                    wanderTimer = wanderTimerMax;
                }
                pf.SetPath(path);
            }
        }
    }
    void WanderOutGoingTransitions() //Designer Freindly //MUST BE CALLED IN WANDER
    {
        if(Vector3.Distance(playerReference.transform.position,transform.position)< seekRadius)
        {
            SwitchToSeekPlayer(); //really should try and attack them first but whatever
        }
    }
    void SwitchToWander()
    {
        stateUpdate = Wander;
        wanderTimer = 0;
    }
    //MELEE ATTACK
    void MeleeAttack()
    {
        if(NavMeshValid()) pf.isStopped = true;
        AttackTimer -= Time.deltaTime;
        if (AttackTimer <= 0)
        {
            animator.SetTrigger("Attack");
            var hits = Physics.SphereCastAll(transform.position + transform.forward*attackRange, attackRadius, transform.forward, 0);
            lastAttackPos = transform.position + transform.forward * attackRange;
            foreach(var hit in hits) {
                print("hit " + hit.transform.name);
                if (hit.rigidbody) {
                    if (hit.transform == playerReference.transform) {
                        hit.rigidbody.AddForce(transform.forward * attackKnockBack, ForceMode.Impulse);
                        playerReference.TakeDamage(attackDamage, transform.position + (transform.position - playerReference.transform.position));
                    }
                }
            } 
        }
        MeleeAttackOutGoingTransitions();
        if(stateUpdate != MeleeAttack)
        {
            if(NavMeshValid())pf.isStopped = false;
        }
        //Debug.draw WHY IS THEIR NO DEBUG DRAW SPHERE OUTSIDE OF DRAW GIZMOS
    }
    void MeleeAttackOutGoingTransitions()
    {
        if(AttackTimer <= 0)
        {
            SwitchToWander();
        }
    }
    void SwitchToMeleeAttack()
    {
        animator.SetBool("Moving", false);
        AttackTimer = attackTimerMax;
        stateUpdate = MeleeAttack;
    }
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
}
