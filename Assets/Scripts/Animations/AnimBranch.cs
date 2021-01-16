﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Animation
{
    [System.Serializable]
    public class AnimBranch
    {
        public enum WeaponOrdering
        {
            Weapons_Front,
            Weapons_Back
        }

        private int currentFrame = 0;
        public WeaponOrdering weaponOrdering;
        public List<AnimFrame> frames;

        public Sprite TickFrames(float deltaTime)
        {
            if (!frames[currentFrame].TickFrame(deltaTime))
            {
                currentFrame++;
                if (currentFrame >= frames.Count)
                    currentFrame = 0;
                return frames[currentFrame].SetActiveFrame();
            }

            return frames[currentFrame].GetSprite();
        }
        
    }
}