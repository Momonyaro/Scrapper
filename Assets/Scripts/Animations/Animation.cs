using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Animation
{
    [CreateAssetMenu(fileName = "Animation", menuName = "Scrapper/Animation", order = 0)]
    public class Animation : ScriptableObject
    {
        public enum BranchFacing
        {
            S, SE, E, NE, N, NW, W, SW 
        }
        
        public bool displayWeapons;
        public bool newFrame = false;
        public bool loopFrame = false;
        public bool loopAnim = true;
        public string transitionTo = "";
        public BranchStruct[] branches = new BranchStruct[8]
        {
            new BranchStruct(new AnimBranch(), BranchFacing.S), 
            new BranchStruct(new AnimBranch(), BranchFacing.SE), 
            new BranchStruct(new AnimBranch(), BranchFacing.E), 
            new BranchStruct(new AnimBranch(), BranchFacing.NE), 
            new BranchStruct(new AnimBranch(), BranchFacing.N), 
            new BranchStruct(new AnimBranch(), BranchFacing.NW), 
            new BranchStruct(new AnimBranch(), BranchFacing.W), 
            new BranchStruct(new AnimBranch(), BranchFacing.SW) 
        };

        public Sprite GetFrameOfCurrentBranch(BranchFacing facing)
        {
            for (int i = 0; i < branches.Length; i++)
            {
                if (branches[i].GetFacing() != facing) continue;

                return branches[i].GetBranch().TickFrames(Time.deltaTime, out newFrame, out loopFrame);
            }
            
            return branches[0].GetBranch().TickFrames(Time.deltaTime, out newFrame, out loopFrame);
        }
        
        
    }

    [System.Serializable]
    public struct BranchStruct
    {
        public string name;
        [SerializeField] private Animation.BranchFacing facing;
        [SerializeField] private AnimBranch branch;

        public BranchStruct(AnimBranch branch ,Animation.BranchFacing facing)
        {
            this.facing = facing;
            this.branch = branch;
            name = facing.ToString();
        }

        public Animation.BranchFacing GetFacing()
        {
            return facing;
        }

        public AnimBranch GetBranch()
        {
            return branch;
        }

        public void SetBranch(AnimBranch branch)
        {
            this.branch = branch;
        }
    }
}
