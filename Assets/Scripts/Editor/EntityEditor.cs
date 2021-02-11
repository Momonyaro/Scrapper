using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scrapper.Entities;
using UnityEditor;
using UnityEngine;

namespace Scrapper.Editor
{
    [CustomEditor(typeof(Entity))]
    public class EntityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Entity entity = (Entity) target;

            if (GUILayout.Button("Set HP Current Value to Max"))
            {
                entity.healthPts[0] = entity.healthPts[1];
            }
            
            EditorGUILayout.BeginVertical("HelpBox");
            entity.entityName = EditorGUILayout.TextField("Entity Name: ", entity.entityName);
            entity.entityAltTitle = EditorGUILayout.TextField("Entity Alternate Title: ", entity.entityAltTitle);
            entity.pPOpinion = EditorGUILayout.IntSlider("Player Opinion",entity.pPOpinion, -100, 100);
            
            EditorGUILayout.BeginHorizontal("HelpBox");
            entity.healthPts[0] = EditorGUILayout.IntField("Current HP:", entity.healthPts[0], GUILayout.MinWidth(150));
            entity.healthPts[1] = EditorGUILayout.IntField("Max HP:", entity.healthPts[1], GUILayout.MinWidth(150));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal("HelpBox");
            entity.actionPts[0] = EditorGUILayout.IntField("Current AP:", entity.actionPts[0], GUILayout.MinWidth(150));
            entity.actionPts[1] = EditorGUILayout.IntField("Max AP:", entity.actionPts[1], GUILayout.MinWidth(150));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal("HelpBox");
            entity.expPts[1] = EditorGUILayout.IntField("Current Level:", entity.expPts[1], GUILayout.MinWidth(150));
            entity.expPts[0] = EditorGUILayout.IntField("XP needed:", entity.expPts[0], GUILayout.MinWidth(150));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("HelpBox");

            entity.stats[0] = EditorGUILayout.IntField("Guns", entity.stats[0]);
            entity.stats[1] = EditorGUILayout.IntField("Tech", entity.stats[1]);
            entity.stats[2] = EditorGUILayout.IntField("Strength", entity.stats[2]);
            entity.stats[3] = EditorGUILayout.IntField("Dexterity", entity.stats[3]);
            entity.stats[4] = EditorGUILayout.IntField("Endurance", entity.stats[4]);
            entity.stats[5] = EditorGUILayout.IntField("Charisma", entity.stats[5]);
            entity.stats[6] = EditorGUILayout.IntField("Luck", entity.stats[6]);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }
    }
}
