using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    private Color basicColor = Color.black;
    private Color hoverColor = Color.blue;
    private Color selectColor = Color.green;
    public Image boarder;

    public void MouseOver()
    {
        boarder.color = hoverColor;
    }

    public void MouseOff()
    {
        boarder.color = basicColor;
    }

    public void Selected ()
    {
        boarder.color = selectColor;
    }
}
