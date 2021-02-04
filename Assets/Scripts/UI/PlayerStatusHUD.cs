using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Entities;
using Scrapper.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerStatusHUD : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Entity player = null;
    private bool playerNotNull = false;
    public Gradient healthGradient = new Gradient();
    public Color deadColor = new Color();
    public Image healthBorderImg;
    public MouseHoverTooltip Tooltip; //Not a great way to handle it...
    public static bool drawingTooltip = false;

    private void Awake()
    {
        Tooltip = FindObjectOfType<MouseHoverTooltip>();
    }

    private void LateUpdate()
    {
        if (!playerNotNull)
            for (int i = 0; i < EntityManager.Entities.Count; i++)
            {
                if (EntityManager.Entities[i]._pathfinder.playerControlled)
                {
                    player = EntityManager.Entities[i];
                    playerNotNull = true;
                }
            }
        else
        {
            Color borderColor = healthGradient.Evaluate(1f - (float)player.healthPts[0] / (float)player.healthPts[1]);
            if (player.healthPts[0] <= 0) borderColor = deadColor;
            healthBorderImg.color = borderColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerStatusHUD.drawingTooltip = true;
        Tooltip.CreateTooltip(player.entityName, $"Health: {player.healthPts[0]}/{player.healthPts[1]} ({player.GetHealthPercentageStatus()})\n" +
                                                            $"Level: {player.expPts[1]}, XP to next level: {player.expPts[0]}");
        Tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.gameObject.SetActive(false);
        Tooltip.DestroyTooltip();
        PlayerStatusHUD.drawingTooltip = false;
    }
}
