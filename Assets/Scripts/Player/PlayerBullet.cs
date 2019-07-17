using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public bool ChainLightning = false;
    public Rigidbody rb;
    public LayerMask layerMask;
    LineRenderer lineRender;
    public GameObject electrocute;
    public GameObject EffectsObject;

    [ReadOnlyField] public float Damage = 0.0f;
    [ReadOnlyField] public Vector3 FireVector;
    [ReadOnlyField] public float ShotSpeed = 0.0f;

    private bool IsDead = false;

    private void Start()
    {
        if (ChainLightning)
            lineRender = GetComponent<LineRenderer>();
    }

    void Update ()
    {
        if (!IsDead)
            rb.velocity = FireVector * ShotSpeed;
        else rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool hitEnemy = false;

        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<FoxAI>())
            {
                hitEnemy = true;
                collision.gameObject.GetComponent<FoxAI>().Damage(Damage, transform.position - collision.gameObject.transform.position);
            }
        }
        else if (collision.gameObject.tag == "Snail")
        {
            collision.gameObject.GetComponent<SnailWander>().Damage(Damage, transform.position - collision.gameObject.transform.position);
            hitEnemy = true;
        }
        else if (collision.gameObject.tag == "Mushroom")
        {
            collision.gameObject.GetComponent<MushroomAI>().Damage(Damage, transform.position - collision.gameObject.transform.position);
            hitEnemy = true;
        }
        else if (collision.gameObject.tag == "Tree")
        {
            collision.gameObject.GetComponent<TreeController>().Damage(Damage, transform.position - collision.gameObject.transform.position);
            hitEnemy = true;
        }
        else if (collision.gameObject.tag == "RangedEnemy")
        {
            collision.gameObject.GetComponent<RangedAI>().Damage(Damage, transform.position - collision.gameObject.transform.position);
            hitEnemy = true;
        }

        rb.velocity = Vector3.zero;
        
        if(hitEnemy && ChainLightning)
        {
            lineRender.positionCount = 0;

            Collider[] hit = Physics.OverlapSphere(transform.position, 8, layerMask);
            List<Vector3> positions = new List<Vector3>();

            if (hit.Length > 0)
            {
                foreach (Collider hitObj in hit)
                {
                    bool didHitObj = false;

                    if (hitObj.transform.tag == "Enemy")
                    {
                        if (hitObj.transform.GetComponent<FoxAI>())
                        {
                            didHitObj = true;
                            hitObj.transform.GetComponent<FoxAI>().Damage(35, transform.position - hitObj.transform.transform.position);
                            Instantiate(electrocute, hitObj.transform.position, Quaternion.identity);
                        }
                    }
                    else if (hitObj.transform.tag == "Snail")
                    {
                        didHitObj = true;
                        hitObj.transform.GetComponent<SnailWander>().Damage(35, transform.position - hitObj.transform.transform.position);
                        Instantiate(electrocute, hitObj.transform.position, Quaternion.identity);
                    }
                    else if (hitObj.transform.tag == "Mushroom")
                    {
                        didHitObj = true;
                        hitObj.transform.GetComponent<MushroomAI>().Damage(35, transform.position - hitObj.transform.transform.position);
                        Instantiate(electrocute, hitObj.transform.position, Quaternion.identity);
                    }
                    else if (hitObj.transform.tag == "Tree")
                    {
                        didHitObj = true;
                        hitObj.transform.GetComponent<TreeController>().Damage(35, transform.position - hitObj.transform.transform.position);
                        Instantiate(electrocute, hitObj.transform.position, Quaternion.identity);
                    }
                    else if (hitObj.transform.tag == "RangedEnemy")
                    {
                        didHitObj = true;
                        hitObj.transform.GetComponent<RangedAI>().Damage(35, transform.position - hitObj.transform.transform.position);
                        Instantiate(electrocute, hitObj.transform.position, Quaternion.identity);
                    }

                    if(didHitObj) positions.Add(hitObj.transform.position);
                }
            }

            if (positions.Count > 0)
            {
                lineRender.positionCount = positions.Count;// * 2;

                for (int i = 0; i < positions.Count; ++i)//i += 2)
                {
                    lineRender.SetPosition(i, positions[i]);
                    //lineRender.SetPosition(i + 1, transform.position);
                }
            }
        }

        if (!IsDead)
        {
            if (EffectsObject)
                EffectsObject.SetActive(false);

            IsDead = true;
            StartCoroutine(DoADie());
        }
    }

    IEnumerator DoADie()
    {
        yield return new WaitForSeconds(0.5f);

        if (ChainLightning)
            lineRender.positionCount = 0;

        yield return new WaitForSeconds(1f);

        IsDead = false;

        EffectsObject.SetActive(true);
        gameObject.SetActive(false);
    }
}