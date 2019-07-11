using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibletSpawner : MonoBehaviour
{
    public class SpawnedObject
    {
        public GameObject Object;
        public Vector3 Position;
        public Quaternion Rotation;

        public SpawnedObject(GameObject _object, Vector3 _position, Quaternion _rotation)
        {
            Object = _object;
            Position = _position;
            Rotation = _rotation;
        }
    }

    public static List<SpawnedObject> SpawnStack = new List<SpawnedObject>();

	void Update ()
    {
		if(SpawnStack.Count > 0)
        {
            Instantiate(SpawnStack[0].Object, SpawnStack[0].Position, SpawnStack[0].Rotation);
            SpawnStack.Remove(SpawnStack[0]);
        }
	}
}