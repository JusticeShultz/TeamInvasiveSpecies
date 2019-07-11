using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDeath : MonoBehaviour
{
    public float LiveTime;

    private void Update()
    {
        LiveTime -= Time.deltaTime;

        if (LiveTime <= 0) Destroy(gameObject);
    }
}
