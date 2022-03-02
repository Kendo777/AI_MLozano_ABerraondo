using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Planning : MonoBehaviour
{
    NodePlanning CurrentStartNode;
    NodePlanning CurrentTargetNode;

    World mWorld;

    /***************************************************************************/

    void Awake()
    {
        mWorld = GetComponent<World>();
    }
    private void Start()
    {
        //Debug.Log("Planning...");
        //FindPlan(World.WorldState.WORLD_STATE_NONE, World.WorldState.WORLD_STATE_STICK_OWNED);
    }
    /***************************************************************************/

    void Update()
    {
    }

    /***************************************************************************/

    public List<NodePlanning> FindPlan(WorldState.WorldMask startWorldState, WorldState.WorldMask targetWorldState)
    {
        WorldState wState = new WorldState();
        wState.worldMask = (wState.worldMask | startWorldState);
        CurrentStartNode = new NodePlanning(wState, null);

        wState = new WorldState();
        wState.worldMask = (wState.worldMask | targetWorldState);
        CurrentTargetNode = new NodePlanning(wState, null);

        List<NodePlanning> openSet = new List<NodePlanning>();
        HashSet<NodePlanning> closedSet = new HashSet<NodePlanning>();
        openSet.Add(CurrentStartNode);
        mWorld.openSet = openSet;

        NodePlanning node = CurrentStartNode;
        while (openSet.Count > 0 && ((node.mWorldState.worldMask & CurrentTargetNode.mWorldState.worldMask) != CurrentTargetNode.mWorldState.worldMask))
        {
            // Select best node from open list
            node = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || (openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost))
                {
                    node = openSet[i];
                }
            }

            // Manage open/closed list
            openSet.Remove(node);
            closedSet.Add(node);
            mWorld.openSet = openSet;
            mWorld.closedSet = closedSet;


            // Check destination
            if ((node.mWorldState.worldMask & CurrentTargetNode.mWorldState.worldMask) != CurrentTargetNode.mWorldState.worldMask)
            {

                // Open neighbours
                foreach (NodePlanning neighbour in mWorld.GetNeighbours(node))
                {
                    if ( /*!neighbour.mWalkable ||*/ closedSet.Any(n => n.mWorldState.worldMask == neighbour.mWorldState.worldMask))
                    {
                        continue;
                    }

                    float newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Any(n => n.mWorldState.worldMask == neighbour.mWorldState.worldMask))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, CurrentTargetNode);
                        neighbour.mParent = node;

                        if (!openSet.Any(n => n.mWorldState.worldMask == neighbour.mWorldState.worldMask))
                        {
                            openSet.Add(neighbour);
                            mWorld.openSet = openSet;
                        }
                        else
                        {
                            // Find neighbour and replace
                            openSet[openSet.FindIndex(x => x.mWorldState.worldMask == neighbour.mWorldState.worldMask)] = neighbour;
                        }
                    }
                }
            }
            else
            {
                // Path found!

                // End node must be copied
                CurrentTargetNode.mParent = node.mParent;
                CurrentTargetNode.mAction = node.mAction;
                CurrentTargetNode.gCost = node.gCost;
                CurrentTargetNode.hCost = node.hCost;
                //mWorld.mWorldState = node.mWorldState;
                RetracePlan(CurrentStartNode, CurrentTargetNode);

                Debug.Log("Statistics:");
                Debug.LogFormat("Total nodes:  {0}", openSet.Count + closedSet.Count);
                Debug.LogFormat("Open nodes:   {0}", openSet.Count);
                Debug.LogFormat("Closed nodes: {0}", closedSet.Count);
            }
        }

        // Log plan
        if (mWorld.plan != null)
        {
            Debug.Log("PLAN FOUND!");
            for (int i = 0; i < mWorld.plan.Count; ++i)
            {
                Debug.LogFormat("{0} Accumulated cost: {1}", mWorld.plan[i].mAction.mName, mWorld.plan[i].gCost);
            }
        }
        else
        {
            Debug.LogFormat("PLAN NOT FOUND!");
        }
        

        return mWorld.plan;
    } 
    
    public List<NodePlanning> FindPlanBackwards(WorldState.WorldMask startWorldState, WorldState.WorldMask targetWorldState)
    {
        WorldState wState = new WorldState();
        wState.worldMask = (wState.worldMask | targetWorldState);
        CurrentStartNode = new NodePlanning(wState, null);

        wState = new WorldState();
        wState.worldMask = (wState.worldMask | startWorldState);
        CurrentTargetNode = new NodePlanning(wState, null);

        List<NodePlanning> openSet = new List<NodePlanning>();
        HashSet<NodePlanning> closedSet = new HashSet<NodePlanning>();
        openSet.Add(CurrentStartNode);
        mWorld.openSet = openSet;

        NodePlanning node = CurrentStartNode;
        while (openSet.Count > 0 && !node.mWorldState.compareState(CurrentTargetNode.mWorldState))//((node.mWorldState.worldMask & CurrentTargetNode.mWorldState.worldMask) != CurrentTargetNode.mWorldState.worldMask))
        {
            // Select best node from open list
            node = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || (openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost))
                {
                    node = openSet[i];
                }
            }

            // Manage open/closed list
            openSet.Remove(node);
            closedSet.Add(node);
            mWorld.openSet = openSet;
            mWorld.closedSet = closedSet;


            // Check destination
            if (!node.mWorldState.compareState(CurrentTargetNode.mWorldState))//(node.mWorldState.worldMask & CurrentTargetNode.mWorldState.worldMask) != CurrentTargetNode.mWorldState.worldMask)
            {

                // Open neighbours
                foreach (NodePlanning neighbour in mWorld.GetNeighboursBackwards(CurrentTargetNode, node))
                {
                    if ( /*!neighbour.mWalkable ||*/ closedSet.Any(n => n.mWorldState.compareState(neighbour.mWorldState)))
                    {
                        continue;
                    }

                    float newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Any(n => n.mWorldState.compareState(neighbour.mWorldState)))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, CurrentTargetNode);
                        neighbour.mParent = node;

                        if (!openSet.Any(n => n.mWorldState.compareState(neighbour.mWorldState)))
                        {
                            openSet.Add(neighbour);
                            mWorld.openSet = openSet;
                        }
                        else
                        {
                            // Find neighbour and replace
                            openSet[openSet.FindIndex(x => x.mWorldState.compareState(neighbour.mWorldState))] = neighbour;
                        }
                    }
                }
            }
            else
            {
                // Path found!

                // End node must be copied
                CurrentTargetNode.mParent = node.mParent;
                CurrentTargetNode.mAction = node.mAction;
                CurrentTargetNode.gCost = node.gCost;
                CurrentTargetNode.hCost = node.hCost;
                //mWorld.mWorldState = node.mWorldState;
                RetracePlanBackwards(CurrentStartNode, CurrentTargetNode);

                Debug.Log("Statistics:");
                Debug.LogFormat("Total nodes:  {0}", openSet.Count + closedSet.Count);
                Debug.LogFormat("Open nodes:   {0}", openSet.Count);
                Debug.LogFormat("Closed nodes: {0}", closedSet.Count);
            }
        }
        // Log plan
        if (mWorld.plan != null)
        {
            Debug.Log("PLAN FOUND!");
            for (int i = 0; i < mWorld.plan.Count; ++i)
            {
                Debug.LogFormat("{0} Accumulated cost: {1}", mWorld.plan[i].mAction.mName, mWorld.plan[i].gCost);
            }
        }
        else
        {
            Debug.LogFormat("PLAN NOT FOUND!");
        }

        return mWorld.plan;
    }

    /***************************************************************************/
    void RetracePlan(NodePlanning startNode, NodePlanning endNode)
    {
        List<NodePlanning> plan = new List<NodePlanning>();

        NodePlanning currentNode = endNode;

        while (currentNode != startNode)
        {
            plan.Add(currentNode);
            currentNode = currentNode.mParent;
        }
        plan.Reverse();

        mWorld.plan = plan;
    }

    void RetracePlanBackwards(NodePlanning startNode, NodePlanning endNode)
    {
        List<NodePlanning> plan = new List<NodePlanning>();

        NodePlanning currentNode = endNode;

        while (currentNode != startNode)
        {
            plan.Add(currentNode);
            currentNode = currentNode.mParent;
        }
        //plan.Reverse();

        mWorld.plan = plan;
    }

    /***************************************************************************/

    float GetDistance(NodePlanning nodeA, NodePlanning nodeB)
    {
        // Distance function
        return nodeB.mAction.mCost;
    }

    /***************************************************************************/

    float Heuristic(NodePlanning nodeA, NodePlanning nodeB)
    {
        float cost = 0.0f;
        // Heuristic function
        if ((nodeB.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_AXE_OWNED) == WorldState.WorldMask.WORLD_STATE_AXE_OWNED)
        {
            if(nodeA.mWorldState.nStick == 1)
            {
                cost -= 150000.0f;
            }
            if(nodeA.mWorldState.nStone == 1)
            {
                cost -= 150000.0f;
            }
            if(nodeA.mWorldState.nRope == 1)
            {
                cost -= 150000.0f;
            }
        }
        if ((nodeB.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED) == WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED)
        {
            if (nodeA.mWorldState.nStick == 1)
            {
                cost -= 150000.0f;
            }
            if (nodeA.mWorldState.nStone == 1)
            {
                cost -= 150000.0f;
            }
        }
        if ((nodeB.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED) == WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED)
        {
            if (nodeA.mWorldState.nStick == 1)
            {
                cost -= 150000.0f;
            }
            if (nodeA.mWorldState.nRope == 1)
            {
                cost -= 150000.0f;
            }
        }
        if ((nodeB.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED) == WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED)
        {
            if (nodeA.mWorldState.nStick > 0 && nodeA.mWorldState.nStick <= 3)
            {
                cost -= 150000.0f;
            }
            if (nodeA.mWorldState.nStone > 0 && nodeA.mWorldState.nStone <= 2)
            {
                cost -= 150000.0f;
            }
        }

        if ((nodeB.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_POT_OWNED) == WorldState.WorldMask.WORLD_STATE_POT_OWNED)
        {
            if((nodeA.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED) == WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED)
            {
                cost -= 200000.0f;
            }
            if ((nodeA.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED) == WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED)
            {
                cost -= 300000.0f;
            }
            if (nodeA.mWorldState.nIron > 0 && nodeA.mWorldState.nIron <= 2)
            {
                cost -= 150000.0f;
            }
        }       
        
        if((nodeB.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_TENT_OWNED) == WorldState.WorldMask.WORLD_STATE_TENT_OWNED)
        {
            if((nodeA.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_AXE_OWNED) == WorldState.WorldMask.WORLD_STATE_AXE_OWNED)
            {
                cost -= 200000.0f;
            }
            if ((nodeA.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED) == WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED)
            {
                cost -= 300000.0f;
            }
            if ((nodeA.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_POT_OWNED) == WorldState.WorldMask.WORLD_STATE_POT_OWNED)
            {
                cost -= 350000.0f;
            }

            if (nodeA.mWorldState.nWood > 0 && nodeA.mWorldState.nWood <= 2)
            {
                cost -= 150000.0f;
            }
            if (nodeA.mWorldState.nRope == 1)
            {
                cost -= 150000.0f;
            }
        }
        return cost;
    }

    public World GetWorld()
    {
        return mWorld;
    }
    /***************************************************************************/

}
