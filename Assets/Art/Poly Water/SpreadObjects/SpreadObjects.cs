using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpreadObjects : MonoBehaviour
{
    public int maxPlacedObjects = 15;
    public int minimumPlacedObjects = 4;

    public float xSpread = 10;
    public float zSpread = 10;

    public Vector3 rotationInDegrees = Vector3.zero;
    public Vector3 randomRotation = Vector3.zero;

    public Vector3 scaleFactor = Vector3.one;
    public Vector3 maxScale = Vector3.zero;
    public Vector3 minScale = Vector3.zero;

    public bool UniformScaling = true;

    public LayerMask floorMask;

    public List<GameObject> objectVarient = new List<GameObject>();

    public void Spread()
    {
        int amount = Random.Range(minimumPlacedObjects, maxPlacedObjects + 1);

        for(int i = 0; i < amount; ++i)
        {
            float 
            x = Random.Range(-xSpread - 1, xSpread + 1),
            z = Random.Range(-zSpread - 1, zSpread + 1),
            rotX = Random.Range(-randomRotation.x, randomRotation.x),
            rotY = Random.Range(-randomRotation.y, randomRotation.y),
            rotZ = Random.Range(-randomRotation.z, randomRotation.z);

            int objType = Random.Range(0, objectVarient.Count);

            GameObject obj = Instantiate(objectVarient[objType], 
            new Vector3(x + transform.position.x, transform.position.y, z + transform.position.z), 
            Quaternion.Euler(rotationInDegrees + new Vector3(rotX, rotY, rotZ)));

            float 
            scaleX = Random.Range(minScale.x, maxScale.x), 
            scaleY, 
            scaleZ;

            if (UniformScaling)
            {
                scaleY = scaleX;
                scaleZ = scaleX;
            }
            else
            {
                scaleY = Random.Range(minScale.y, maxScale.y);
                scaleZ = Random.Range(minScale.z, maxScale.z);
            }

            obj.transform.localScale = new Vector3(scaleFactor.x + scaleX, scaleFactor.y + scaleY, scaleFactor.z + scaleZ);

            Ray groundCheck = new Ray();

            groundCheck.origin = obj.transform.position + (Vector3.up * 10);
            groundCheck.direction = -transform.up;

            RaycastHit hitInfo = new RaycastHit();

            if(Physics.Raycast(groundCheck, out hitInfo, 100, floorMask))
                obj.transform.position = hitInfo.point;
        }
    }
}