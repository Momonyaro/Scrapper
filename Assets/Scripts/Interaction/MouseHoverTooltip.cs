using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseHoverTooltip : MonoBehaviour
{
    public RectTransform tooltipContainer;
    public Text tooltipTitle;
    public bool drawTooltip = false;

    private void Update()
    {
        if (!drawTooltip && gameObject.activeInHierarchy) { gameObject.SetActive(false); }
        if (drawTooltip && !gameObject.activeInHierarchy) { gameObject.SetActive(true);  }

        if (!drawTooltip) return;

        float halfWidth = tooltipContainer.rect.width / 2;
        float halfScreenWidth = Screen.width / 2;

        if (Input.mousePosition.x < halfScreenWidth)
        {
            tooltipContainer.anchoredPosition = new Vector2(halfWidth, tooltipContainer.anchoredPosition.y);
        }
        else
        {
            tooltipContainer.anchoredPosition = new Vector2(-halfWidth, tooltipContainer.anchoredPosition.y);
        }

        GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void CreateTooltip(string title, string content)
    {
        tooltipTitle.text = title + "   (" + content + ")";
        GetComponent<RectTransform>().position = Input.mousePosition;
        drawTooltip = true;
    }

    public void DestroyTooltip()
    {
        tooltipTitle.text = "";
        drawTooltip = false;
    }
}
