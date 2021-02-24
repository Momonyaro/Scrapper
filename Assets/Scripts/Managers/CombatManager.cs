using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Entities;
using Scrapper.Items;
using UnityEngine;
using UnityEngine.UI;
using Animator = Scrapper.Animation.Animator;

namespace Scrapper.Managers
{
    public class CombatManager : MonoBehaviour
    {
        public static bool playerCombatMode = false;
        public Entity player;
        public static Entity playerEntity;
        public Color borderColor;
        public Image combatBorder;
        public Item fallbackWeapon; // Thee olde fistecuffs aye'?
        public static Item sFallbackWeapon;
        public static bool outOfReach = false;
        public static Entity attacker;
        public static Entity target;

        private void Awake()
        {
            CombatManager.playerEntity = player;
            borderColor = combatBorder.color;

            CombatManager.sFallbackWeapon = fallbackWeapon;
        }

        private void Update()
        {
            combatBorder.gameObject.SetActive(CombatManager.playerCombatMode);
            combatBorder.color = borderColor * Mathf.Clamp(Mathf.Sin(Time.time), 0.7f, 1.0f);
        }

        public static float DistanceToTarget()
        {
            
            float distance = Vector2.Distance(CombatManager.attacker.transform.position,
                                              CombatManager.target.transform.position);

            CombatManager.outOfReach = (distance >= GetEntityWeapon(attacker).maxReach);

            return distance;
        }
        
        public static float DistanceToPlayer(Vector2 position)
        {
            Vector2 playerPos = playerEntity.transform.position;
            
            float distance = Vector2.Distance(playerPos, position);

            CombatManager.outOfReach = (distance >= GetEntityWeapon(playerEntity).maxReach);

            return distance;
        }

        public static void AttackTarget(Entity target)
        {
            CombatManager.target = target;
            if (outOfReach) return;
            if (CombatManager.target == null) return;
            if (CombatManager.target.healthPts[0] == 0) return;

            //Unarmed damage for now is just item damage * entityStr + 1.
            Item playerWeapon = GetEntityWeapon(CombatManager.attacker);
            
            int damage = (playerWeapon.dam * playerEntity.stats[2]) + 1;
            //Debug.Log("Player str is: " + playerEntity.stats[2]);
            //Debug.Log("Dealt " + damage + " damage to: " + lastTarget.entityName);
            CombatManager.target.EntityTakeDamage(damage);
            EventLog.Instance.Print($"<color=red>{attacker.entityName}</color> dealt {damage} damage to {target.entityName}");
            
            if (EntityManager.turnBasedEngaged)
            {
                attacker.actionPts[0] = Mathf.Clamp(attacker.actionPts[0] - playerWeapon.apCost, 0, attacker.actionPts[1]);
                Debug.Log("Decreased " + attacker.entityName + "'s AP with " + playerWeapon.apCost + ". It is now: " + attacker.actionPts[0]);
            }
            playerCombatMode = false;
        }

        public static Item GetEntityWeapon(Entity entity)
        {
            if (entity.currentWeapon == null)
                return CombatManager.sFallbackWeapon;

            return entity.currentWeapon;
        }

        public void TogglePlayerCombatMode()
        {
            if (playerEntity._pathfinder.currentPath.Count > 0) return;
            //Later we have to check if the player is in combat and if it's their turn.
            CombatManager.playerCombatMode = !CombatManager.playerCombatMode;
        }
    }
}
