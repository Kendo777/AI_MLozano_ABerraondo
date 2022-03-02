using UnityEngine;
using System.Collections;

public class ActionPlanning
{
    public ActionType mActionType;
    public WorldState.WorldMask mPreconditions;
    public WorldState.WorldMask mNegativePreconditions;
    public WorldState.WorldMask mEffects;
    public WorldState.WorldMask mNegativeEffects;
    public float mCost;
    public string mName;

    /***************************************************************************/

    public enum ActionType
    {
        ACTION_TYPE_NONE = -1,
        ACTION_TYPE_GET_STICK,//
        ACTION_TYPE_GET_STONES,//
        ACTION_TYPE_GET_ROPE,//
        ACTION_TYPE_GET_IRON, //
        ACTION_TYPE_GET_FISH, //
        ACTION_TYPE_GET_MUSHROOM, //
        ACTION_TYPE_CRAFT_AXE,//
        ACTION_TYPE_CRAFT_PICK_AXE,//
        ACTION_TYPE_CRAFT_FISHING_ROD, //
        ACTION_TYPE_GET_WOOD,//
        ACTION_TYPE_CRAFT_FIREPLACE, //
        ACTION_TYPE_COOK_FISH, //
        ACTION_TYPE_COOK_MUSHROOM, //
        ACTION_TYPE_CRAFT_TENT, //
        ACTION_TYPE_CRAFT_POT, //
        ACTION_TYPES
    }

    /***************************************************************************/

    public ActionPlanning(ActionType actionType, WorldState.WorldMask preconditions, WorldState.WorldMask negativePreconditions, WorldState.WorldMask effects, WorldState.WorldMask negativeEffects,float cost, string name)
    {
        mActionType = actionType;
        mPreconditions = preconditions;
        mNegativePreconditions = negativePreconditions;
        mEffects = effects;
        mNegativeEffects = negativeEffects;
        mCost = cost;
        mName = name;
    }

    /***************************************************************************/

}
