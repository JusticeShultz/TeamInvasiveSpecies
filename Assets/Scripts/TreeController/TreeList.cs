using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeList : MonoBehaviour
{
    public static List<TreeController> Trees = new List<TreeController>();

	void Start ()
    {
        Trees.Add(GetComponent<TreeController>());
	}

}