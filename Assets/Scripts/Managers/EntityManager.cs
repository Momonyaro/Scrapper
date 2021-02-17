using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Entities;
using UnityEngine;

namespace Scrapper.Managers
{
    public class EntityManager : MonoBehaviour
    {
        public static List<Entity> Entities = new List<Entity>();

        private static bool turnBasedStart = false;
        public static bool turnBasedEngaged = false;
        public static bool playerTurnFlag = false;
        public static bool endPlayerTurnFlag = false;
        public TurnPrompter TurnPrompter;
        public MouseHoverTooltip cachedMouseTooltip;

        public float waitTime = 0.15f; //To not just catapult between entities between turns.

        private void Awake()
        {
            cachedMouseTooltip = FindObjectOfType<MouseHoverTooltip>();
        }

        public static void ActivateTurnBased()
        {
            EntityManager.turnBasedStart = true;
            // Perhaps do some prep garbage here?
        }

        private void Update()
        {
            if (!EntityManager.turnBasedEngaged && EntityManager.turnBasedStart)
            {
                EntityManager.turnBasedEngaged = true;
                EntityManager.turnBasedStart = false;
                for (int i = 0; i < Entities.Count; i++)
                {
                    Entities[i].actionPts[0] = 0;
                }
                StartCoroutine(TurnBasedLoop());
            }
        }

        public void EndPlayerTurn()
        {
            if (playerTurnFlag)
                endPlayerTurnFlag = true;
        }

        private IEnumerator TurnBasedLoop()
        {
            while (EntityManager.turnBasedEngaged)
            {
                bool stayInCombat = false;
                //Debug.Log(EntityManager.Entities.Count);
                for (int i = 0; i < EntityManager.Entities.Count; i++)
                {
                    Entity entity = EntityManager.Entities[i];
                    
                    if (entity._pathfinder.playerControlled)
                    {
                        if (entity.healthPts[0] <= 0) yield break; //PLAYER DEAD
                        
                        playerTurnFlag = true;
                        TurnPrompter.PlayFadingPrompt();
                    }
                    
                    for (int j = 0; j < EntityManager.Entities.Count; j++)
                    {
                        if (EntityManager.Entities[j]._pathfinder.playerControlled) continue;
                        if (EntityManager.Entities[j].CheckForStillInCombat() && EntityManager.Entities[j].healthPts[0] > 0)
                        {
                            Debug.Log(EntityManager.Entities[j].entityName + " still wants to fight!");
                            stayInCombat = true;
                        }
                    }
                    
                    StartCoroutine(entity.TakeTurn());
                    
                    while (!entity.eTFlag) yield return null;
                    
                    endPlayerTurnFlag = false;
                    playerTurnFlag = false; 
                    if (!stayInCombat) break;
                }

                if (!stayInCombat)
                {
                    EntityManager.turnBasedEngaged = false;
                    EntityManager.turnBasedStart = false;
                    Debug.Log("Exiting Turn based combat mode!");
                    break;
                }
                
                yield return null;
            }
            
            cachedMouseTooltip.DestroyTooltip(21);
            cachedMouseTooltip.gameObject.SetActive(false);
            yield break;
        }
    }
}
