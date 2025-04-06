using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiToPolyfunksDemo : MonoBehaviour
{
    public List<Slider> sliders = new();

    public List<PolyfunksReference> polyfunksReferences = new();

    void Update()
    {
        for (int i = 0; i < Mathf.Min(polyfunksReferences.Count, sliders.Count); i++)
        {
            Slider sl = sliders[i];
            PolyfunksReference pr = polyfunksReferences[i];

            pr.value = sl.value;

        }
    }
}
