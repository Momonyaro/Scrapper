using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Pathfinding
{
    public class PolyNavObstacle : MonoBehaviour
    {
        public PolygonCollider2D polyCollider;

        private void OnValidate()
        {
            polyCollider = GetComponent<PolygonCollider2D>();
        }
    }
}
