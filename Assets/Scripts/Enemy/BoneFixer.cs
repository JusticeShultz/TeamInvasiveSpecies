using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneFixer : MonoBehaviour {

	void Awake () {
		var springJoints = GetComponents<SpringJoint>();
        foreach(var springJoint in springJoints)
        {
            //springJoint.minDistance *= transform.parent.localScale.x; //assumeing uniform scale
            //springJoint.maxDistance *= transform.parent.localScale.x;
            //springJoint.spring *= transform.parent.localScale.x;
        }
	}

}
