using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIprototype : MonoBehaviour {
    NavMeshAgent pf;
    Vector3 wanderPoint;
    float wanderTimer = 0;
    float wanderTimerMax = 2.5f;
	// Use this for initialization
    delegate void State();
    State stateUpdate;
	void Start () {
	    pf = GetComponent<NavMeshAgent>();	
        SwitchToIdle();
	}
	
	// Update is called once per frame
	void Update () {
        stateUpdate();
	}

    //state boiler plate.
    //IDLE
    void Idle()
    {
        pf.isStopped = true;
        IdleOutGoingTransitions();
        //IDLE CLEANUP
        if(stateUpdate != Idle)
        {
            pf.isStopped = false;
        }
    }
    void IdleOutGoingTransitions() //Designer Freindly //MUST BE CALLED IN IDLE
    {
        SwitchToWander();
    }
    void SwitchToIdle()
    {
        stateUpdate = Idle;
    }
    //WANDER
    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            wanderTimer += wanderTimerMax;
            var randomInCircle = Random.insideUnitCircle;
            wanderPoint = (new Vector3(randomInCircle.x, 0, randomInCircle.y)) * 5;
            print(wanderPoint);
        }

        pf.SetDestination(wanderPoint);
        WanderOutGoingTransitions();
    }
    void WanderOutGoingTransitions() //Designer Freindly //MUST BE CALLED IN WANDER
    {
    }
    void SwitchToWander()
    {
        stateUpdate = Wander;
        wanderTimer = 0;
    }
}
