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

        private IEnumerator TurnBasedLoop()
        {
            while (turnBasedEngaged)
            {
                bool stayInCombat = false;
                Debug.Log(EntityManager.Entities.Count);
                for (int i = 0; i < EntityManager.Entities.Count; i++)
                {
                    StartCoroutine(EntityManager.Entities[i].TakeTurn());
                    while (!EntityManager.Entities[i].eTFlag) yield return null;
                    if (EntityManager.Entities[i].sICFlag) stayInCombat = true;
                }

                yield return null;

                if (!stayInCombat)
                {
                    
                }
            }
            yield break;
        }
    }
}
