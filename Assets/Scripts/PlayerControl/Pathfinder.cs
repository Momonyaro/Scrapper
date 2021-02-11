using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Scrapper.Entities;
using Scrapper.Managers;
using UnityEngine;
using Scrapper.Pathfinding;
using Srapper.Interaction;
using UnityEngine.EventSystems;
using Animation = Scrapper.Animation.Animation;
using Animator = Scrapper.Animation.Animator;

public class Pathfinder : MonoBehaviour
{
    public bool drawDebugPath = false;
    public bool enableController = true;
    public bool playerControlled = false;
    public bool useLineRenderer = false;
    public Animator characterAnimator;
    public Entity pathingEntity;
    public UnityEngine.LineRenderer lineRenderer;
    public UnityEngine.LineRenderer dottedLineRenderer;
    public float playerSpeed = 2.5f;
    public float minNodeDistance = 0.2f;
    private bool haltAllMovement = false;
    private Vector2 down = Vector2.down;
    private float currentAngle = 0;
    private float cachedAngle = 0;
    private bool stopShort = false;
    private float stopShortDist = 1.5f;
    private Vector2 magicZeroVector = new Vector2(0.578f, -1);
    public List<Vector2> currentPath = new List<Vector2>();


    private void Update()
    {
        if (!enableController) return;

        if (playerControlled)
        {
            if (!CombatManager.playerCombatMode && currentPath.Count == 0 && Input.GetKeyDown(KeyCode.A))
            {
                CombatManager.playerCombatMode = true;
            }
            
            if (CombatManager.playerCombatMode)
            {
                if (currentPath.Count > 0) currentPath.Clear();
                lineRenderer.positionCount = 0;
            
                if (Input.GetMouseButtonDown(1))
                {
                    CombatManager.playerCombatMode = false;
                }
                else if (Input.GetMouseButtonDown(0) && CombatManager.target != null && !CombatManager.outOfReach)
                {
                    CombatManager.attacker = pathingEntity;
                    if (!EntityManager.turnBasedEngaged || CombatManager.GetEntityWeapon(pathingEntity).apCost <= pathingEntity.actionPts[0])
                        characterAnimator.PlayAnimFromKeyword(CombatManager.GetEntityWeapon(pathingEntity).itemCombatAnim);
                }

                //return;
            }
            
            if (!CombatManager.playerCombatMode && Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIElement())
                {
                    if (EventSystem.current.currentSelectedGameObject != null)
                    {
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<PolygonalNavMesh>() == null)
                            return;
                    }
                }
                else
                    return;
            
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, -10));
                // Casts the ray and get the first game object hit
                RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);

                if (hit2D.collider != null)
                {
                    Vector2[] path = new Vector2[0];
                    if (hit2D.collider.GetComponent<HoverOverEntity>() != null)
                    {
                        if (pathingEntity.hTFlag)
                        {
                            CombatManager.attacker = pathingEntity;
                            if (!EntityManager.turnBasedEngaged || CombatManager.GetEntityWeapon(pathingEntity).apCost <= pathingEntity.actionPts[0])
                                characterAnimator.PlayAnimFromKeyword(CombatManager.GetEntityWeapon(pathingEntity).itemCombatAnim);
                        }
                        else
                        {
                            path = FindObjectOfType<PolygonalNavMesh>().GetShortestPath(transform.position, hit2D.collider.transform.parent.position);
                            stopShort = true;
                        }
                    }
                    else if (hit2D.collider.GetComponent<PolygonCollider2D>() == null) return;
                    else if (hit2D.collider.GetComponent<PolygonalNavMesh>() != null)
                    {
                    
                        PolygonalNavMesh polyMesh = hit2D.collider.GetComponent<PolygonalNavMesh>();
                        path = polyMesh.GetShortestPath(transform.position, hit2D.point);
                        stopShort = false;
                    }

                    if (path.Length == 0) return; //It's an empty path, why use it?
                    
                    if (pathingEntity.hTFlag)
                    {
                        float distance = 0;
                        for (int i = 1; i < path.Length; i++)
                        {
                            distance += Vector2.Distance(path[i - 1], path[i]);
                        }

                        int cost = Mathf.FloorToInt(distance / 2f) + 1;
                        if (cost > pathingEntity.actionPts[0])
                            return; //Don't move, it's too expensive!

                        pathingEntity.actionPts[0] -= cost;
                    }
                    
                    //Set Movement end point to hit.point and calculate path
                    currentPath.Clear();
                    StopCoroutine("FollowPath");
                    StartCoroutine(nameof(FollowPath), path);
                }
            }

            dottedLineRenderer.positionCount = 0;
            if (pathingEntity.hTFlag)
            {
                //We need to get the path to the mouse position if it's touches the ground and also display tooltip
                if (!IsPointerOverUIElement())
                {
                    if (EventSystem.current.currentSelectedGameObject != null)
                    {
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<PolygonalNavMesh>() == null)
                        {
                            pathingEntity._hoverOverEntity.cachedMouseTooltip.DestroyTooltip(21);
                            return;
                        }
                    }
                }
                else
                {
                    if (!PlayerStatusHUD.drawingTooltip)
                        pathingEntity._hoverOverEntity.cachedMouseTooltip.DestroyTooltip(21);
                    return;
                }
            
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, -10));
                // Casts the ray and get the first game object hit
                RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);

                if (hit2D.collider != null)
                {
                    Vector2[] path = new Vector2[0];
                    bool drawEntityTooltip = false;
                    if (hit2D.collider.GetComponent<HoverOverEntity>() != null)
                    {
                        //Ignore the tooltip since hover over entity should show it's instead
                        path = FindObjectOfType<PolygonalNavMesh>().GetShortestPath(transform.position, hit2D.collider.transform.parent.position);
                        if (hit2D.collider.GetComponent<HoverOverEntity>().entityComponent.entityID != this.pathingEntity.entityID)
                            drawEntityTooltip = true;
                    }
                    else if (hit2D.collider.GetComponent<PolygonCollider2D>() == null) return;
                    else if (hit2D.collider.GetComponent<PolygonalNavMesh>() != null)
                    {
                        PolygonalNavMesh polyMesh = hit2D.collider.GetComponent<PolygonalNavMesh>();
                        path = polyMesh.GetShortestPath(transform.position, hit2D.point);
                    }

                    if (path.Length == 0) return; //It's an empty path, why use it?

                    float distance = 0;
                    for (int i = 1; i < path.Length; i++)
                    {
                        distance += Vector2.Distance(path[i - 1], path[i]);
                    }
                    int cost = Mathf.FloorToInt(distance / 2f) + 1;

                    

                    if (currentPath.Count == 0)
                    {
                    
                        string costTitle = "AP Cost: ";
                        if (!drawEntityTooltip)
                        {
                            costTitle += (cost > pathingEntity.actionPts[0]) ? "<color=red>Too Expensive!</color>" : cost.ToString();
                            
                            pathingEntity._hoverOverEntity.cachedMouseTooltip.CreateTooltip(21, costTitle, 
                                "distance: " + distance.ToString("F1") + "m");
                        }
                        else
                        {
                            Entity entityComponent = hit2D.collider.GetComponent<Entity>();
                            string title = entityComponent.entityName;
                            string content = "";
                            if (entityComponent.entityAltTitle.Length > 0) content += entityComponent.entityAltTitle + "\n";
                            content += "\n" + costTitle;
                            int wpnCost = CombatManager.GetEntityWeapon(pathingEntity).apCost;
                            content += (CombatManager.playerCombatMode && wpnCost > pathingEntity.actionPts[0]) 
                                ? "<color=red>Not Enough AP!</color>" 
                                : wpnCost.ToString();
                            CombatManager.DistanceToPlayer(entityComponent.transform.parent.position);
                            if (CombatManager.playerCombatMode)
                            {
                                if (FindObjectOfType<PolygonalNavMesh>()
                                    .InLineOfSight(CombatManager.playerEntity.transform.parent.position, entityComponent.transform.parent.position))
                                {
                                    if (CombatManager.outOfReach)
                                    {
                                        content +=  "\n <color=red>Out of Reach!</color>";
                                    }
                                    else
                                        content +=  "\n" + distance.ToString("F1") + "m";
                                }
                                else 
                                    content +=  "\n <color=red>Can't see the Target</color>";
                            }
                            else
                                content +=  "\n" + distance.ToString("F1") + "m";
                            content += "\n" + entityComponent.GetHealthPercentageStatus();
                        
                            pathingEntity._hoverOverEntity.cachedMouseTooltip.CreateTooltip(21, title, content);
                        }
                        
                        List<Vector3> vec3Casted = new List<Vector3>();
                        for (int i = 0; i < path.Length; i++)
                        {
                            vec3Casted.Add(path[i]);
                        }
                        dottedLineRenderer.positionCount = path.Length;
                        dottedLineRenderer.SetPositions(vec3Casted.ToArray());
                    }
                    
                }
            }
        }

        if (Mathf.Abs(currentAngle - cachedAngle) > 1f)
        {
            //Check if we should rotate the player sprite
            //Debug.Log(currentAngle);
            RotatePlayerFacing();
            cachedAngle = currentAngle;
        }
    }

    private IEnumerator FollowPath(Vector2[] path)
    {
        currentPath = path.ToList();
        characterAnimator.PlayAnimFromKeyword("_run");
        while (currentPath.Count > 0) //While we have not reached the last
        {
            if (haltAllMovement) yield break;

            if (currentPath.Count == 1 && stopShort)
            {
                if (Vector2.Distance(transform.position, currentPath[0]) > stopShortDist)
                {
                    transform.position = Vector2.MoveTowards(transform.position, currentPath[0], playerSpeed * Time.deltaTime);
                    transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
                }
                else
                {
                    currentPath.RemoveAt(0);
                    stopShort = false;
                    
                    //We should trigger NPC interactions here since we've reached the NPC target if we're not aggro.
                }
            }
            else if (Vector2.Distance(transform.position, currentPath[0]) > minNodeDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, currentPath[0], playerSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
            }
            else
            {
                currentPath.RemoveAt(0);
                if (currentPath.Count > 0)
                {
                    Vector2 targetDir = currentPath[0] - new Vector2(transform.position.x, transform.position.y);
                    currentAngle = AngleBetweenVector2(magicZeroVector, targetDir); //Give it a direction that results in 30deg being the new "0"
                }
            }

            if (useLineRenderer)
            {
                List<Vector3> vec3Casted = new List<Vector3>() { transform.position };
                for (int i = 0; i < currentPath.Count; i++)
                {
                    vec3Casted.Add(currentPath[i]);
                }

                lineRenderer.positionCount = currentPath.Count + 1;
                lineRenderer.SetPositions(vec3Casted.ToArray());
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

    private void OnDrawGizmos()
    {
        if (drawDebugPath)
        {
            if (currentPath.Count > 0)
            {
                Gizmos.DrawLine(transform.position, currentPath[0]);
                
                Gizmos.DrawLine(currentPath[currentPath.Count - 1] + new Vector2(-0.5f, -0.5f), currentPath[currentPath.Count - 1] + new Vector2(0.5f, 0.5f));
                Gizmos.DrawLine(currentPath[currentPath.Count - 1] + new Vector2(-0.5f, 0.5f), currentPath[currentPath.Count - 1] + new Vector2(0.5f, -0.5f));
            }
            
            for (int i = 1; i < currentPath.Count; i++)
            {
                Gizmos.DrawLine(currentPath[i - 1], currentPath[i]);
            }
        }
    }
    
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults )
    {
        for(int index = 0;  index < eventSystemRaysastResults.Count; index ++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position =  Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventData, raysastResults );
        return raysastResults;
    }
}
