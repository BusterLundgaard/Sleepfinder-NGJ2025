using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;

public class MouseControlledPolyfunksParams : MonoBehaviour
{
    public PolyfunksReference pr;

    public Vector2 screenMin = new Vector2(0, 0);
    public Vector2 screenMax = new Vector2(1, 1);

    public enum When
    {
        Always,
        OnClick,
        Toggle
    }

    [EnumButtons] public When whenToSet;

    private bool whenToSet_ToggleState = false;

    public bool x = true;
    public bool y = true;

    public string paramX = "polyfunkx";
    public string paramY = "polyfunky";

    public Vector2 xRange = Vector2.up;
    public Vector2 yRange = Vector2.up;

    void Update()
    {
        if (whenToSet == When.Always)
        {
            SetValuesToPolyfunks();
        }
        else if (whenToSet == When.OnClick)
        {
            if (Input.GetMouseButton(0))
                SetValuesToPolyfunks();
        }
        else if (whenToSet == When.Toggle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                whenToSet_ToggleState = !whenToSet_ToggleState;
            }

            if (whenToSet_ToggleState)
                SetValuesToPolyfunks();
        }
    }

    private void SetValuesToPolyfunks()
    {
        var mouse01 = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
        var mousePosLimited01 = new Vector2(
            Mathf.Clamp01(Mathf.InverseLerp(screenMin.x, screenMax.x, mouse01.x)),
            Mathf.Clamp01(Mathf.InverseLerp(screenMin.y, screenMax.y, mouse01.y))
        );

        if (x)
        {
            pr.SetPolyfunkParam(paramX, Mathf.Lerp(xRange.x, xRange.y,mousePosLimited01.x), false);
        }

        if (y)
        {
            pr.SetPolyfunkParam(paramY, Mathf.Lerp(yRange.y, yRange.y, mousePosLimited01.y), false);
        }
    }
}