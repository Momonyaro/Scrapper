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
    
    
    // Update is called once per frame
    void Update()
    {
        if (!enableTestController) return;

        if (Input.GetMouseButton(0))
        {
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float currentAngle = Vector2.Angle(new Vector2(compass.position.x, compass.position.y), lastMousePos);

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
                    if (currentAngle < 50)
                        animator.currentFacing = Animation.BranchFacing.S;
                    if (currentAngle >= 50)
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
                    if (currentAngle < 105)
                        animator.currentFacing = Animation.BranchFacing.E;
                    if (currentAngle >= 105)
                        animator.currentFacing = Animation.BranchFacing.NE;
                }
                else
                {
                    if (currentAngle < 105)
                        animator.currentFacing = Animation.BranchFacing.NW;
                    if (currentAngle >= 105)
                        animator.currentFacing = Animation.BranchFacing.N;
                }
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 crossed = Vector3.Cross(compass.forward, -compass.right);
        Vector3 rotatedCross = Quaternion.Euler(0, 0, 60) * crossed;
        Vector3 rotatedRight = Quaternion.Euler(0, 0, 30) * compass.right;
        Gizmos.DrawRay(compass.position, rotatedCross);
        Gizmos.DrawRay(compass.position, -rotatedCross);
        Gizmos.DrawRay(compass.position,  rotatedRight);
        Gizmos.DrawRay(compass.position, -rotatedRight);
        
        Gizmos.DrawRay(compass.position, ( rotatedCross +  rotatedRight) * .6f);
        Gizmos.DrawRay(compass.position, (-rotatedCross +  rotatedRight) * .6f);
        Gizmos.DrawRay(compass.position, ( rotatedCross + -rotatedRight) * .6f);
        Gizmos.DrawRay(compass.position, (-rotatedCross + -rotatedRight) * .6f);
        
        Gizmos.DrawLine(compass.position, lastMousePos);
    }
}
