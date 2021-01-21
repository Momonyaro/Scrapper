using System;
using System.Collections;
using System.Collections.Generic;
using Scrapper.Pathfinding;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PolygonalNavMesh))]
public class PolyNavEditor : Editor
{
    private PolygonalNavMesh navMesh;
    
    public override void OnInspectorGUI()
    {
        navMesh = (PolygonalNavMesh) target;

        if (GUILayout.Button("Generate Default Nodes"))
        {
            //Generate a list of 2D vectors to be modified
            navMesh.CreateNavNodes();
        }
        
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        navMesh = (PolygonalNavMesh) target;

        for (int i = 0; i < navMesh.navNodes.Count; i++)
        {
            navMesh.navNodes[i] = Handles.PositionHandle(navMesh.navNodes[i], Quaternion.identity);
        }
    }
}
