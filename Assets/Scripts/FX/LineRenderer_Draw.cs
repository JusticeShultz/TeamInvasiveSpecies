using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderer_Draw : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject StartPoint;
    public GameObject EndPoint;

	void Update ()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, StartPoint.transform.position);
        lineRenderer.SetPosition(1, EndPoint.transform.position);
	}

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.white;

        if (StartPoint && EndPoint)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, StartPoint.transform.position);
            lineRenderer.SetPosition(1, EndPoint.transform.position);

            //Gizmos.DrawLine(StartPoint.transform.position, EndPoint.transform.position);
        }
    }
}
