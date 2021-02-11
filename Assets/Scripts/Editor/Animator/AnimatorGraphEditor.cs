using System.Collections;
using System.Collections.Generic;
using Scrapper.Animation;
using UnityEditor;
using UnityEditorInternal;
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

            if (GUILayout.Button("Save Animation"))
            {
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.BeginHorizontal();
            currentEditorFacing =
                (Animation.Animation.BranchFacing) EditorGUILayout.EnumPopup("Current Editor Facing", currentEditorFacing);
            
            currentAnim.displayWeapons = EditorGUILayout.Toggle("Display Weapons", currentAnim.displayWeapons);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            currentAnim.loopAnim = EditorGUILayout.Toggle("Loop Animation", currentAnim.loopAnim);
            if (!currentAnim.loopAnim)
            {
                currentAnim.transitionTo = EditorGUILayout.TextField("Transition To", currentAnim.transitionTo);
            }
            EditorGUILayout.EndHorizontal();

            if (currentAnim.branches.Length < 8)
            {
                currentAnim.branches = new BranchStruct[8]
                {
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.S), 
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.SE), 
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.E), 
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.NE), 
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.N), 
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.NW), 
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.W), 
                    new BranchStruct(new AnimBranch(), Animation.Animation.BranchFacing.SW) 
                };
            }

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

            if (currentBranch.frames == null)
                currentBranch.frames = new List<AnimFrame>();
            
            for (int i = 0; i < currentBranch.frames.Count; i++)
            {
                currentBranch.frames[i] = DrawFrameObj(currentBranch.frames[i], i);
            }

            for (int i = 0; i < currentBranch.frames.Count; i++)
            {
                for (int j = 0; j < currentAnim.branches.Length; j++)
                {
                    if (currentAnim.branches[j].GetBranch() == null) currentAnim.branches[j].SetBranch(new AnimBranch() {frames = new List<AnimFrame>()});
                    if (currentAnim.branches[j].GetBranch().frames == null) currentAnim.branches[j].SetBranch(new AnimBranch() {frames = new List<AnimFrame>()});
                    if (currentAnim.branches[j].GetBranch().frames.Count < currentBranch.frames.Count)
                        currentAnim.branches[j].SetBranch(new AnimBranch()
                        {
                            frames = new List<AnimFrame>(currentBranch.frames.Count)
                            {
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f},
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f},
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f},
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f},
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f},
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f},
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f},
                                new AnimFrame() {audioActions = new List<string>(), logicActions = new List<string>(), frameDuration = 0.2f}
                            }
                        });
                    currentAnim.branches[j].GetBranch().frames[i].frameDuration = currentBranch.frames[i].frameDuration;
                    currentAnim.branches[j].GetBranch().frames[i].audioActions = currentBranch.frames[i].audioActions;
                    currentAnim.branches[j].GetBranch().frames[i].logicActions = currentBranch.frames[i].logicActions;
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
                    if (branch == null) branch = new AnimBranch() {frames = new List<AnimFrame>()};
                    if (branch.frames == null) branch.frames = new List<AnimFrame>(currentAnim.branches[0].GetBranch().frames);
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
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.BeginHorizontal();
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
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.BeginVertical(new GUIStyle() {fixedWidth = EditorGUIUtility.currentViewWidth / 2.2f}); //audioActions
            for (int i = 0; i < frame.audioActions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                frame.audioActions[i] = EditorGUILayout.TextField(frame.audioActions[i]);
                if (GUILayout.Button("Delete"))
                {
                    frame.audioActions.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add AudioAction"))
            {
                frame.audioActions.Add("New AudioAction");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(new GUIStyle() {fixedWidth = EditorGUIUtility.currentViewWidth / 2.2f}); //logicActions
            for (int i = 0; i < frame.logicActions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                frame.logicActions[i] = EditorGUILayout.TextField(frame.logicActions[i]);
                if (GUILayout.Button("Delete"))
                {
                    frame.logicActions.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add LogicAction"))
            {
                frame.logicActions.Add("New LogicAction");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return frame;
        }
    }

    [CustomEditor(typeof(Animator))]
    public class AnimatorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Animation.Animator currentAnim = (Animation.Animator) target;

            if (GUILayout.Button("Set Correct Animation Rotation"))
            {
                currentAnim.PlayAnimFromKeyword(currentAnim.animations[currentAnim.currentAnimIndex].key, 0);
                currentAnim.sprRenderer.sprite = currentAnim.animations[currentAnim.currentAnimIndex].animation.GetFrameOfCurrentBranch(currentAnim.currentFacing);
            }
            
            base.OnInspectorGUI();
        }
    }
}
