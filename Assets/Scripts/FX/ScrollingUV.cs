﻿using UnityEngine;
using System.Collections;

public class ScrollingUV : MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";
    public MeshRenderer renderer;

    Vector2 uvOffset = Vector2.zero;

    private void Awake()
    {
        if (renderer == null) renderer = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (renderer.enabled)
        {
            renderer.materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }
    }
}