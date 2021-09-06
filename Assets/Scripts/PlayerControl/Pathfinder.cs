using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Scrapper.Entities;
using Scrapper.Items;
using Scrapper.Managers;
using UnityEngine;
using Scrapper.Pathfinding;
using Srapper.Interaction;
using UnityEngine.EventSystems;
using Animation = Scrapper.Animation.Animation;
using Animator = Scrapper.Animation.Animator;
using Object = System.Object;

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
    private bool _haltAllMovement = false;
    private float _currentAngle = 0;
    private float _cachedAngle = 0;
    private bool _stopShort = false;
    private float _stopShortDist = 1.5f;
    private PolygonalNavMesh _navMesh;
    private readonly Vector2 _magicZeroVector = new Vector2(0.578f, -1);
    private bool _drawEntityToolTip = false;
    
    public List<Vector2> currentPath = new List<Vector2>();


    private void Awake()
    {
        _navMesh = FindObjectOfType<PolygonalNavMesh>(); //There should only be one in each scene so it should be fine.
    }

    //Quite frankly awful, fix it up!
    private void Update()
    {
        if (!enableController) return;

        if (playerControlled)
        {
            bool pathEmpty = currentPath.Count == 0;
            
            if (Input.GetKeyDown(KeyCode.A) && pathEmpty)
            {
                CombatManager.playerCombatMode = true;
            }
            
            if (CombatManager.playerCombatMode)
            {
                if (!pathEmpty)
                {
                    currentPath.Clear();
                    lineRenderer.positionCount = 0;
                }
            
                if (Input.GetMouseButtonDown(1))
                {
                    CombatManager.playerCombatMode = false;
                }
                else if (Input.GetMouseButtonDown(0) && CombatManager.target != null && !CombatManager.outOfReach)
                {
                    CombatManager.attacker = pathingEntity;
                    Vector3 ab = CombatManager.target.transform.parent.position - transform.position;
                    _currentAngle = AngleBetweenVector2(_magicZeroVector, ab);
                    if (!EntityManager.turnBasedEngaged || CombatManager.GetEntityWeapon(pathingEntity).apCost <= pathingEntity.actionPts[0])
                        characterAnimator.PlayAnimFromKeyword(CombatManager.GetEntityWeapon(pathingEntity).itemCombatAnim);
                }
            }
            
            dottedLineRenderer.positionCount = 0;

            if (!CheckValidPosition())
            {
                pathingEntity._hoverOverEntity.cachedMouseTooltip.DestroyTooltip(21);
                return;
            }
            
            Vector2[] path;
            
            if (!CombatManager.playerCombatMode && Input.GetMouseButtonDown(0))
            {
                path = GetPath(noCost: false);
                
                if (path.Length > 0)
                {
                    //Set Movement end point to hit.point and calculate path
                    currentPath.Clear();
                    StopCoroutine("FollowPath");
                    StartCoroutine(nameof(FollowPath), path);
                }
            }
            
            
            if (pathingEntity.hTFlag)
            {
                path = GetPath(noCost: true);
                
                if (path.Length > 0)
                {
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

        if (Mathf.Abs(_currentAngle - _cachedAngle) > 1f)
        {
            //Check if we should rotate the player sprite
            //Debug.Log(currentAngle);
            RotatePlayerFacing();
            _cachedAngle = _currentAngle;
        }
    }

    private IEnumerator FollowPath(Vector2[] path)
    {
        currentPath = path.ToList();
        characterAnimator.PlayAnimFromKeyword("_run");
        while (currentPath.Count > 0) //While we have not reached the last
        {
            if (_haltAllMovement) yield break;
            Vector3 position = transform.position;

            if (currentPath.Count == 1 && _stopShort)
            {
                if (Vector2.Distance(transform.position, currentPath[0]) > _stopShortDist)
                {
                    position = Vector2.MoveTowards(transform.position, currentPath[0], playerSpeed * Time.deltaTime);
                    position = new Vector3(position.x, position.y, -0.1f);
                }
                else
                {
                    currentPath.RemoveAt(0);
                    _stopShort = false;
                    
                    //We should trigger NPC interactions here since we've reached the NPC target if we're not aggro.
                }
            }
            else if (Vector2.Distance(transform.position, currentPath[0]) > minNodeDistance)
            {
                position = Vector2.MoveTowards(transform.position, currentPath[0], playerSpeed * Time.deltaTime);
                position = new Vector3(position.x, position.y, -0.1f);
            }
            else
            {
                currentPath.RemoveAt(0);
                if (currentPath.Count > 0)
                {
                    Vector2 targetDir = currentPath[0] - new Vector2(transform.position.x, position.y); //OA - OB = BA
                    _currentAngle = AngleBetweenVector2(_magicZeroVector, targetDir); //Give it a direction that results in 30deg being the new "0"
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

            transform.position = position;
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

    public void LookAtEntity(Entity target)
    {
        Vector3 ab = target.transform.parent.position - transform.position;
        _currentAngle = AngleBetweenVector2(_magicZeroVector, ab);
    }
    
    private void RotatePlayerFacing()
    {
        //Set the facing based on the currentAngle.
        if (_currentAngle > 0)
        {
            if      (_currentAngle < 22.50f) characterAnimator.currentFacing = Animation.BranchFacing.S;
            else if (_currentAngle < 67.50f) characterAnimator.currentFacing = Animation.BranchFacing.SE;
            else if (_currentAngle < 112.5f) characterAnimator.currentFacing = Animation.BranchFacing.E;
            else if (_currentAngle < 167.5f) characterAnimator.currentFacing = Animation.BranchFacing.NE;
            else                            characterAnimator.currentFacing = Animation.BranchFacing.N;
        }
        else
        {
            if      (_currentAngle > -22.50f) characterAnimator.currentFacing = Animation.BranchFacing.S;
            else if (_currentAngle > -67.50f) characterAnimator.currentFacing = Animation.BranchFacing.SW;
            else if (_currentAngle > -112.5f) characterAnimator.currentFacing = Animation.BranchFacing.W;
            else if (_currentAngle > -167.5f) characterAnimator.currentFacing = Animation.BranchFacing.NW;
            else                             characterAnimator.currentFacing = Animation.BranchFacing.N;
        }
    }

    private Vector2[] GetPath(bool noCost = false)
    {
        Vector2[] path = new Vector2[0];
        _drawEntityToolTip = false;
        bool parsingTurn = pathingEntity.hTFlag;
        _stopShort = false;
        
        //We use a 2D ray to check for 2D colliders!
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, -20));
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        if (hit2D.collider == null) return path;
        
        Collider2D hitCollider = hit2D.collider;
        if (hitCollider.GetComponent<HoverOverEntity>() != null) //Either go to or attack the NPC
        {
            Item entityWeapon = CombatManager.GetEntityWeapon(pathingEntity);
            bool canAttack = entityWeapon.apCost <= pathingEntity.actionPts[0];
            if (!CombatManager.playerCombatMode)
            {
                path = _navMesh.GetShortestPathAStar(transform.position, hitCollider.transform.parent.position);
                _stopShort = true;
            }
            
            if (parsingTurn)
            {
                CombatManager.attacker = pathingEntity;
                noCost = true;

                if (hit2D.collider.gameObject != pathingEntity.gameObject)
                    _drawEntityToolTip = true;
            }
        }
        else if (hitCollider.GetComponent<PolygonalNavMesh>() != null)
        {
            DateTime before = DateTime.Now;
            path = _navMesh.GetShortestPathAStar(transform.position, hit2D.point);
            DateTime after = DateTime.Now; 
            TimeSpan duration = after.Subtract(before);
            Debug.Log("Duration in milliseconds: " + duration.Milliseconds);
        }
        
        if (parsingTurn)
        {
            float dist = 0;
            for (int i = 1; i < path.Length; i++)
            {
                dist += Vector2.Distance(path[i - 1], path[i]);
            }
            
            PrintTooltip(dist, CombatManager.target);

            if (path.Length == 0) return new Vector2[0];
            
            int cost = CalculateMoveCost(dist);
            if (cost > pathingEntity.actionPts[0])
                return new Vector2[0];
            
            if (noCost == false)
                pathingEntity.actionPts[0] -= cost;
        }
        
        return path;
    }

    private void PrintTooltip(float distance, Entity entity)
    {
        string title = "";
        string content = "";
        int cost = CalculateMoveCost(distance);
        if (!_drawEntityToolTip)
        {
            title = "AP Cost: ";
            title += (cost > pathingEntity.actionPts[0]) ? "<color=red>Too Expensive!</color>" : cost.ToString();
            
            pathingEntity._hoverOverEntity.cachedMouseTooltip.CreateTooltip(21, title, 
                "distance: " + distance.ToString("F1") + "m");
        }
        else
        {
            title = entity.entityName;
            if (entity.entityAltTitle.Length > 0) 
                content += entity.entityAltTitle + "\n";
            
            content += "\n" + "AP Cost: ";

            Item weapon = CombatManager.GetEntityWeapon(pathingEntity);

            if (CombatManager.playerCombatMode)
            {
                content += (CombatManager.playerCombatMode && weapon.apCost > pathingEntity.actionPts[0])
                    ? "<color=red>Not Enough AP!</color>"
                    : weapon.apCost.ToString();
            }
            else
            {
                content += (cost > pathingEntity.actionPts[0])
                    ? "<color=red>Not Enough AP!</color>"
                    : cost.ToString();
            }
            
            
            Vector3 entityPos = entity.transform.parent.position;
            Vector3 playerPos = CombatManager.playerEntity.transform.parent.position;
            CombatManager.DistanceToPlayer(entityPos);
            
            if (!_navMesh.InLineOfSight(playerPos, entityPos))
                content += "\n <color=red>Can't see the Target</color>";
            else if (CombatManager.outOfReach)
                content += "\n <color=red>Out of Reach!</color>";
            else if (distance > 0.0f)
                content += "\n" + distance.ToString("F1") + "m";

            content += "\n" + entity.GetHealthPercentageStatus();

        }
        pathingEntity._hoverOverEntity.cachedMouseTooltip.CreateTooltip(21, title, content);
    }

    //Move cost calculation
    private int CalculateMoveCost(float distance) => Mathf.FloorToInt(distance / 2f) + 1;
 
    private bool CheckValidPosition()
    {
        if (IsPointerOverUIElement()) return false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, -10));
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        if (hit2D.collider == null) return false;
        return true;
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
