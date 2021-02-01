using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Entities;
using Scrapper.Managers;
using Scrapper.Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Srapper.Interaction
{
    public class HoverOverEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float lerpSpeed = 0.8f;
        public Entity entityComponent;
        private Color ambientColor;
        private readonly Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        private readonly Color aggroColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] public bool activeHover = false;
        [SerializeField] public bool debugAggroHover = false;
        [SerializeField] public bool disableTooltipForEntity = false;
        private MouseHoverTooltip cachedMouseTooltip;
        
        [SerializeField] private SpriteRenderer circleRenderer;

        //When hovering over an entity, the circle beneath them should become white (perhaps pulse slightly)
        //If you stop hovering it should then fade back to the ambientColor that's decided in the editor.
        
        private void Awake()
        {
            ambientColor = circleRenderer.color;
            cachedMouseTooltip = FindObjectOfType<MouseHoverTooltip>();
        }

        private void FixedUpdate()
        {
            // Implement this later when combat is implemented, this should only be enabled when this NPC is aggro on the player
            // or when the player is hovering over this NPC in combatMode.
            if (debugAggroHover)
            {
                if (circleRenderer.color != aggroColor)
                    circleRenderer.color = aggroColor;
            
                float colorSin = Mathf.Abs(Mathf.Sin(Time.time * lerpSpeed)) * 0.5f;
                colorSin += 0.5f; //This should be a range from 0.5 to 1.0;
            
                circleRenderer.color = circleRenderer.color * new Color(1, 1, 1, colorSin);
            }
            else if (activeHover)
            {
                if (circleRenderer.color != activeColor)
                    circleRenderer.color = activeColor;

                float colorSin = Mathf.Abs(Mathf.Sin(Time.time * lerpSpeed)) * 0.5f;
                colorSin += 0.5f; //This should be a range from 0.5 to 1.0;

                circleRenderer.color = circleRenderer.color * new Color(1, 1, 1, colorSin);
            }
            else if (circleRenderer.color != ambientColor)
                circleRenderer.color = ambientColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //GetComponent<Entity>().EntityTakeDamage(1); // Debug entities taking damage onHover
            
            
            activeHover = true;
            string title = entityComponent.entityName;
            string content = "";
            if (entityComponent.entityAltTitle.Length > 0) content += entityComponent.entityAltTitle + "\n";
            content += "\n" + entityComponent.GetHealthPercentageStatus();

            if (!disableTooltipForEntity)
            {
                // Create an alternate tooltip for when the player has toggled combatMode
                if (CombatManager.playerCombatMode && entityComponent.healthPts[0] != 0)
                {
                    CombatManager.target = this.entityComponent;
                    debugAggroHover = true;
                    float distance = CombatManager.DistanceToPlayer(transform.parent.position);
                    content = "";
                    if (FindObjectOfType<PolygonalNavMesh>()
                        .InLineOfSight(CombatManager.playerEntity.transform.parent.position, transform.parent.position))
                    {
                        if (CombatManager.outOfReach)
                        {
                            content +=  "\n <color=red>Out of Reach!</color>";
                        }
                        else
                            content +=  "\n " + distance + "m";
                    }
                    else 
                        content +=  "\n <color=red>Can't see the Target</color>";
                    //Insert AP cost here.
                    content += "\n " + entityComponent.GetHealthPercentageStatus();
                }
                
                cachedMouseTooltip.CreateTooltip(title, content);
                cachedMouseTooltip.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            debugAggroHover = false;
            cachedMouseTooltip.DestroyTooltip();
            activeHover = false;
        }
    }
}
