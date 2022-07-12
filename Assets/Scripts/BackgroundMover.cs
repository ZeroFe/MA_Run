using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    public float moveSpeed = 0.8f;

    private Material mat;

    private void Awake()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
    }

    private void Update()
    {
        mat.mainTextureOffset += Vector2.right * (moveSpeed * Time.deltaTime);
    }
}
