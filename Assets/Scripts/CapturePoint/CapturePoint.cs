using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    public float ChargeTime = 7.0f;
    public UnityEngine.UI.Image FillBar;
    public LayerMask LayerMask;
    public float Radius;
    public GameObject CaptureEffect;
    public GameObject CaptureBeam;
    public GameObject ForceField;

    AudioSource audioObj;

    [ReadOnlyField] public bool Captured = false;
    [ReadOnlyField] public float CurrentCharge;

    private void Start()
    {
        audioObj = GetComponent<AudioSource>();
        Captured = false;
    }

    void Update()
    {
        if (Captured) return;

        if (CurrentCharge >= ChargeTime && !Captured)
        {
            Captured = true;
            PointCaptured();
        }
        else
        {
            if (Physics.OverlapSphere(transform.position, Radius, LayerMask).Length > 0)
                CurrentCharge += Time.deltaTime;
            else if (CurrentCharge > 0) CurrentCharge -= Time.deltaTime;

            FillBar.fillAmount = CurrentCharge / ChargeTime;
        }
	}

    public void PointCaptured()
    {
        FillBar.color = Color.white;
        CaptureEffect.SetActive(true);
        CaptureBeam.SetActive(false);
        audioObj.enabled = true;

        if (ForceField)
            ForceField.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.45f);
        Gizmos.DrawSphere(transform.position, Radius);
    }
}