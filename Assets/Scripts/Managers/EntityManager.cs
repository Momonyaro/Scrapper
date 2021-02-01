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

        public float waitTime = 0.15f; //To not just catapult between entities between turns.

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
                    if (EntityManager.Entities[i]._pathfinder.playerControlled) playerTurnFlag = true; 
                    StartCoroutine(EntityManager.Entities[i].TakeTurn());
                    while (!EntityManager.Entities[i].eTFlag) yield return null;
                    for (int j = 0; j < EntityManager.Entities.Count; j++)
                    {
                        if (Entities[j].sICFlag)
                        {
                            Debug.Log(Entities[j].entityName + " still wants to fight!");
                            stayInCombat = true;
                        }
                    }

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
            yield break;
        }
    }
}
