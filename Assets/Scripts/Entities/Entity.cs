using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Items;
using Scrapper.Managers;
using Srapper.Interaction;
using UnityEngine;
using Animator = Scrapper.Animation.Animator;

namespace Scrapper.Entities
{
    public class Entity : MonoBehaviour
    {
        public string entityName;
        public string entityAltTitle;
        public string entityID = Guid.NewGuid().ToString();
        public int[] healthPts = new int[2]; // [0] = Current HP, [1] = Max HP
        public int[] actionPts = new int[2]; // [0] = Current AP, [1] = Max AP
        public int[] expPts    = new int[2]; // [0] = current XP, [1] = Current Level
        public Item[] inventory = new Item[24];
        public Item currentWeapon;
        public Item currentArmor;
        public int[] stats = new int[]
        {
            0,    // GUNS
            0,    // TECH
            0,    // STRENGTH
            0,    // DEXTERITY
            0,    // ENDURANCE
            0,    // CHARISMA
            0     // LUCK
        };

        public int pPOpinion = 0; // -100 to 100, the NPCs current personal opinion of the player party.

        public bool hTFlag = false; // Has Turn Flag
        public bool eTFlag = false; // End Turn Flag 
        public bool sICFlag = false; // Stay in Combat Flag
            
        public Pathfinder _pathfinder { get; private set; }
        private bool _hasPathfinder = false;
        
        private Animator _animator;
        private bool _hasAnimator = false;
        
        public HoverOverEntity _hoverOverEntity { get; private set; }
        private bool _hasEntityHover = false;

        private void Awake()
        {
            if (transform.parent.GetComponent<Pathfinder>() != null)
            {
                _pathfinder = transform.parent.GetComponent<Pathfinder>();
                _hasPathfinder = true;
            }

            if (GetComponent<HoverOverEntity>() != null)
            {
                _hoverOverEntity = GetComponent<HoverOverEntity>();
                _hasEntityHover = true;
            }

            if (GetComponent<Animator>() != null)
            {
                _animator = GetComponent<Animator>();
                _hasAnimator = true;
            }
            
            EntityManager.Entities.Add(this);
        }

        public Animator GetAnimator()
        {
            if (_hasAnimator)
                return _animator;
            else return null;
        }

        private void Start()
        {
            if (healthPts[0] <= 0)
                EntityDeath();
        }

        public bool CheckForStillInCombat()
        {
            if (pPOpinion < -92) return true;
            if (_hasPathfinder && _pathfinder.playerControlled) return false;

            return false;
        }

        public IEnumerator TakeTurn()
        {
            eTFlag = false;
            sICFlag = false;
            hTFlag = true;
            if (healthPts[0] <= 0) { eTFlag = true; hTFlag = false; yield break;}
            Debug.Log(entityName + " is currently parsing it's turn...");
            actionPts[0] = Mathf.Clamp(actionPts[0] + 4, 0, actionPts[1]);
            CombatManager.attacker = this;

            if (pPOpinion < -92) sICFlag = true;
            else { eTFlag = true; hTFlag = false; yield break;}
            if (_hasPathfinder && _pathfinder.playerControlled) sICFlag = false;
            
            while (!eTFlag)
            {
                if (actionPts[0] <= 0 && _pathfinder.currentPath.Count == 0)
                {
                    eTFlag = true; 
                }

                //Here the AI decides what to do. or if we're
                // player controlled we enable the player controls in economy mode.
                // DEBUG AI SHITTY, PLEASE ACTUALLY MAKE SOME AI
                
                
                if (EntityManager.playerTurnFlag)
                {
                    bool stayInCombat = false;
                    for (int j = 0; j < EntityManager.Entities.Count; j++)
                    {
                        if (EntityManager.Entities[j]._pathfinder.playerControlled) continue;
                        if (EntityManager.Entities[j].CheckForStillInCombat() && EntityManager.Entities[j].healthPts[0] > 0)
                        {
                            stayInCombat = true;
                        }
                    }
                
                    if (EntityManager.endPlayerTurnFlag || !stayInCombat)
                    {
                        eTFlag = true;
                        hTFlag = false;
                        yield break;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.4f);
                    Item weapon = CombatManager.GetEntityWeapon(this);
                    if (weapon.maxReach > Vector2.Distance(_pathfinder.transform.position,
                        CombatManager.playerEntity._pathfinder.transform.position))
                    {
                        if (weapon.apCost <= actionPts[0])
                        {
                            CombatManager.attacker = this;
                            CombatManager.target = CombatManager.playerEntity;
                            _pathfinder.LookAtEntity(CombatManager.target);
                            _animator.PlayAnimFromKeyword(weapon.itemCombatAnim);
                            yield return new WaitForSeconds(1.2f);
                            if (CombatManager.playerEntity.healthPts[0] <= 0) eTFlag = true;
                        }
                        else { eTFlag = true; }
                    }
                    else { eTFlag = true; }
                }
                
                
                yield return null;
            }

            hTFlag = false;
            yield break;
        }

        public string GetHealthPercentageStatus()
        {
            float percentage = (float)healthPts[0] / (float)healthPts[1];
            //Debug.Log(percentage);
            if      (percentage > 0.75f) return "<color=green>Healthy</color>";
            else if (percentage > 0.50f) return "<color=yellow>Minor Injuries</color>";
            else if (percentage > 0.25f) return "<color=orange>Wounded</color>";
            else if (percentage > 0.00f) return "<color=red>Nearly Dead</color>";
            else return "<color=grey>Dead</color>";
        }

        public void EntityDeath()
        {
            healthPts[0] = 0;
            _animator.PlayAnimFromKeyword("_hitdead");
            _hoverOverEntity.activeHover = false;
            _pathfinder.enableController = false;
        }

        public void EntityTakeDamage(int damage)
        {
            healthPts[0] -= damage;
            pPOpinion = Mathf.Clamp(pPOpinion - 60, -100, 100);
            
            if (healthPts[0] <= 0)
                EntityDeath();
            else
            {
                if (pPOpinion < -92 && !EntityManager.turnBasedEngaged) EntityManager.ActivateTurnBased();
                if (_hasAnimator) _animator.PlayAnimFromKeyword("_hit");
            }
        }
        
        //
    }
}
