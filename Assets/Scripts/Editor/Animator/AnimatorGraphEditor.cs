using System.Collections;
using System.Collections.Generic;
using Scrapper.Animation;
using UnityEditor;
using UnityEngine;
using Animator = Scrapper.Animation.Animator;

namespace Scrapper.Editor
{
    [CustomEditor(typeof(Animation.Animation))]
    public class AnimatorEditor : UnityEditor.Editor
    {
        public Animation.Animation.BranchFacing currentEditorFacing = Animation.Animation.BranchFacing.S;
        
        public override void OnInspectorGUI()
        {
            Animation.Animation currentAnim = (Animation.Animation) target;

            EditorGUILayout.BeginHorizontal();
            currentEditorFacing =
                (Animation.Animation.BranchFacing) EditorGUILayout.EnumPopup("Current Editor Facing", currentEditorFacing);
            
            currentAnim.displayWeapons = EditorGUILayout.Toggle("Display Weapons", currentAnim.displayWeapons);
            EditorGUILayout.EndHorizontal();

            AnimBranch currentBranch = currentAnim.branches[0].GetBranch();

            for (int i = 0; i < currentAnim.branches.Length; i++)
            {
                if (currentAnim.branches[i].GetFacing() == currentEditorFacing)
                {
                    currentBranch = currentAnim.branches[i].GetBranch();
                    break;
                }
            }
            
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            //Draw Anim Branch
            currentBranch.weaponOrdering =
                (AnimBranch.WeaponOrdering) EditorGUILayout.EnumPopup("Weapon Ordering", currentBranch.weaponOrdering);

            for (int i = 0; i < currentBranch.frames.Count; i++)
            {
                currentBranch.frames[i] = DrawFrameObj(currentBranch.frames[i]);
            }
            
            EditorGUILayout.EndVertical();
            
            for (int i = 0; i < currentAnim.branches.Length; i++)
            {
                if (currentAnim.branches[i].GetFacing() == currentEditorFacing)
                {
                    currentAnim.branches[i].SetBranch(currentBranch);
                    break;
                }
            }
            
            //base.OnInspectorGUI();
        }

        private AnimFrame DrawFrameObj(AnimFrame frame)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            frame.frameDuration = EditorGUILayout.Slider("Frame Duration", frame.frameDuration, 0, 2);
            frame.SetSpriteNoFuss((Sprite)EditorGUILayout.ObjectField("Sprite", frame.GetSprite(), typeof(Sprite), allowSceneObjects: true));
            EditorGUILayout.EndVertical();
            return frame;
        }
    }
}
