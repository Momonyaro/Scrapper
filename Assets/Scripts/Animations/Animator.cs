using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Animation
{
    public class Animator : MonoBehaviour
    {
        public Animation.BranchFacing currentFacing = Animation.BranchFacing.S;
        private int currentAnimIndex = 0;
        public List<AnimationBlock> animations;
        public SpriteRenderer renderer;

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
            if (renderer == null) return;

            renderer.sprite = animations[currentAnimIndex].animation.GetFrameOfCurrentBranch(currentFacing);
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
