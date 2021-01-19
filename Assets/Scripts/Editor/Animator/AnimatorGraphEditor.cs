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
        public int deleteIndex = -1;
        
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
                currentBranch.frames[i] = DrawFrameObj(currentBranch.frames[i], i);
            }

            for (int i = 0; i < currentBranch.frames.Count; i++)
            {
                for (int j = 0; j < currentAnim.branches.Length; j++)
                {
                    currentAnim.branches[j].GetBranch().frames[i].frameDuration = currentBranch.frames[i].frameDuration;
                }
            }

            for (int i = 0; i < currentAnim.branches.Length; i++)
            {
                if (currentAnim.branches[i].GetFacing() == currentEditorFacing)
                {
                    currentAnim.branches[i].SetBranch(currentBranch);
                    break;
                }
            }
            
            if (GUILayout.Button("Add Frame To Branches"))
            {
                for (int i = 0; i < currentAnim.branches.Length; i++)
                {
                    AnimBranch branch = currentAnim.branches[i].GetBranch();
                    branch.frames.Add(new AnimFrame()
                    {
                        frameDuration = 0.2f
                    });
                    currentAnim.branches[i].SetBranch(branch);
                }
            }

            if (deleteIndex >= 0)
            {
                for (int i = 0; i < currentAnim.branches.Length; i++)
                {
                    AnimBranch branch = currentAnim.branches[i].GetBranch();
                    branch.frames.RemoveAt(deleteIndex);
                    currentAnim.branches[i].SetBranch(branch);
                }
                deleteIndex = -1;
            }
            
            EditorGUILayout.EndVertical();
            
            
            //base.OnInspectorGUI();
        }

        private AnimFrame DrawFrameObj(AnimFrame frame, int index)
        {
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.BeginVertical();
            frame.frameDuration = EditorGUILayout.Slider("Frame Duration", frame.frameDuration, 0, 2);
            frame.SetSpriteNoFuss((Sprite)EditorGUILayout.ObjectField("Sprite", frame.GetSprite(), typeof(Sprite), allowSceneObjects: true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Frame"))
            {
                deleteIndex = index;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            return frame;
        }
    }
}
