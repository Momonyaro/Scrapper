using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using Scrapper.Pathfinding;
using Animation = Scrapper.Animation.Animation;
using Animator = Scrapper.Animation.Animator;

public class Pathfinder : MonoBehaviour
{
    public bool enableController = true;
    public bool playerControlled = false;
    public Animator characterAnimator;
    public float playerSpeed = 2.5f;
    public float minNodeDistance = 0.2f;
    private bool haltAllMovement = false;
    private Vector2 down = Vector2.down;
    private float currentAngle = 0;
    private float cachedAngle = 0;
    private Vector2 magicZeroVector = new Vector2(0.578f, -1);


    private void Update()
    {
        if (playerControlled && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, -10));
            // Casts the ray and get the first game object hit
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);

            if (hit2D.collider != null)
            {
                if (hit2D.collider.GetComponent<PolygonCollider2D>() == null) return;
                
                PolygonalNavMesh polyMesh = hit2D.collider.GetComponent<PolygonalNavMesh>();
                Debug.Log("Hit the navmesh!");
                
                //Set Movement end point to hit.point and calculate path
                Vector2[] path = polyMesh.GetShortestPath(transform.position, hit2D.point);
                StopCoroutine("FollowPath");
                StartCoroutine(nameof(FollowPath), path);
            }
        }

        if (Mathf.Abs(currentAngle - cachedAngle) > 1f)
        {
            //Check if we should rotate the player sprite
            Debug.Log(currentAngle);
            RotatePlayerFacing();
            cachedAngle = currentAngle;
        }
    }

    private IEnumerator FollowPath(Vector2[] path)
    {
        List<Vector2> pathList = path.ToList();
        characterAnimator.PlayAnimFromKeyword("_run");
        while (pathList.Count > 0) //While we have not reached the last
        {
            if (haltAllMovement) yield break;

            if (Vector2.Distance(transform.position, pathList[0]) > minNodeDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, pathList[0], playerSpeed * Time.deltaTime);
            }
            else
            {
                pathList.RemoveAt(0);
                if (pathList.Count > 0)
                {
                    Vector2 targetDir = pathList[0] - new Vector2(transform.position.x, transform.position.y);
                    currentAngle = AngleBetweenVector2(magicZeroVector, targetDir); //Give it a direction that results in 30deg being the new "0"
                }
            }
            
            yield return new WaitForEndOfFrame();
        }
        characterAnimator.PlayAnimFromKeyword("_idle");
        yield break;
    }
    
    float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
        float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
        return Vector2.Angle(vec1, vec2) * sign;
    }

    private void RotatePlayerFacing()
    {
        //Set the facing based on the currentAngle.
        if (currentAngle > 0)
        {
            if (currentAngle < 22.5f) // S
                characterAnimator.currentFacing = Animation.BranchFacing.S;
            else if (currentAngle < 67.5f) // SE
                characterAnimator.currentFacing = Animation.BranchFacing.SE;
            else if (currentAngle < 112.5f) // E
                characterAnimator.currentFacing = Animation.BranchFacing.E;
            else if (currentAngle < 167.5f) // NE
                characterAnimator.currentFacing = Animation.BranchFacing.NE;
            else 
                characterAnimator.currentFacing = Animation.BranchFacing.N;
        }
        else
        {
            if (currentAngle > -22.5f) // S
                characterAnimator.currentFacing = Animation.BranchFacing.S;
            else if (currentAngle > -67.5f) // SE
                characterAnimator.currentFacing = Animation.BranchFacing.SW;
            else if (currentAngle > -112.5f) // E
                characterAnimator.currentFacing = Animation.BranchFacing.W;
            else if (currentAngle > -167.5f) // NE
                characterAnimator.currentFacing = Animation.BranchFacing.NW;
            else 
                characterAnimator.currentFacing = Animation.BranchFacing.N;
        }
    }
    
}
