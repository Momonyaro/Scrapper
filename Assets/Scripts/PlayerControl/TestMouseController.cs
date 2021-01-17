using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animation = Scrapper.Animation.Animation;
using Animator = Scrapper.Animation.Animator;

public class TestMouseController : MonoBehaviour
{
    public bool enableTestController = true;
    private Vector2 lastMousePos;
    public Animator animator;
    public Transform compass;
    public Transform mouseMagnet;
    
    
    // Update is called once per frame
    void Update()
    {
        if (!enableTestController) return;

        lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetMouseButton(0))
        {
            float currentAngle = 0;

            Vector2 v2CompassPos = new Vector2(compass.position.x, compass.position.y);
            float cos = Vector2.Dot(lastMousePos, v2CompassPos) / (lastMousePos.magnitude * v2CompassPos.magnitude);
            currentAngle = Mathf.Rad2Deg * Mathf.Acos(cos);

            Vector2 dir = new Vector2(0, 0);
            if (compass.position.x < lastMousePos.x) //The mouse is to the left.
                dir.x =  1;
            else
                dir.x = -1;
            if (compass.position.y < lastMousePos.y) //The mouse is to the above.
                dir.y =  1;
            else
                dir.y = -1;
            
            //Debug.Log(currentAngle + ", " + dir);

            if (dir.y < 0)
            {
                if (dir.x > 0)
                {
                    if (currentAngle < 53)
                        animator.currentFacing = Animation.BranchFacing.S;
                    if (currentAngle >= 53)
                        animator.currentFacing = Animation.BranchFacing.SE;
                }
                else
                {
                    if (currentAngle < 50)
                        animator.currentFacing = Animation.BranchFacing.SW;
                    if (currentAngle >= 50)
                        animator.currentFacing = Animation.BranchFacing.W;
                }
            }
            else
            {
                if (dir.x > 0)
                {
                    if (currentAngle < 114)
                        animator.currentFacing = Animation.BranchFacing.E;
                    if (currentAngle >= 114)
                        animator.currentFacing = Animation.BranchFacing.NE;
                }
                else
                {
                    if (currentAngle < 110)
                        animator.currentFacing = Animation.BranchFacing.NW;
                    if (currentAngle >= 110)
                        animator.currentFacing = Animation.BranchFacing.N;
                }
            }
        }

        mouseMagnet.position = lastMousePos;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(compass.position, compass.up * 0.2f);
        Gizmos.DrawRay(compass.position, -compass.up * 0.2f);
        Gizmos.DrawRay(compass.position, compass.right * 0.2f);
        Gizmos.DrawRay(compass.position, -compass.right * 0.2f);
        
        Gizmos.color = Color.blue;
        Vector3 crossed = Vector3.Cross(compass.forward, -compass.right);
        Quaternion rotDeg = Quaternion.Euler(0, 0, 30);
        Vector3 rotatedRight = rotDeg * compass.right;
        Vector3 rotatedCrossed = rotDeg * crossed;
        Gizmos.DrawRay(compass.position,  rotatedCrossed * 0.5f);
        Gizmos.DrawRay(compass.position, -rotatedCrossed * 0.5f);
        Gizmos.DrawRay(compass.position,  rotatedRight * 0.5f);
        Gizmos.DrawRay(compass.position, -rotatedRight * 0.5f);
        
        Gizmos.DrawRay(compass.position, ( rotatedCrossed +  rotatedRight) * 0.3f);
        Gizmos.DrawRay(compass.position, ( rotatedCrossed + -rotatedRight) * 0.3f);
        Gizmos.DrawRay(compass.position, (-rotatedCrossed +  rotatedRight) * 0.3f);
        Gizmos.DrawRay(compass.position, (-rotatedCrossed + -rotatedRight) * 0.3f);
    }
}
