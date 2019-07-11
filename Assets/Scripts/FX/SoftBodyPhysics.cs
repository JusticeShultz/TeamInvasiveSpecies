using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBodyPhysics : MonoBehaviour
{
    private bool Collided = false;
    public float JiggleIntensity = 3.0f;
    public Rigidbody _rigidbody;
    float ChangeTime = 0f;
    Vector3 changeTo;

    private void Update()
    {
        if (Collided)
        {
            ChangeTime += Time.deltaTime;

            if (ChangeTime > 0.15f)
            {
                ChangeTime = 0f;
                changeTo = new Vector3(
                1, 1 + (Mathf.Sin((_rigidbody.velocity.y * (JiggleIntensity * 0.3f))) * ((_rigidbody.velocity.y * JiggleIntensity) * 0.2f)), 1);

                //changeTo.z = changeTo.x;
                //changeTo.y = changeTo.x; 
            }

            transform.localScale = Vector3.Lerp(transform.localScale, changeTo, 0.06f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collided = true;
    }
}
