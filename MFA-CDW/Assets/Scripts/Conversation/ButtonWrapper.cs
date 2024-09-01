using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWrapper : Button
{
    public bool IsItHighlighted()
    {
        return IsHighlighted();
    }

    public void SetImageVisibility(bool val)
    {
        Image img = GetComponent<Image>();
        img.enabled = val;
    }
}
