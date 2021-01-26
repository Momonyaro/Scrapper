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
        public static Entity lastTarget;

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

        public static float DistanceToPlayer(Vector2 position)
        {
            Vector2 playerPos = playerEntity.transform.position;
            
            float distance = Vector2.Distance(playerPos, position);

            CombatManager.outOfReach = (distance >= GetPlayerWeapon().maxReach);

            return distance;
        }

        public static void AttackLastTarget()
        {
            if (outOfReach) return;
            if (lastTarget == null) return;
            if (lastTarget.healthPts[0] == 0) return;

            //Unarmed damage for now is just item damage * entityStr + 1.
            Item playerWeapon = GetPlayerWeapon();
            int damage = (playerWeapon.dam * playerEntity.stats[2]) + 1;
            //Debug.Log("Player str is: " + playerEntity.stats[2]);
            //Debug.Log("Dealt " + damage + " damage to: " + lastTarget.entityName);
            lastTarget.EntityTakeDamage(damage);

            Animator animator = playerEntity.GetAnimator();
            if (animator != null)
                animator.PlayAnimFromKeyword("_punch");
            
            playerCombatMode = false;
            lastTarget = null;
        }

        public static Item GetPlayerWeapon()
        {
            if (playerEntity.currentWeapon == null)
                return CombatManager.sFallbackWeapon;

            return playerEntity.currentWeapon;
        }

        public void TogglePlayerCombatMode()
        {
            //Later we have to check if the player is in combat and if it's their turn.
            CombatManager.playerCombatMode = !CombatManager.playerCombatMode;
        }
    }
}
