﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Srapper.Interaction
{
    public class HoverOverEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float lerpSpeed = 0.8f;
        private Color ambientColor;
        private readonly Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        private readonly Color aggroColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private bool activeHover = false;
        [SerializeField] private bool debugAggroHover = false;
        
        [SerializeField] private SpriteRenderer circleRenderer;

        //When hovering over an entity, the circle beneath them should become white (perhaps pulse slightly)
        //If you stop hovering it should then fade back to the ambientColor that's decided in the editor.
        
        private void Awake()
        {
            ambientColor = circleRenderer.color;
        }

        private void FixedUpdate()
        {
            // Implement this later when combat is implemented, this should only be enabled when this NPC is aggro on the player
            // or when the player is hovering over this NPC in combatMode.
            // if (debugAggroHover)
            // {
            //     if (circleRenderer.color != aggroColor)
            //         circleRenderer.color = aggroColor;
            //
            //     float colorSin = Mathf.Abs(Mathf.Sin(Time.time * lerpSpeed)) * 0.5f;
            //     colorSin += 0.5f; //This should be a range from 0.5 to 1.0;
            //
            //     circleRenderer.color = circleRenderer.color * new Color(1, 1, 1, colorSin);
            // }
            if (activeHover)
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
            activeHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            activeHover = false;
        }
    }
}