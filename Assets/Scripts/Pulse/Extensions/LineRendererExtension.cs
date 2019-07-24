using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineRendererExtension
{
    public static void InitilizeLine(this LineRenderer renderer, Transform transform, Transform right, Color makerColor)
    {
        renderer.SetPosition(0, transform.position);
        renderer.SetPosition(1, right.position);
        renderer.startWidth = 0.1f;
        renderer.endWidth = 0.1f;
        renderer.material.color = makerColor;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.generateLightingData = true;
    }
}
