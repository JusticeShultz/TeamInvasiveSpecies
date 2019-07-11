using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodModule : MonoBehaviour
{
    public float SplatterIntensity = 10f;
    public float ShrinkIntensity = 0.1f;
    public float MaxSize = 1.0f;
    public MeshRenderer MatObj;

    private bool Impact = false;
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    float aliveTime = 0;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        Mesh deformingMesh = GetComponent<MeshFilter>().mesh;
        Vector3[] originalVertices = deformingMesh.vertices;

        for(int i = 0; i < originalVertices.Length; ++i)
            originalVertices[i] += Random.insideUnitSphere * 0.35f;

        deformingMesh.vertices = originalVertices;
        float size = Random.Range(0.15f, MaxSize);
        transform.localScale = new Vector3(size, size, size);
        rb = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
    }

    public void Instantiate()
    {
        rb.isKinematic = false;
        sphereCollider.enabled = true;
        Impact = false;
        transform.parent = null;
        float size = Random.Range(0.5f, MaxSize);
        transform.localScale = new Vector3(size, size, size);
        aliveTime = 0;
        rb.velocity = new Vector3(Random.Range(-SplatterIntensity, SplatterIntensity), Random.Range(3, SplatterIntensity * 1.5f), Random.Range(-SplatterIntensity, SplatterIntensity));
    }

    private void Update()
    {
        aliveTime += Time.deltaTime;
        if(!Impact) rb.velocity += Vector3.down * 0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (aliveTime >= 0.35f)
        {
            Impact = true;
            transform.parent = other.transform;
            rb.isKinematic = true;
            sphereCollider.enabled = false;
            StartCoroutine(ShrinkOverTime());
        }
    }

    IEnumerator ShrinkOverTime()
    {
        yield return new WaitForSeconds(ShrinkIntensity);

        transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f);

        if (transform.localScale.x <= 0.01f)
        {
            transform.parent = BloodPool.BloodPoolObject.transform;
            gameObject.SetActive(false);
        }
        else StartCoroutine(ShrinkOverTime());
    }
}
