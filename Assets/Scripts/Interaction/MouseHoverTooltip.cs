using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseHoverTooltip : MonoBehaviour
{
    public RectTransform tooltipContainer;
    public TextMeshProUGUI tooltipTitle;
    public TextMeshProUGUI tooltipContent;
    public LayoutElement LayoutElement;
    public int breakNewLineAt = 50;
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
        LayoutElement.enabled = (title.Length >= breakNewLineAt || content.Length >= breakNewLineAt * 2);
            
        tooltipTitle.text = title;
        tooltipContent.text = content;
        GetComponent<RectTransform>().position = Input.mousePosition;
        drawTooltip = true;
    }

    public void DestroyTooltip()
    {
        tooltipTitle.text = "";
        tooltipContent.text = "";
        drawTooltip = false;
    }
}
