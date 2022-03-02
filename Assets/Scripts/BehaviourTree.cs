using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NPBehave;
public class BehaviourTree : MonoBehaviour
{
    [Header("World")]
    public bool complexWorld = true;
    public WorldState.WorldMask startWorldState;
    public WorldState.WorldMask targetWorldState;
    public Pathfinding pathfinding;
    public Planning worldPlanning;

    [Header("Particles")]

    public GameObject whiteParticles;
    public GameObject lightBulb;
    public GameObject collectMaterialsParticles;

    [Header("Objects")]
    public GameObject axe;
    public GameObject pickaxe;
    public GameObject fishingRod;
    public GameObject shroomsInFirePlace;
    public GameObject fishInFirePlace;
    public GameObject fishToEat;
    public GameObject logCut;

    [Header("Times")]
    public float timeToRecolect = 2.0f;
    public float timeToCraft = 4.0f;
    public float timeToEat = 5.0f;
    public float timeToHardWorking = 5.0f;

    [Header("Animator")]
    public Animator playerAnimator;


    List<NodePlanning> mPlan;
    int mPlanSteps = 0;
    int mCurrentAction = -1;

    private GameObject nearestObject = null;
    private Root mBehaviorTree;



    private float timmer = 0.0f;


    /****************************************************************************/

    // Use this for initialization
    void Start()
    {
        mBehaviorTree = new Root(
          new Sequence(
            new Action((bool planning) =>
            {
                Debug.Log("Planning...");
                if (complexWorld)
                {
                    mPlan = worldPlanning.FindPlanBackwards(startWorldState, targetWorldState);
                }
                else
                {
                    mPlan = worldPlanning.FindPlan(startWorldState, targetWorldState);
                }
                worldPlanning.GetWorld().mWorldState.tent.SetActive(false);
                worldPlanning.GetWorld().mWorldState.pot.SetActive(false);
                worldPlanning.GetWorld().mWorldState.firePlace.SetActive(false);


                if (mPlan != null)   mPlanSteps = mPlan.Count;
                mCurrentAction = -1;

                Debug.Log("Planned in " + mPlanSteps + " steps");
                if (mPlan != null && mPlan.Count > 0)
                {
                    return Action.Result.SUCCESS;
                }
                else
                {
                    StopBehaviorTree();
                    return Action.Result.FAILED;
                }
            })
            { Label = "Planning" },
            new Repeater(-1,
              new Sequence(
                new Action((bool nextActionAvailable) =>
                {
                    mCurrentAction++;
                    timmer = 0.0f;

                    if (mCurrentAction >= mPlan.Count)
                    {
                        StopBehaviorTree();
                        return Action.Result.FAILED;
                    }
                    else
                    {
                        Debug.Log(mPlan[mCurrentAction].mAction.mActionType.ToString());
                        return Action.Result.SUCCESS;
                    }
                }),
                new Selector(
                    new Sequence(                   // GET STICK
                  new Action((bool nearStick) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_STICK)
                      {
                              WorldState.WorldMask preconditions;
                              WorldState.WorldMask negativePreconditions;
                              WorldState.WorldMask effects;
                              WorldState.WorldMask negativeEffects;
                              if (complexWorld)
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_IRON | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                                  effects = WorldState.WorldMask.WORLD_STATE_NEAR_STICK;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                              }
                              else
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_STICK_OWNED |WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_IRON | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                                  effects = WorldState.WorldMask.WORLD_STATE_NEAR_STICK;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                              }
                              if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                              {
                                  if (pathfinding.GetGrid().path == null)
                                  {
                                      nearestObject = worldPlanning.GetWorld().mWorldState.GetNearestObject(worldPlanning.GetWorld().mWorldState.sticks);
                                      nearestObject.layer = 0;
                                      foreach (Transform child in nearestObject.GetComponentsInChildren<Transform>(true))
                                      {
                                          child.gameObject.layer = LayerMask.NameToLayer("Default");
                                      }


                                      pathfinding.GetGrid().UpdateGrid();

                                      pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                      worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                                  }
                              }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go near stick");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              
                              return Action.Result.SUCCESS;

                          }
                          else
                          {
                              // Action in progress
                              //playerAnimator.SetBool("Walk", false);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearStick" },
                  new Action((bool recolectStick) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_STICK)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STICK;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_NONE;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_STICK;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STICK;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_STICK_OWNED;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_STICK;
                          }
                          if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                          {
                              timmer += Time.deltaTime;
                          }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (timmer >= timeToRecolect)
                          {
                              Debug.Log("Recolect stick");
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              worldPlanning.GetWorld().mWorldState.nStick++;
                              nearestObject.SetActive(false);
                              nearestObject.tag = "Untagged";
                              playerAnimator.SetBool("Recolect", false);
                              Instantiate(collectMaterialsParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              playerAnimator.SetBool("Recolect", true);

                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "RecolectStick" }
                  ),
                  new Sequence(                   // GET STONE
                  new Action((bool nearStone) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_STONES)
                      {
                              WorldState.WorldMask preconditions;
                              WorldState.WorldMask negativePreconditions;
                              WorldState.WorldMask effects;
                              WorldState.WorldMask negativeEffects;
                              if (complexWorld)
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_IRON | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                                  effects = WorldState.WorldMask.WORLD_STATE_NEAR_STONE;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                              }
                              else
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_STONE_OWNED |WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_IRON | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                                  effects = WorldState.WorldMask.WORLD_STATE_NEAR_STONE;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                              }
                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                                if (pathfinding.GetGrid().path == null)
                                {
                                    nearestObject = worldPlanning.GetWorld().mWorldState.GetNearestObject(worldPlanning.GetWorld().mWorldState.stones);
                                    nearestObject.layer = 0;
                                    foreach (Transform child in nearestObject.GetComponentsInChildren<Transform>(true))
                                    {
                                        child.gameObject.layer = LayerMask.NameToLayer("Default");
                                    }


                                    pathfinding.GetGrid().UpdateGrid();

                                    pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                    worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                              }
                            }
                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go near stone");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                          // Action in progress
                          return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearStone" },
                  new Action((bool recolectStone) =>
                    {
                        if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_STONES)
                        {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                            if (complexWorld)
                            {
                                preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STONE;
                                negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                effects = WorldState.WorldMask.WORLD_STATE_NONE;
                                negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_STONE;
                            }
                            else
                            {
                                preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STONE;
                                negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                effects = WorldState.WorldMask.WORLD_STATE_STONE_OWNED;
                                negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_STONE;
                            }
                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                                timmer += Time.deltaTime;
                            }

                            // If execution succeeded return "success". Otherwise return "failed".
                            if (timmer >= timeToRecolect)
                            {
                                Debug.Log("Recolect stone");
                                // Apply action and effects
                                worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                worldPlanning.GetWorld().mWorldState.nStone++;
                                nearestObject.SetActive(false);
                                nearestObject.tag = "Untagged";
                                Instantiate(collectMaterialsParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                                playerAnimator.SetBool("Recolect", false);
                                return Action.Result.SUCCESS;
                            }
                            else
                            {
                                // Action in progress
                                //playerAnimator.SetBool("Walk", false);
                                playerAnimator.SetBool("Recolect", true);
                                return Action.Result.PROGRESS;
                            }
                        }
                        else
                        {
                            return Action.Result.FAILED;
                        }
                    })
                  { Label = "RecolectStone" }
                  ),                  
                  new Sequence(                   // GET IRON
                  new Action((bool nearIron) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_IRON)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              effects = WorldState.WorldMask.WORLD_STATE_NEAR_IRON;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_IRON_OWNED |WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              effects = WorldState.WorldMask.WORLD_STATE_NEAR_IRON;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                                if (pathfinding.GetGrid().path == null)
                                {
                                    nearestObject = worldPlanning.GetWorld().mWorldState.GetNearestObject(worldPlanning.GetWorld().mWorldState.iron);
                                    nearestObject.layer = 0;
                                    foreach (Transform child in nearestObject.GetComponentsInChildren<Transform>(true))
                                    {
                                        child.gameObject.layer = LayerMask.NameToLayer("Default");
                                    }


                                    pathfinding.GetGrid().UpdateGrid();

                                    pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                    worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                              }
                            }
                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go near Iron");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              axe.SetActive(false);
                              fishingRod.SetActive(false);
                              pickaxe.SetActive(true);
                          return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearIron" },
                  new Action((bool recolectIron) =>
                      {
                          if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_IRON)
                          {
                              WorldState.WorldMask preconditions;
                              WorldState.WorldMask negativePreconditions;
                              WorldState.WorldMask effects;
                              WorldState.WorldMask negativeEffects;
                              if (complexWorld)
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_IRON | WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  effects = WorldState.WorldMask.WORLD_STATE_NONE;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_IRON;
                              }
                              else
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_IRON | WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  effects = WorldState.WorldMask.WORLD_STATE_IRON_OWNED;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_IRON;
                              }
                              if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                              {
                                  timmer += Time.deltaTime;
                              }

                              // If execution succeeded return "success". Otherwise return "failed".
                              if (timmer >= timeToHardWorking)
                              {
                                  Debug.Log("Recolect iron");
                                  // Apply action and effects
                                  worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                  worldPlanning.GetWorld().mWorldState.nIron++;
                                  nearestObject.SetActive(false);
                                  Instantiate(collectMaterialsParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                                  nearestObject.tag = "Untagged";
                                  playerAnimator.SetBool("CuttingWood", false);
                                  return Action.Result.SUCCESS;
                              }
                              else
                              {
                                  // Action in progress
                                  //playerAnimator.SetBool("Walk", false);
                                  pickaxe.SetActive(true);
                                  axe.SetActive(false);
                                  fishingRod.SetActive(false);
                                  playerAnimator.SetBool("CuttingWood", true);
                                  return Action.Result.PROGRESS;
                              }
                          }
                          else
                          {
                              return Action.Result.FAILED;
                          }
                      })
                  { Label = "RecolectIron" }
                  ),
                  new Sequence(                   // GET ROPE
                  new Action((bool nearRope) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_ROPE)
                      {
                              WorldState.WorldMask preconditions;
                              WorldState.WorldMask negativePreconditions;
                              WorldState.WorldMask effects;
                              WorldState.WorldMask negativeEffects;
                              if (complexWorld)
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                                  effects = WorldState.WorldMask.WORLD_STATE_NEAR_ROPE;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                              }
                              else
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_ROPE_OWNED |WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                                  effects = WorldState.WorldMask.WORLD_STATE_NEAR_ROPE;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                              }
                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                                if (pathfinding.GetGrid().path == null)
                                {
                                    nearestObject = worldPlanning.GetWorld().mWorldState.GetNearestObject(worldPlanning.GetWorld().mWorldState.rope);
                                    nearestObject.layer = 0;
                                    foreach (Transform child in nearestObject.GetComponentsInChildren<Transform>(true))
                                    {
                                        child.gameObject.layer = LayerMask.NameToLayer("Default");
                                    }


                                    pathfinding.GetGrid().UpdateGrid();

                                    pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                  worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                              }
                            }
                          
                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go near rope");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearRope" },
                  new Action((bool recolectRope) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_ROPE)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_ROPE;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_NONE;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_ROPE;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_ROPE;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_ROPE_OWNED;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_ROPE;
                          }
                          if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                          {
                              timmer += Time.deltaTime;
                          }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (timmer >= timeToRecolect)
                          {
                              Debug.Log("Recolect rope");
                              playerAnimator.SetBool("Recolect", false);
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              worldPlanning.GetWorld().mWorldState.nRope++;
                              Instantiate(collectMaterialsParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                              nearestObject.SetActive(false);
                              nearestObject.tag = "Untagged";
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              //playerAnimator.SetBool("Walk", false);
                              playerAnimator.SetBool("Recolect", true);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "RecolectRope" }
                  ),
                  new Sequence(                   // GET MUSHROOM
                  new Action((bool nearMushroom) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_MUSHROOM)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                              effects = WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED | WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                              effects = WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                                if (pathfinding.GetGrid().path == null)
                                {
                                    nearestObject = worldPlanning.GetWorld().mWorldState.GetNearestObject(worldPlanning.GetWorld().mWorldState.mushrooms);
                                    nearestObject.layer = 0;
                                    foreach (Transform child in nearestObject.GetComponentsInChildren<Transform>(true))
                                    {
                                        child.gameObject.layer = LayerMask.NameToLayer("Default");
                                    }


                                    pathfinding.GetGrid().UpdateGrid();

                                    pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                  worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                              }
                            }
                          
                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go near Mushroom");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearMushroom" },
                  new Action((bool recolectMushroom) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_MUSHROOM)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_NONE;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                          }
                          if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                          {
                              timmer += Time.deltaTime;
                          }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (timmer >= timeToRecolect)
                          {
                              Debug.Log("Recolect Mushroom");
                              playerAnimator.SetBool("Recolect", false);
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              worldPlanning.GetWorld().mWorldState.nMushrooms++;
                              nearestObject.SetActive(false);
                              nearestObject.tag = "Untagged";
                              Instantiate(collectMaterialsParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              //playerAnimator.SetBool("Walk", false);
                              playerAnimator.SetBool("Recolect", true);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "RecolectMushroom" }
                  ),
                  new Sequence(                   // GET FISH
                  new Action((bool nearWater) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_FISH)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              effects = WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_FISH_OWNED |WorldState.WorldMask.WORLD_STATE_NEAR_STICK | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_STONE | WorldState.WorldMask.WORLD_STATE_NEAR_ROPE | WorldState.WorldMask.WORLD_STATE_NEAR_MUSHROOM;
                              effects = WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                                if (pathfinding.GetGrid().path == null)
                                {
                                    nearestObject = worldPlanning.GetWorld().mWorldState.GetNearestObject(worldPlanning.GetWorld().mWorldState.water);
                                    nearestObject.layer = 0;
                                    foreach (Transform child in nearestObject.GetComponentsInChildren<Transform>(true))
                                    {
                                        child.gameObject.layer = LayerMask.NameToLayer("Default");
                                    }


                                    pathfinding.GetGrid().UpdateGrid();

                                    pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                  worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                              }
                            }
                          
                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go near water");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              fishingRod.SetActive(true);
                              axe.SetActive(false);
                              pickaxe.SetActive(false);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearWater" },
                  new Action((bool fishing) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_FISH)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_NONE;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                              effects = WorldState.WorldMask.WORLD_STATE_FISH_OWNED;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_WATER;
                          }
                          if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                          {
                              timmer += Time.deltaTime;
                          }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (timmer >= timeToHardWorking)
                          {
                              Debug.Log("Catch a fish");
                              playerAnimator.SetBool("Recolect", false);
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              worldPlanning.GetWorld().mWorldState.nFish++;
                              nearestObject.tag = "Untagged";
                              playerAnimator.SetBool("CuttingWood", false);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              //playerAnimator.SetBool("Walk", false);
                              playerAnimator.SetBool("CuttingWood", true);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "Fishing" }
                  ),
                  new Sequence(                   // CRAFT
                  new Action((bool nearCraftingTable) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_AXE || mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_PICK_AXE || mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FISHING_ROD)
                      {
                          
                            WorldState.WorldMask preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                            WorldState.WorldMask negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                            WorldState.WorldMask effects = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE;
                            WorldState.WorldMask negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;

                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                                if (pathfinding.GetGrid().path == null)
                                {
                                    nearestObject = worldPlanning.GetWorld().mWorldState.craftingTables;

                                    pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                  worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                                }
                            }
                          
                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go to crafting table");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearCraaftingTable" },
                  new Selector(
                      new Action((bool craft) =>
                      {
                          if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_AXE)
                          {
                              WorldState.WorldMask preconditions;
                              WorldState.WorldMask negativePreconditions;
                              WorldState.WorldMask effects;
                              WorldState.WorldMask negativeEffects;
                              if (complexWorld)
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_AXE_OWNED;
                                  effects = WorldState.WorldMask.WORLD_STATE_AXE_OWNED;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE;
                              }
                              else
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED| WorldState.WorldMask.WORLD_STATE_ROPE_OWNED;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_AXE_OWNED;
                                  effects = WorldState.WorldMask.WORLD_STATE_AXE_OWNED;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED;
                              }
                              if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                              {
                                  if (!complexWorld || (worldPlanning.GetWorld().mWorldState.nRope >= 1 && worldPlanning.GetWorld().mWorldState.nStone >= 1 && worldPlanning.GetWorld().mWorldState.nStick >= 1))
                                  {
                                      timmer += Time.deltaTime;
                                  }
                                  else
                                  {
                                      return Action.Result.FAILED;
                                  }
                              }

                              // If execution succeeded return "success". Otherwise return "failed".
                              if (timmer >= timeToCraft)
                              {
                                  Debug.Log("Axe crafted");
                                  // Apply action and effects
                                  worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                  worldPlanning.GetWorld().mWorldState.nStick -= 1;
                                  worldPlanning.GetWorld().mWorldState.nRope -= 1;
                                  worldPlanning.GetWorld().mWorldState.nStone -= 1;
                                  axe.SetActive(true);
                                  pickaxe.SetActive(false);
                                  fishingRod.SetActive(false);
                                  lightBulb.SetActive(false);
                                  Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                  playerAnimator.SetBool("Craft", false);
                                  return Action.Result.SUCCESS;
                              }
                              else
                              {
                                  // Action in progress
                                  if (!lightBulb.activeSelf)
                                  {
                                      Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                  }
                                  lightBulb.SetActive(true);
                                  playerAnimator.SetBool("Craft", true);
                                  return Action.Result.PROGRESS;
                              }
                          }
                          else
                          {
                              return Action.Result.FAILED;
                          }
                      })
                      { Label = "Craft axe" },
                      new Action((bool craft) =>
                      {
                          if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_PICK_AXE)
                          {
                              WorldState.WorldMask preconditions;
                              WorldState.WorldMask negativePreconditions;
                              WorldState.WorldMask effects;
                              WorldState.WorldMask negativeEffects;
                              if (complexWorld)
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                                  effects = WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE;
                              }
                              else
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                                  effects = WorldState.WorldMask.WORLD_STATE_PICK_AXE_OWNED;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED;
                              }
                              if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                              {
                                  if (!complexWorld || (worldPlanning.GetWorld().mWorldState.nStick >= 1 && worldPlanning.GetWorld().mWorldState.nStone >= 1))
                                  {
                                      timmer += Time.deltaTime;
                                  }
                                  else
                                  {
                                      return Action.Result.FAILED;
                                  }
                              }

                              // If execution succeeded return "success". Otherwise return "failed".
                              if (timmer >= timeToCraft)
                              {
                                  Debug.Log("Pick Axe crafted");
                                  // Apply action and effects
                                  worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                  worldPlanning.GetWorld().mWorldState.nStone -= 1;
                                  worldPlanning.GetWorld().mWorldState.nStick -= 1;
                                  pickaxe.SetActive(true);
                                  axe.SetActive(false);
                                  fishingRod.SetActive(false);
                                  playerAnimator.SetBool("Craft", false);
                                  Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                  lightBulb.SetActive(false);
                                  return Action.Result.SUCCESS;
                              }
                              else
                              {
                                  // Action in progress
                                  if (!lightBulb.activeSelf)
                                  {
                                      Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                  }
                                  lightBulb.SetActive(true);
                                  playerAnimator.SetBool("Craft", true);
                                  return Action.Result.PROGRESS;
                              }
                          }
                          else
                          {
                              return Action.Result.FAILED;
                          }
                      })
                      { Label = "Craft pick axe" },
                      new Action((bool craft) =>
                      {
                          if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FISHING_ROD)
                          {
                              WorldState.WorldMask preconditions;
                              WorldState.WorldMask negativePreconditions;
                              WorldState.WorldMask effects;
                              WorldState.WorldMask negativeEffects;
                              if (complexWorld)
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED;
                                  effects = WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE;
                              }
                              else
                              {
                                  preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED;
                                  negativePreconditions = WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED;
                                  effects = WorldState.WorldMask.WORLD_STATE_FISHING_ROD_OWNED;
                                  negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_CRAFTING_TABLE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED;
                              }
                              if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                              {
                                  if (!complexWorld || (worldPlanning.GetWorld().mWorldState.nStick >= 1 && worldPlanning.GetWorld().mWorldState.nRope >= 1))
                                  {
                                      timmer += Time.deltaTime;
                                  }
                                  else
                                  {
                                      return Action.Result.FAILED;
                                  }
                              }

                              // If execution succeeded return "success". Otherwise return "failed".
                              if (timmer >= timeToCraft)
                              {
                                  Debug.Log("Fishing rod crafted");
                                  // Apply action and effects
                                  worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                  worldPlanning.GetWorld().mWorldState.nStick -= 1;
                                  worldPlanning.GetWorld().mWorldState.nRope -= 1;
                                  fishingRod.SetActive(true);
                                  pickaxe.SetActive(false);
                                  axe.SetActive(false);
                                  playerAnimator.SetBool("Craft", false);
                                  Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                  lightBulb.SetActive(false);
                                  return Action.Result.SUCCESS;
                              }
                              else
                              {
                                  // Action in progress
                                  if (!lightBulb.activeSelf)
                                  {
                                      Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                  }
                                  lightBulb.SetActive(true);
                                  playerAnimator.SetBool("Craft", true);
                                  return Action.Result.PROGRESS;
                              }
                          }
                          else
                          {
                              return Action.Result.FAILED;
                          }
                      })
                      { Label = "Craft fishing rod" }
                      )
                  ),
                  new Sequence(                   // FIREPLACE
                  new Action((bool nearBonfire) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FIREPLACE || mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_COOK_MUSHROOM || mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_COOK_FISH || mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_POT)
                      {

                          WorldState.WorldMask preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                          WorldState.WorldMask negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                          WorldState.WorldMask effects = WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                          WorldState.WorldMask negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;

                          if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                          {
                              if (pathfinding.GetGrid().path == null && nearestObject != worldPlanning.GetWorld().mWorldState.firePlace)
                              {
                                  nearestObject = worldPlanning.GetWorld().mWorldState.firePlace;

                                  pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                  worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                              }
                          }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go to bonfire place");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              //playerAnimator.SetBool("Craft", false);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              //playerAnimator.SetBool("Craft", true);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearFirePlace" },
                  new Selector(
                        new Action((bool craft) =>
                        {
                            if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_FIREPLACE)
                            {
                                WorldState.WorldMask preconditions;
                                WorldState.WorldMask negativePreconditions;
                                WorldState.WorldMask effects;
                                WorldState.WorldMask negativeEffects;
                                if (complexWorld)
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED;
                                    effects = WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                }
                                else
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED;
                                    effects = WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_STONE_OWNED;
                                }

                                if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                                {
                                    if (!complexWorld || (worldPlanning.GetWorld().mWorldState.nStick >= 3 && worldPlanning.GetWorld().mWorldState.nStone >= 2))
                                    {
                                        timmer += Time.deltaTime;
                                    }
                                    else
                                    {
                                        return Action.Result.FAILED;
                                    }
                                }

                                // If execution succeeded return "success". Otherwise return "failed".
                                if (timmer >= timeToCraft)
                                {
                                    Debug.Log("Bonfire crafted");
                                    worldPlanning.GetWorld().mWorldState.firePlace.SetActive(true);
                                    Instantiate(whiteParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                                    // Apply action and effects
                                    playerAnimator.SetBool("Craft", false);
                                    Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                    lightBulb.SetActive(false);
                                    worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                    worldPlanning.GetWorld().mWorldState.nStone -= 2;
                                    worldPlanning.GetWorld().mWorldState.nStick -= 3;
                                    return Action.Result.SUCCESS;
                                }
                                else
                                {
                                    // Action in progress
                                    if (!lightBulb.activeSelf)
                                    {
                                        Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                    }
                                    lightBulb.SetActive(true);
                                    playerAnimator.SetBool("Craft", true);
                                    return Action.Result.PROGRESS;
                                }
                            }
                            else
                            {
                                return Action.Result.FAILED;
                            }
                        })
                        { Label = "Craft Fireplace" },
                        new Action((bool craft) =>
                        {
                            if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_POT)
                            {
                                WorldState.WorldMask preconditions;
                                WorldState.WorldMask negativePreconditions;
                                WorldState.WorldMask effects;
                                WorldState.WorldMask negativeEffects;
                                if (complexWorld)
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    effects = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                }
                                else
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_IRON_OWNED;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    effects = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_IRON_OWNED;
                                }
                                if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                                {
                                    if (!complexWorld || worldPlanning.GetWorld().mWorldState.nIron >= 2)
                                    {
                                        timmer += Time.deltaTime;
                                    }
                                    else
                                    {
                                        return Action.Result.FAILED;
                                    }
                                }

                                // If execution succeeded return "success". Otherwise return "failed".
                                if (timmer >= timeToCraft)
                                {
                                    Debug.Log("Pot crafted");
                                    worldPlanning.GetWorld().mWorldState.pot.SetActive(true);
                                    Instantiate(whiteParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                                    // Apply action and effects
                                    worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                    worldPlanning.GetWorld().mWorldState.nIron -= 2;
                                    playerAnimator.SetBool("Craft", false);
                                    Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                    lightBulb.SetActive(false);
                                    return Action.Result.SUCCESS;
                                }
                                else
                                {
                                    // Action in progress
                                    if (!lightBulb.activeSelf)
                                    {
                                        Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                    }
                                    lightBulb.SetActive(true);
                                    playerAnimator.SetBool("Craft", true);
                                    return Action.Result.PROGRESS;
                                }
                            }
                            else
                            {
                                return Action.Result.FAILED;
                            }
                        })
                        { Label = "Craft Pot" },
                        new Action((bool cook) =>
                        {
                            if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_COOK_MUSHROOM)
                            {
                                WorldState.WorldMask preconditions;
                                WorldState.WorldMask negativePreconditions;
                                WorldState.WorldMask effects;
                                WorldState.WorldMask negativeEffects;
                                if (complexWorld)
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                    effects = WorldState.WorldMask.WORLD_STATE_NONE;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                }
                                else
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED | WorldState.WorldMask.WORLD_STATE_STICK_OWNED;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                    effects = WorldState.WorldMask.WORLD_STATE_NONE;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_MUSHROOM_OWNED;
                                }
                                if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                                {
                                    if (!complexWorld || (worldPlanning.GetWorld().mWorldState.nStick >= 1 && worldPlanning.GetWorld().mWorldState.nMushrooms >= 1))
                                    {
                                        timmer += Time.deltaTime;
                                    }
                                    else
                                    {
                                        return Action.Result.FAILED;
                                    }
                                }

                                // If execution succeeded return "success". Otherwise return "failed".
                                if (timmer >= timeToEat)
                                {
                                    Debug.Log("Cook crafted");
                                    // Apply action and effects
                                    worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                    worldPlanning.GetWorld().mWorldState.nMushrooms -= 1;
                                    worldPlanning.GetWorld().mWorldState.nStick -= 1;
                                    return Action.Result.SUCCESS;
                                }
                                else
                                {
                                    // Action in progress
                                    return Action.Result.PROGRESS;
                                }
                            }
                            else
                            {
                                return Action.Result.FAILED;
                            }
                        })
                        { Label = "Cook Mushroom" },
                        new Action((bool cook) =>
                        {
                            if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_COOK_FISH)
                            {
                                WorldState.WorldMask preconditions;
                                WorldState.WorldMask negativePreconditions;
                                WorldState.WorldMask effects;
                                WorldState.WorldMask negativeEffects;
                                if (complexWorld)
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                    effects = WorldState.WorldMask.WORLD_STATE_NONE;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE;
                                }
                                else
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_POT_OWNED | WorldState.WorldMask.WORLD_STATE_FIREPLACE_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_FISH_OWNED;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                                    effects = WorldState.WorldMask.WORLD_STATE_NONE;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_NEAR_FIREPLACE | WorldState.WorldMask.WORLD_STATE_STICK_OWNED | WorldState.WorldMask.WORLD_STATE_FISH_OWNED;
                                }
                                if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                                {
                                    if (!complexWorld || (worldPlanning.GetWorld().mWorldState.nStick >= 1 && worldPlanning.GetWorld().mWorldState.nFish >= 1))
                                    {
                                        timmer += Time.deltaTime;
                                    }
                                    else
                                    {
                                        return Action.Result.FAILED;
                                    }
                                }

                                // If execution succeeded return "success". Otherwise return "failed".
                                if (timmer >= timeToEat)
                                {
                                    Debug.Log("Cook crafted");
                                    // Apply action and effects
                                    worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                    worldPlanning.GetWorld().mWorldState.nStick -= 1;
                                    worldPlanning.GetWorld().mWorldState.nFish -= 1;
                                    playerAnimator.SetBool("Eat", false);
                                    lightBulb.SetActive(false);
                                    fishToEat.SetActive(false);
                                    return Action.Result.SUCCESS;
                                }
                                else
                                {
                                    fishToEat.SetActive(true);
                                    // Action in progress
                                    if (!lightBulb.activeSelf)
                                    {
                                        Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                    }
                                    lightBulb.SetActive(true);

                                    playerAnimator.SetBool("Eat", true);

                                    if (!fishInFirePlace.activeSelf)
                                    {
                                        Instantiate(whiteParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                                    }
                                    fishInFirePlace.SetActive(true);
                                    return Action.Result.PROGRESS;
                                }
                            }
                            else
                            {
                                return Action.Result.FAILED;
                            }
                        })
                        { Label = "Cook Fish" }
                     ) 
                  ),
                  new Sequence(                   // TENT
                  new Action((bool nearTent) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_TENT)
                      {
                          WorldState.WorldMask preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                          WorldState.WorldMask negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                          WorldState.WorldMask effects = WorldState.WorldMask.WORLD_STATE_NEAR_TENT;
                          WorldState.WorldMask negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;

                          if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                          {
                              if (pathfinding.GetGrid().path == null)
                              {
                                  nearestObject = worldPlanning.GetWorld().mWorldState.tent;

                                  pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                  worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                              }
                          }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go to tent place");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearTent" },
                  new Selector(
                        new Action((bool craft) =>
                        {
                            if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_CRAFT_TENT)
                            {
                                WorldState.WorldMask preconditions;
                                WorldState.WorldMask negativePreconditions;
                                WorldState.WorldMask effects;
                                WorldState.WorldMask negativeEffects;
                                if (complexWorld)
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_TENT;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    effects = WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_TENT;
                                }
                                else
                                {
                                    preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_TENT | WorldState.WorldMask.WORLD_STATE_WOOD_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED;
                                    negativePreconditions = WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    effects = WorldState.WorldMask.WORLD_STATE_TENT_OWNED | WorldState.WorldMask.WORLD_STATE_HUNGER;
                                    negativeEffects = WorldState.WorldMask.WORLD_STATE_NEAR_TENT | WorldState.WorldMask.WORLD_STATE_WOOD_OWNED | WorldState.WorldMask.WORLD_STATE_ROPE_OWNED;
                                }
                                if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                                {
                                    if (!complexWorld || (worldPlanning.GetWorld().mWorldState.nWood >= 2 && worldPlanning.GetWorld().mWorldState.nRope >= 1))
                                    {
                                        timmer += Time.deltaTime;
                                    }
                                    else
                                    {
                                        return Action.Result.FAILED;
                                    }
                                }

                                // If execution succeeded return "success". Otherwise return "failed".
                                if (timmer >= timeToCraft)
                                {
                                    Debug.Log("Tent crafted");
                                    worldPlanning.GetWorld().mWorldState.tent.SetActive(true);
                                    Instantiate(whiteParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                                    // Apply action and effects
                                    worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                                    worldPlanning.GetWorld().mWorldState.nWood -= 2;
                                    worldPlanning.GetWorld().mWorldState.nRope -= 1;
                                    playerAnimator.SetBool("Craft", false);
                                    Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                    lightBulb.SetActive(false);
                                    return Action.Result.SUCCESS;
                                }
                                else
                                {
                                    // Action in progress
                                    if (!lightBulb.activeSelf)
                                    {
                                        Instantiate(collectMaterialsParticles, lightBulb.transform.position, lightBulb.transform.rotation);
                                    }
                                    lightBulb.SetActive(true);
                                    playerAnimator.SetBool("Craft", true);
                                    return Action.Result.PROGRESS;
                                }
                            }
                            else
                            {
                                return Action.Result.FAILED;
                            }
                        })
                        { Label = "Craft Tent" }
                     )
                  ),
                  new Sequence(                   // GET WOOD
                  new Action((bool nearTree) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_WOOD)
                      {
                          
                            WorldState.WorldMask preconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                            WorldState.WorldMask negativePreconditions = WorldState.WorldMask.WORLD_STATE_NONE;
                            WorldState.WorldMask effects = WorldState.WorldMask.WORLD_STATE_NEAR_TREE;
                            WorldState.WorldMask negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;

                            if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                            {
                              if (worldPlanning.GetWorld().mWorldState.nTress > 0)
                              {
                                  if (pathfinding.GetGrid().path == null)
                                  {
                                      nearestObject = worldPlanning.GetWorld().mWorldState.GetNearestObject(worldPlanning.GetWorld().mWorldState.trees);
                                      nearestObject.layer = 0;
                                      foreach (Transform child in nearestObject.GetComponentsInChildren<Transform>(true))
                                      {
                                          child.gameObject.layer = LayerMask.NameToLayer("Default");
                                      }


                                      pathfinding.GetGrid().UpdateGrid();

                                      pathfinding.DoPathfinding(worldPlanning.GetWorld().mWorldState.player.transform, nearestObject.transform);

                                      worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().SetPath(pathfinding.GetGrid().path);
                                  }
                              }
                              else
                              {
                                  return Action.Result.FAILED;
                              }
                            }
                          
                          // If execution succeeded return "success". Otherwise return "failed".
                          if (worldPlanning.GetWorld().mWorldState.player.GetComponent<PlayerMovment>().GetArriveToTarget())
                          {
                              Debug.Log("Go to tree");
                              pathfinding.GetGrid().path = null;
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              pickaxe.SetActive(false);
                              axe.SetActive(true);
                              fishingRod.SetActive(false);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "GoNearTree" },
                  new Action((bool cutTree) =>
                  {
                      if (mPlan[mCurrentAction].mAction.mActionType == ActionPlanning.ActionType.ACTION_TYPE_GET_WOOD)
                      {
                          WorldState.WorldMask preconditions;
                          WorldState.WorldMask negativePreconditions;
                          WorldState.WorldMask effects;
                          WorldState.WorldMask negativeEffects;
                          if (complexWorld)
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_TREE | WorldState.WorldMask.WORLD_STATE_AXE_OWNED;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_HUNGER;
                              effects = WorldState.WorldMask.WORLD_STATE_HUNGER;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                          else
                          {
                              preconditions = WorldState.WorldMask.WORLD_STATE_NEAR_TREE | WorldState.WorldMask.WORLD_STATE_AXE_OWNED;
                              negativePreconditions = WorldState.WorldMask.WORLD_STATE_HUNGER;
                              effects = WorldState.WorldMask.WORLD_STATE_HUNGER | WorldState.WorldMask.WORLD_STATE_WOOD_OWNED;
                              negativeEffects = WorldState.WorldMask.WORLD_STATE_NONE;
                          }
                          if ((worldPlanning.GetWorld().mWorldState.worldMask & preconditions) == preconditions && (~(worldPlanning.GetWorld().mWorldState.worldMask) & negativePreconditions) == negativePreconditions)
                          {
                              timmer += Time.deltaTime;
                          }

                          // If execution succeeded return "success". Otherwise return "failed".
                          if (timmer >= timeToHardWorking)
                          {
                              Debug.Log("Cut tree");
                              Instantiate(whiteParticles, nearestObject.transform.position, nearestObject.transform.rotation);
                              Instantiate(logCut,nearestObject.transform.position, nearestObject.transform.rotation).layer = LayerMask.NameToLayer("Unwalkable");
                              pathfinding.GetGrid().UpdateGrid();
                              nearestObject.SetActive(false);
                              nearestObject.tag = "Untagged";
                              playerAnimator.SetBool("CuttingWood", false);
                              // Apply action and effects
                              worldPlanning.GetWorld().mWorldState.worldMask = (worldPlanning.GetWorld().mWorldState.worldMask | effects) & ~(negativeEffects);
                              worldPlanning.GetWorld().mWorldState.nWood++;
                              worldPlanning.GetWorld().mWorldState.nTress -= 1;
                              return Action.Result.SUCCESS;
                          }
                          else
                          {
                              // Action in progress
                              playerAnimator.SetBool("CuttingWood",true);
                              return Action.Result.PROGRESS;
                          }
                      }
                      else
                      {
                          return Action.Result.FAILED;
                      }
                  })
                  { Label = "CutTree" }
                  )
                //... Action3
                //... ActionN
                ) // Selector
              ) // Sequence
            ) // Repeater
          ) // Sequence
        );

        // attach the debugger component if executed in editor (helps to debug in the inspector) 
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = mBehaviorTree;
#endif

        mBehaviorTree.Start();
    }

    /****************************************************************************/

    // Update is called once per frame
    void Update()
    {

    }

    /****************************************************************************/

    public void OnDestroy()
    {
        StopBehaviorTree();
    }


    /****************************************************************************/

    public void StopBehaviorTree()
    {
        if (mBehaviorTree != null && mBehaviorTree.CurrentState == Node.State.ACTIVE)
        {
            mBehaviorTree.Stop();
        }
    }
    /****************************************************************************/



}
