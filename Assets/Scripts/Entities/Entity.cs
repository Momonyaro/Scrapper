using System;
using System.Collections;
using System.Collections.Generic;
using Srapper.Interaction;
using UnityEngine;
using Animator = Scrapper.Animation.Animator;

namespace Scrapper.Entities
{
    public class Entity : MonoBehaviour
    {
        public string entityName;
        public string entityAltTitle;
        public int[] healthPts = new int[2]; // [0] = Current HP, [1] = Max HP
        public int[] actionPts = new int[2]; // [0] = Current AP, [1] = Max AP
        public int[] expPts    = new int[2]; // [0] = current XP, [1] = Current Level
        public Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"Guns",      0},
            {"Tech",      0},
            {"Strength",  0},
            {"Dexterity", 0},
            {"Endurance", 0},
            {"Charisma",  0},
            {"Luck",      0},
        };

        private Pathfinder _pathfinder;
        private bool _hasPathfinder = false;
        
        private Animator _animator;
        private bool _hasAnimator = false;
        
        private HoverOverEntity _hoverOverEntity;
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
        }

        private void Start()
        {
            if (healthPts[0] <= 0)
                EntityDeath();
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
            _animator.PlayAnimFromKeyword("_dead");
            _hoverOverEntity.activeHover = false;
            _pathfinder.enableController = false;
        }

        public void EntityTakeDamage(int damage)
        {
            healthPts[0] -= damage;
            
            if (healthPts[0] <= 0)
                EntityDeath();
            else
            {
                if (_hasAnimator) _animator.PlayAnimFromKeyword("_hit");
            }
        }
        
        //
    }
}
