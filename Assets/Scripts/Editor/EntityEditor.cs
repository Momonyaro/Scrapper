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

            if (GUILayout.Button("Set Current Values to Max"))
            {
                entity.healthPts[0] = entity.healthPts[1];
                entity.actionPts[0] = entity.actionPts[1];
            }
            
            EditorGUILayout.BeginVertical("HelpBox");
            entity.entityName = EditorGUILayout.TextField("Entity Name: ", entity.entityName);
            entity.entityAltTitle = EditorGUILayout.TextField("Entity Alternate Title: ", entity.entityAltTitle);
            
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

            for (int i = 0; i < entity.stats.Keys.Count; i++)
            {
                string key = entity.stats.ElementAt(i).Key;
                entity.stats[key] = EditorGUILayout.IntField(key, entity.stats.ElementAt(i).Value);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }
    }
}
