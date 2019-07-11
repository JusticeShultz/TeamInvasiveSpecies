using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScrollingSkybox : MonoBehaviour
{
    public float SkyboxTurnSpeed = 0.1f;

    private void Awake()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }

    void Update ()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * SkyboxTurnSpeed);
	}
}
