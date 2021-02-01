using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Entities;
using Scrapper.Managers;
using UnityEngine;
using UnityEngine.UI;

public class APPlayerBar : MonoBehaviour
{
    public Entity player = null;
    private bool playerNotNull = false;
    [SerializeField] private int[] apValuePositions = new int[10];
    public Image apBarGraphic;

    private void LateUpdate()
    {
        if (!EntityManager.turnBasedEngaged)
            apBarGraphic.fillAmount = 0f;
        
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
            int playerAp = player.actionPts[0];
            float fillAmount = (playerAp == 0) ? 0f : (float)apValuePositions[playerAp - 1] * 0.01f;
            apBarGraphic.fillAmount = fillAmount;
        }
    }
}
