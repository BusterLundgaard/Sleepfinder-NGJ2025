using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Polyfunks;
using UnityEngine;

public class PolyfunksReference : MonoBehaviour
{
    public Polyfunks2_Graph polyfunks2Graph;
    public string paramId = "polyfunk";
    public float value = 0;

    public bool onUpdate = true;

    private void OnValidate()
    {
        if (polyfunks2Graph == null)
            polyfunks2Graph = GetComponentInParent<Polyfunks2_Graph>();
    }

    private void Update()
    {
        if (onUpdate)
            SetPolyfunkParam(paramId, value, false);
    }

    public void SetPolyfunkParam(string paramId, float value, bool instant)
    {
        var all = polyfunks2Graph.graph.GetParameterNodes();
        foreach (var p in all)
        {
            p.SetParameterFromScript(paramId, value, instant);
        }
    }
}
