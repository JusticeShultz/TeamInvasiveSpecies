using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class gibletJiggler : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] originalVertices, deformedVerticies;
    //p = + and n = -; sorry for bad naming convention
    [SerializeField] Transform[] cubeNodes;
    [SerializeField] Rigidbody[] cubeNodesRb;

    // Use this for initialization
    void OnEnable () {
        StartCoroutine(die());
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        cubeNodesRb[0].WakeUp();
        deformedVerticies = new Vector3[originalVertices.Length];
        for(int i = 0; i < originalVertices.Length; i++)
        {
            deformedVerticies[i] = originalVertices[i];
        }
        impulse(Random.insideUnitSphere * 25);
    }

	void Update () {
        if (Vector3.Distance(transform.position, PlayerController.Player_Controller.transform.position) > 100 && !cubeNodesRb[0].IsSleeping())
        {
            return; //too far do no calculations
        }
        Vector3 center = Vector3.zero;
    
        foreach(Transform cubeNode in cubeNodes)
        {
            center += cubeNode.localPosition/8;
        }
        
        Vector3 deformedXAxis = ((cubeNodes[4].localPosition + cubeNodes[5].localPosition + cubeNodes[6].localPosition + cubeNodes[7].localPosition) / 4)
            -
            ((cubeNodes[0].localPosition + cubeNodes[1].localPosition + cubeNodes[2].localPosition + cubeNodes[3].localPosition) / 4)
            ;
        Vector3 deformedYAxis =
            ((cubeNodes[2].localPosition + cubeNodes[3].localPosition + cubeNodes[6].localPosition + cubeNodes[7].localPosition) / 4)
            -
            ((cubeNodes[0].localPosition + cubeNodes[1].localPosition + cubeNodes[4].localPosition + cubeNodes[5].localPosition) / 4)
            ; 
        Vector3 deformedZAxis = ((cubeNodes[1].localPosition + cubeNodes[3].localPosition + cubeNodes[5].localPosition + cubeNodes[7].localPosition) / 4)
            -
            ((cubeNodes[0].localPosition + cubeNodes[2].localPosition + cubeNodes[4].localPosition + cubeNodes[6].localPosition) / 4)
            ;
        for (int i = 0; i < originalVertices.Length; i++)
        {
            deformedVerticies[i] = ((originalVertices[i].x*deformedXAxis) + (originalVertices[i].y * deformedYAxis) + (originalVertices[i].z * deformedZAxis) + center);
        }
        deformingMesh.vertices = deformedVerticies;

        //for(int i = 0; i < deformingMesh.normals.Length; ++i)
        // deformingMesh.normals[i] *= -1;
    }
    public void impulse(Vector3 velocity)
    {
        foreach(var rb in cubeNodesRb)
        {
            rb.velocity = velocity;
        }
    }

    IEnumerator die()
    {
        yield return new WaitForSeconds(10) ;
        Destroy(gameObject);
    }
}