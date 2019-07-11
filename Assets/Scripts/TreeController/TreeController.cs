using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public GameObject FallenObj;
    public float Health = 10f;

    public void Damage(float damage, Vector3 direction)
    {
        BloodPool.Splatter(transform.position + direction, Mathf.FloorToInt(damage), BloodPool.BloodColor.Yellow);

        Health -= damage;

        if(Health <= 0)
        {
            var obj = Instantiate(FallenObj, transform.position, transform.rotation);
            obj.transform.localScale = transform.localScale;

            foreach (Transform transformobj in transform)
                transformobj.parent = null;

            foreach (Transform trans in obj.transform)
                if (trans.name == "Pine_pCube1") trans.GetComponent<Rigidbody>().velocity = direction*-3f;

            

            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        TreeList.Trees.Remove(this);
    }
}