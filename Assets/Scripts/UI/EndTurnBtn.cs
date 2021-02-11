using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndTurnBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color offColor = new Color();
    private Color _borderOnColor = new Color();
    private Color _textOnColor = new Color();

    public Image chineseImg;
    public Image textImg;
    private Image borderImg;
    private Button borderBtn;

    private void Awake()
    {
        borderImg = GetComponent<Image>();
        borderBtn = GetComponent<Button>();
        _borderOnColor = borderImg.color;
        _textOnColor = textImg.color;
    }

    private void LateUpdate()
    {
        if (!EntityManager.playerTurnFlag)
        {
            textImg.color = offColor;
            borderImg.color = offColor;
            chineseImg.gameObject.SetActive(false);
            borderBtn.interactable = false;
        }
        else
        {
            textImg.color = _textOnColor;
            borderImg.color = _borderOnColor;
            borderBtn.interactable = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EntityManager.playerTurnFlag)
        {
            chineseImg.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EntityManager.playerTurnFlag)
        {
            chineseImg.gameObject.SetActive(false);
        }
    }
}
