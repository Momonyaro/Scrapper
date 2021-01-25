using System.Collections;
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

        public int currentFrame = 0;
        public WeaponOrdering weaponOrdering;
        public List<AnimFrame> frames;

        public Sprite TickFrames(float deltaTime, out bool newFrame, out bool loopFrame)
        {
            if (!frames[currentFrame].TickFrame(deltaTime))
            {
                currentFrame++;
                if (currentFrame >= frames.Count)
                {
                    currentFrame = 0;
                    loopFrame = true;
                }
                else
                    loopFrame = false;
                newFrame = true;
                return frames[currentFrame].SetActiveFrame();
            }

            loopFrame = false;
            newFrame = false;
            return frames[currentFrame].GetSprite();
        }
        
    }
}
