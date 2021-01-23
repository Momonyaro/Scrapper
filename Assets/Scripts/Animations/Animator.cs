using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Animation
{
    public class Animator : MonoBehaviour
    {
        public Animation.BranchFacing currentFacing = Animation.BranchFacing.S;
        [HideInInspector] public int currentAnimIndex = 0;
        public List<AnimationBlock> animations;
        public SpriteRenderer sprRenderer;

        public bool PlayAnimFromKeyword(string key, int frameOffset = 0)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].key != key) continue;

                currentAnimIndex = i;
            }
            return false;
        }

        private void Update()
        {
            if (sprRenderer == null) return;

            sprRenderer.sprite = animations[currentAnimIndex].animation.GetFrameOfCurrentBranch(currentFacing);
            if (animations[currentAnimIndex].animation.newFrame && !animations[currentAnimIndex].animation.loopAnim)
            {
                PlayAnimFromKeyword(animations[currentAnimIndex].animation.transitionTo);
                sprRenderer.sprite = animations[currentAnimIndex].animation.GetFrameOfCurrentBranch(currentFacing);
            }
            
            //Based on the current animation's newFrame flag we can decide to execute frame-logic when it's first displayed!
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
