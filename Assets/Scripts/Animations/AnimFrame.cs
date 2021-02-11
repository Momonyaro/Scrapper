using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Animation
{
    [System.Serializable]
    public class AnimFrame
    {
        public float frameDuration = 1;
        private float currentFrameDuration = 0;
        public List<string> audioActions = new List<string>();
        public List<string> logicActions = new List<string>();
        [SerializeField] private Sprite frameSprite;

        public bool TickFrame(float deltaTime)
        {
            currentFrameDuration += deltaTime;
            if (currentFrameDuration >= frameDuration)
            {
                currentFrameDuration = 0;
                return false;
            }

            return true;
        }

        public Sprite GetSprite()
        {
            return frameSprite;
        }

        public Sprite SetActiveFrame()
        {
            currentFrameDuration = 0;
            
            return frameSprite;
        }

        public void SetSpriteNoFuss(Sprite newSprite)
        {
            frameSprite = newSprite;
        }
    }
}
