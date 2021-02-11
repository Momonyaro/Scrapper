using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using Scrapper.Managers;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;

namespace Scrapper.Animation
{
    public class Animator : MonoBehaviour
    {
        public Animation.BranchFacing currentFacing = Animation.BranchFacing.S;
        [HideInInspector] public int currentAnimIndex = 0;
        public List<AnimationBlock> animations;
        public SpriteRenderer sprRenderer;
        private bool _hasAudioEmitter = false;
        private bool _hasDeathEmitter = false;
        public StudioEventEmitter EventEmitter = null;
        public StudioEventEmitter DeathEmitter = null;

        private void Awake()
        {
            if (EventEmitter != null)
                _hasAudioEmitter = true;

            if (DeathEmitter != null)
                _hasDeathEmitter = true;
        }

        public bool PlayAnimFromKeyword(string key, int frameOffset = 0)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].key != key) continue;

                currentAnimIndex = i;
                animations[currentAnimIndex].animation.ResetBranchIndices();
            }
            return false;
        }

        private void Update()
        {
            if (sprRenderer == null) return;

            sprRenderer.sprite = animations[currentAnimIndex].animation.GetFrameOfCurrentBranch(currentFacing);
            if (animations[currentAnimIndex].animation.loopFrame && !animations[currentAnimIndex].animation.loopAnim)
            {
                PlayAnimFromKeyword(animations[currentAnimIndex].animation.transitionTo);
                return;
            }

            if (animations[currentAnimIndex].animation.newFrame || animations[currentAnimIndex].animation.firstLoop)
            {
                //Debug.Log("Parsing anim actions for " + animations[currentAnimIndex].key);
                //Check if we have a logic or audio actions to parse.
                for (int i = 0; i < animations[currentAnimIndex].animation.currentLogicActions.Count; i++)
                {
                    ParseAnimLogicAction(animations[currentAnimIndex].animation.currentLogicActions[i]);   
                }
                for (int i = 0; i < animations[currentAnimIndex].animation.currentAudioActions.Count; i++)
                {
                    ParseAnimAudioAction(animations[currentAnimIndex].animation.currentAudioActions[i]);   
                }
            }
            
            //Based on the current animation's newFrame flag we can decide to execute frame-logic when it's first displayed!
        }

        private void ParseAnimLogicAction(string action)
        {
            switch (action)
            {
                case "dmgFrame":
                {
                    Debug.Log("Parsing attack");
                    CombatManager.AttackTarget(CombatManager.target);
                    break;
                }
            }
        }

        private void ParseAnimAudioAction(string action)
        {
            if (!_hasAudioEmitter) return;
            //EventEmitter.Event = null;
            //EventEmitter.Stop();
            //EventEmitter.StopInstance();
            
            switch (action) //Perhaps revamp later depending on audio manager implementation!
            {
                case "playPunch":
                {
                    if (AudioManager.events["event:/SFX/Weapons/Melee"].getPath(out var path) == RESULT.OK)
                    {
                        EventEmitter.Event = path;
                        EventEmitter.Play();
                    }
                    break;
                }
                case "playHit":
                {
                    if (AudioManager.events["event:/SFX/Characters/Hurt"].getPath(out var path) == RESULT.OK)
                    {
                        EventEmitter.Event = path;
                        EventEmitter.Play();
                    }
                    break;
                }
                case "playDeath":
                {
                    if (!_hasDeathEmitter) return; 
                    DeathEmitter.Play();
                    break;
                }
            }
        }
    }

    [System.Serializable]
    public struct AnimationBlock
    {
        public string key;
        public Animation animation;

        public AnimationBlock(string key, Animation animation)
        {
            this.key = key;
            this.animation = animation;
        }
    }
}
