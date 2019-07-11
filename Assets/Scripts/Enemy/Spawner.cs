using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField] GameObject enemy;
    [SerializeField] float spawnRate;
    [SerializeField] float radius;
    void Start()
    {
        StartCoroutine("Timer");
    }
    IEnumerator Timer () {
        while (true) {
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Instantiate(enemy, transform.position+(new Vector3(randomPoint.x, 0, randomPoint.y)), Quaternion.identity);
            yield return new WaitForSeconds(spawnRate);
        }
    }
}
