#define USING_WORLDMASK
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class World : MonoBehaviour
{
    public List<NodePlanning> openSet;
    public HashSet<NodePlanning> closedSet;

    public List<NodePlanning> plan;

    public WorldState mWorldState;

    public List<ActionPlanning> mActionList;

    public bool complexWorld = true;

    private void Awake()
    {
        complexWorld = GetComponent<BehaviourTree>().complexWorld;
        mWorldState = new WorldState();
        mActionList = new List<ActionPlanning>();
        if (complexWorld)
        {
            mActionList.Add(
             new ActionPlanning(
               ActionPlanning.ActionType.ACTION_TYPE_GET_STICK,
               WorldState.WorldMask.WORLD_STATE_NONE,
               WorldState.WorldMask.WORLD_STATE_NONE,
               WorldState.WorldMask.WORLD_STATE_NONE,
               WorldState.WorldMask.WORLD_STATE_NONE,
               2.0f, "Get stick")
           );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_STONES,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                3.0f, "Get stone")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_ROPE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                1.0f, "Get rope")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_IRON,
                WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                5.0f, "Get iron")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_MUSHROOM,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                1.0f, "Get mushroom")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_FISH,
                WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                5.0f, "Get fish")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_WOOD,
                WorldState.WorldMask.WORLD_STATE_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_NONE,
                20.0f, "Get wood")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_AXE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                12.0f, "Axe crafted")
            );
            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_PICK_AXE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                8.0f, "Pick axe crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FISHING_ROD,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED,
                WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                4.0f, "Fishing rod crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FIREPLACE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED,
                WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                5.0f, "Fireplace crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_TENT,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_NONE,
                50.0f, "Tent crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_POT,
                WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED,
                WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_NONE,
                30.0f, "Pot crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_COOK_FISH,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_POT_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_HUNGER,
                40.0f, "Cook fish")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_COOK_MUSHROOM,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_POT_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_HUNGER,
                20.0f, "Cook mushroom")
            );
        }
        else
        {
            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_STICK,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_STICK_OWNED,
                WorldState.WorldMask.WORLD_STATE_STICK_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                2.0f + Vector3.Distance(mWorldState.player.transform.position, mWorldState.GetNearestObject(mWorldState.sticks).transform.position), "Get stick")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_STONES,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_STONE_OWNED,
                WorldState.WorldMask.WORLD_STATE_STONE_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                3.0f + Vector3.Distance(mWorldState.player.transform.position, mWorldState.GetNearestObject(mWorldState.stones).transform.position), "Get stone")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_ROPE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_ROPE_OWNED,
                WorldState.WorldMask.WORLD_STATE_ROPE_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                1.0f + Vector3.Distance(mWorldState.player.transform.position, mWorldState.GetNearestObject(mWorldState.rope).transform.position), "Get rope")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_IRON,
                WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_IRON_OWNED,
                WorldState.WorldMask.WORLD_STATE_IRON_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                5.0f + +Vector3.Distance(mWorldState.player.transform.position, mWorldState.GetNearestObject(mWorldState.iron).transform.position), "Get iron")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_MUSHROOM,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED,
                WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                1.0f + +Vector3.Distance(mWorldState.player.transform.position, mWorldState.GetNearestObject(mWorldState.mushrooms).transform.position), "Get mushroom")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_FISH,
                WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED,
                WorldState.WorldMask.WORLD_STATE_FISH_OWNED,
                WorldState.WorldMask.WORLD_STATE_FISH_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                5.0f + Vector3.Distance(mWorldState.player.transform.position, mWorldState.GetNearestObject(mWorldState.water).transform.position), "Get fish")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_GET_WOOD,
                WorldState.WorldMask.WORLD_STATE_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_WOOD_OWNED,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_WOOD_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                20.0f + +Vector3.Distance(mWorldState.player.transform.position, mWorldState.GetNearestObject(mWorldState.trees).transform.position), "Get wood")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_AXE,
                WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED,
                WorldState.WorldMask.WORLD_STATE_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED,
                12.0f, "Axe crafted")
            );
            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_PICK_AXE,
                WorldState.WorldMask.WORLD_STATE_STONE_OWNED | WorldState.WorldMask.WORLD_STATE_STICK_OWNED,
                WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED,
                WorldState.WorldMask.WORLD_STATE_STONE_OWNED | WorldState.WorldMask.WORLD_STATE_STICK_OWNED,
                8.0f, "Pick axe crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FISHING_ROD,
                WorldState.WorldMask.WORLD_STATE_ROPE_OWNED | WorldState.WorldMask.WORLD_STATE_STICK_OWNED,
                WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED,
                WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED,
                WorldState.WorldMask.WORLD_STATE_ROPE_OWNED | WorldState.WorldMask.WORLD_STATE_STICK_OWNED,
                4.0f, "Fishing rod crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FIREPLACE,
                WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED,
                WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED,
                WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED,
                WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED,
                5.0f, "Fireplace crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_TENT,
                WorldState.WorldMask.WORLD_STATE_WOOD_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED,
                WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_WOOD_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED,
                50.0f, "Tent crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_CRAFT_POT,
                WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED | WorldState.WorldMask.WORLD_STATE_IRON_OWNED,
                WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER,
                WorldState.WorldMask.WORLD_STATE_IRON_OWNED,
                30.0f, "Pot crafted")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_COOK_FISH,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_FISH_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_FISH_OWNED,
                5.0f, "Cook fish")
            );

            mActionList.Add(
              new ActionPlanning(
                ActionPlanning.ActionType.ACTION_TYPE_COOK_MUSHROOM,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_NONE,
                WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED,
                20.0f, "Cook mushroom")
            );
        }

    }

    /***************************************************************************/

    public List<NodePlanning> GetNeighbours(NodePlanning node)
    {
        List<NodePlanning> neighbours = new List<NodePlanning>();

        foreach (ActionPlanning action in mActionList)
        {
            if ((node.mWorldState.worldMask & action.mPreconditions) == action.mPreconditions && (~(node.mWorldState.worldMask) & action.mNegativePreconditions) == action.mNegativePreconditions)
            {
                WorldState wState = new WorldState();
                NodePlanning newNodePlanning;
                wState.worldMask = (node.mWorldState.worldMask | action.mEffects) & ~(action.mNegativeEffects);
                newNodePlanning = new NodePlanning(wState, action);
                neighbours.Add(newNodePlanning);
            }

        }

        return neighbours;
    } 
    
    public List<NodePlanning> GetNeighboursBackwards(NodePlanning origin, NodePlanning node)
    {
        List<NodePlanning> neighbours = new List<NodePlanning>();

        foreach (ActionPlanning action in mActionList)
        {
            //if ((node.mWorldState.worldMask & action.mEffects) != 0)//&& (~(node.mWorldState.worldMask) & action.mNegativeEffects) == action.mNegativeEffects)
            //{
                WorldState wState = new WorldState();
                NodePlanning newNodePlanning;
                switch (action.mActionType)
                {
                    case ActionPlanning.ActionType.ACTION_TYPE_GET_STICK:
                        if (origin.mWorldState.nStick != node.mWorldState.nStick)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStick--;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_GET_STONES:
                        if (origin.mWorldState.nStone != node.mWorldState.nStone)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStone--;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;
                    
                    case ActionPlanning.ActionType.ACTION_TYPE_GET_IRON:
                        if (origin.mWorldState.nIron != node.mWorldState.nIron)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nIron--;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_GET_ROPE:
                        if (origin.mWorldState.nRope != node.mWorldState.nRope)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nRope--;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;
                    
                    case ActionPlanning.ActionType.ACTION_TYPE_GET_WOOD:
                        if (origin.mWorldState.nWood != node.mWorldState.nWood && (node.mWorldState.worldMask & action.mEffects) != 0)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nWood--;
                            wState.nTress++;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_GET_MUSHROOM:
                        if (origin.mWorldState.nMushrooms != node.mWorldState.nMushrooms)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nMushrooms--;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_GET_FISH:
                        if (origin.mWorldState.nFish != node.mWorldState.nFish)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nFish--;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_CRAFT_AXE:
                        if ((node.mWorldState.worldMask & action.mEffects) != 0)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStick += 1;
                            wState.nStone += 1;
                            wState.nRope += 1;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_CRAFT_PICK_AXE:
                        if ((node.mWorldState.worldMask & action.mEffects) != 0)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStone += 1;
                            wState.nStick += 1;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FISHING_ROD:
                        if ((node.mWorldState.worldMask & action.mEffects) != 0)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStick += 1;
                            wState.nRope += 1;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FIREPLACE:
                        if ((node.mWorldState.worldMask & action.mEffects) != 0)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStone += 2;
                            wState.nStick += 3;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_CRAFT_POT:
                        if ((node.mWorldState.worldMask & action.mEffects) != 0 && (node.mWorldState.worldMask & action.mEffects) !=  WorldState.WorldMask.WORLD_STATE_HUNGER)
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nIron += 2;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;

                    case ActionPlanning.ActionType.ACTION_TYPE_CRAFT_TENT:
                        if ((node.mWorldState.worldMask & action.mEffects) != 0 && (node.mWorldState.worldMask & action.mEffects) != WorldState.WorldMask.WORLD_STATE_HUNGER)
                        {
                            wState.copyState(node.mWorldState, (node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions));
                            wState.nWood += 2;
                            wState.nRope += 1;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;
                    
                    case ActionPlanning.ActionType.ACTION_TYPE_COOK_MUSHROOM:
                        if (((node.mWorldState.worldMask & WorldState.WorldMask.WORLD_STATE_HUNGER) != WorldState.WorldMask.WORLD_STATE_HUNGER && origin.mWorldState.nWood != node.mWorldState.nWood) || ( node.mAction != null && (node.mAction.mNegativePreconditions & action.mNegativeEffects) != 0))
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStick += 1;
                            wState.nMushrooms += 1;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }
                        break;
                    
                    case ActionPlanning.ActionType.ACTION_TYPE_COOK_FISH:
                        /*if (((node.mWorldState.worldMask & action.mEffects) != 0) || (node.mAction != null && (node.mAction.mNegativePreconditions & action.mNegativeEffects) != 0))
                        {
                            wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                            wState.nStick += 1;
                            wState.nFish += 1;
                            // Apply action and effects
                            newNodePlanning = new NodePlanning(wState, action);
                            neighbours.Add(newNodePlanning);
                        }*/
                        break;
                    
                    default:
                        wState.copyState(node.mWorldState, ((node.mWorldState.worldMask | action.mPreconditions) & ~(action.mNegativePreconditions)));
                        // Apply action and effects
                        newNodePlanning = new NodePlanning(wState, action);
                        neighbours.Add(newNodePlanning);
                        break;
                }
            //}

        }

        return neighbours;
    }

    /***************************************************************************/

    public static int PopulationCount(int n)
    {
        return System.Convert.ToString(n, 2).ToCharArray().Count(c => c == '1');
    }

    /***************************************************************************/

}