using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBalloon : MonoBehaviour
{
    public SkinnedMeshRenderer _renderer;
    public float Damage = 50f;
    public float Range = 15f;
    public Vector3 directionalForce = new Vector3(0, -10, 0);
    public LayerMask layerMask;
    public Rigidbody masterRigidbody;
    public GameObject WaterEffect;
    [ReadOnlyField] public bool CollisionContactMade = false;
    [ReadOnlyField] public float Dissolve = 0f;
    [ReadOnlyField] public bool OneCheckTrigger = false;

    void Start ()
    {
        masterRigidbody.AddForce(directionalForce, ForceMode.VelocityChange);
    }
	
	void Update ()
    {
		if(CollisionContactMade)
        {
            Dissolve = Mathf.Lerp(Dissolve, 1f, 0.015f);
            _renderer.material.SetFloat("_SliceAmount", Dissolve);

            if(!OneCheckTrigger)
            {
                OneCheckTrigger = true;
                StartCoroutine(deathTriggered());
            }
        }
	}

    IEnumerator deathTriggered()
    {
        Instantiate(WaterEffect, transform.position, Quaternion.identity);
        //Splash sound created here (Don't play sound on this object)

        //Instantiate effects here:

        //

        //RaycastHit[] hit = Physics.SphereCastAll(transform.position, Range, Vector3.zero, layerMask);

        Collider[] hit = Physics.OverlapSphere(transform.position, Range, layerMask);

        if(hit.Length > 0)
        {
            foreach(Collider hitObj in hit)
            {
                if (hitObj.transform.tag == "Enemy")
                {
                    if (hitObj.transform.GetComponent<FoxAI>())
                        hitObj.transform.GetComponent<FoxAI>().Damage(Damage, transform.position - hitObj.transform.transform.position, true);
                }
                else if (hitObj.transform.tag == "Snail")
                    hitObj.transform.GetComponent<SnailWander>().Damage(Damage, transform.position - hitObj.transform.transform.position);
                else if (hitObj.transform.tag == "Mushroom")
                    hitObj.transform.GetComponent<MushroomAI>().Damage(Damage, transform.position - hitObj.transform.transform.position);
                else if (hitObj.transform.tag == "Tree")
                    hitObj.transform.GetComponent<TreeController>().Damage(Damage, transform.position - hitObj.transform.transform.position);
                else if (hitObj.transform.tag == "RangedEnemy")
                    hitObj.transform.GetComponent<RangedAI>().Damage(Damage, transform.position - hitObj.transform.transform.position);
            }
        }

        yield return new WaitForSeconds(1.4f);

        foreach(Transform transformObj in transform) if (transformObj.name == "BloodSplatter (Clone)") transformObj.parent = null;

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "joint2" || collision.gameObject.name == "joint3" ) return;

        CollisionContactMade = true;
    }
}