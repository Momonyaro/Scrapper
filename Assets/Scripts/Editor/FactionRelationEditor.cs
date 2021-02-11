using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Factions;
using UnityEditor;
using UnityEngine;

namespace Scrapper.Editor
{
    [CustomEditor(typeof(FactionRelations))]
    public class FactionRelationEditor : UnityEditor.Editor
    {
        private FactionRelations relations;
        
        public override void OnInspectorGUI()
        {
            relations = (FactionRelations) target;
            var so = new SerializedObject(relations);


            if (GUILayout.Button("Save Relation Data"))
            {
                EditorUtility.SetDirty(target); // Save please :(, please!!
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < relations.factions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove Faction"))
                {
                    relations.factions.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
                relations.factions[i] = DrawFaction(relations.factions[i]);
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add Faction"))
            {
                relations.factions.Add(new Faction("New Faction", relations.GenerateGUID()));
            }
            EditorGUILayout.EndVertical(); //AAA

            so.ApplyModifiedProperties();
        }

        public Faction DrawFaction(Faction faction)
        {

            EditorGUILayout.BeginVertical("HelpBox");
            
            EditorGUILayout.BeginVertical("HelpBox");
            faction.factionName = EditorGUILayout.TextField("Faction Title:", faction.factionName);
            EditorGUILayout.BeginHorizontal(new GUIStyle() {padding = new RectOffset(4, 4, 10, 10)});
            if (GUILayout.Button("Generate new GUID"))
                faction.factionID = Guid.NewGuid().ToString();
            GUILayout.Label(faction.factionID);
            EditorGUILayout.EndHorizontal();
            faction.playerPartyOpinion =
                EditorGUILayout.IntSlider("Opinion of Player Party: ", faction.playerPartyOpinion, -100, 100);
            EditorGUILayout.EndVertical();

            faction.folded = EditorGUILayout.Foldout(faction.folded, "Faction Relations:");
            
            if (faction.folded)
            {
                // Here is where it gets difficult...
                for (int i = 0; i < faction.relations.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Remove Faction"))
                    {
                        faction.relations.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    faction.relations[i] = DrawFactionRelComponent(faction.relations[i]);
                }
                if (GUILayout.Button("Add Faction Dynamic"))
                {
                    faction.relations.Add(new FactionRelationComponent(faction.factionID, 0));
                }
            }
            
            EditorGUILayout.EndVertical();
            
            return faction;
        }

        public FactionRelationComponent DrawFactionRelComponent(FactionRelationComponent component)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.BeginHorizontal();
            List<string> factionNames = relations.GetFactionNames("");
            string labelText = "";
            for (int i = 0; i < factionNames.Count; i++)
            {
                if (component.factionID.Equals(relations.GetGUIDFromFactionTitle(factionNames[i])))
                    labelText = factionNames[i];
            }
            GUILayout.Label(labelText);
            component.factionEditorIndex = Mathf.Clamp(component.factionEditorIndex, 0, factionNames.Count);

            int oldIndex = component.factionEditorIndex;
            
            component.factionEditorIndex =
                EditorGUILayout.Popup("Factions:", component.factionEditorIndex, factionNames.ToArray());

            if (component.factionEditorIndex != oldIndex)
            {
                component.factionID = relations.GetGUIDFromFactionTitle(factionNames[component.factionEditorIndex]);
            }
            EditorGUILayout.EndHorizontal();
            component.factionOpinion = EditorGUILayout.IntSlider("Faction Opinion: ", component.factionOpinion, -100, 100);
            EditorGUILayout.EndVertical();
            return component;
        }
    }
}
