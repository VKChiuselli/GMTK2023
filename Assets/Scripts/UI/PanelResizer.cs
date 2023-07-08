using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelResizer : MonoBehaviour
{
    public float localScalex;
    public float localScaley;
    public float varX;
    public float varY;

    void Update()
    {
        CheckResolution();

        GetComponent<RectTransform>().localScale = new Vector2(res_x * localScalex, res_y * localScaley);
        GetComponent<RectTransform>().localPosition = new Vector2(res_x * varX, res_y * varY);

    }

    int res_x;
    int res_y;

    public void CheckResolution()
    {
        res_x = Screen.width;
        res_y = Screen.height;
    }


}
