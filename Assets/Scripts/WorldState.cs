using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public GameObject[] trees;
    public GameObject[] rope;
    public GameObject[] sticks;
    public GameObject[] stones;
    public GameObject[] iron;
    public GameObject[] mushrooms;
    public GameObject[] water;
    public GameObject craftingTables;
    public GameObject firePlace;
    public GameObject tent;
    public GameObject pot;
    public GameObject player;


    public int nStick = 0;
    public int nStone = 0;
    public int nRope = 0;
    public int nTress = 0;
    public int nWood = 0;
    public int nMushrooms = 0;
    public int nFish = 0;
    public int nIron = 0;
    /***************************************************************************/

    public enum WorldMask
    {
        WORLD_STATE_NONE = 0,
        WORLD_STATE_AXE_OWNED = 1,
        WORLD_STATE_PICK_AXE_OWNED = 2,
        WORLD_STATE_NEAR_STICK = 4,
        WORLD_STATE_NEAR_STONE = 8,
        WORLD_STATE_NEAR_ROPE = 16,
        WORLD_STATE_NEAR_CRAFTING_TABLE = 32,
        WORLD_STATE_NEAR_TREE = 64,
        WORLD_STATE_FIREPLACE_OWNED = 128,
        WORLD_STATE_NEAR_FIREPLACE = 256,
        WORLD_STATE_NEAR_TENT = 512,
        WORLD_STATE_TENT_OWNED = 1024,
        WORLD_STATE_POT_OWNED = 2048,
        WORLD_STATE_HUNGER = 4096,
        WORLD_STATE_NEAR_WATER = 8192,
        WORLD_STATE_NEAR_MUSHROOM = 16384,
        WORLD_STATE_FISHING_ROD_OWNED = 32768,
        WORLD_STATE_NEAR_IRON = 65536,
        WORLD_STATE_STICK_OWNED = 131072,
        WORLD_STATE_STONE_OWNED = 262144,
        WORLD_STATE_ROPE_OWNED = 524288,
        WORLD_STATE_WOOD_OWNED = 1048576,
        WORLD_STATE_IRON_OWNED = 2097152,
        WORLD_STATE_FISH_OWNED = 4194304,
        WORLD_STATE_MUSHROOM_OWNED = 8388608
    }

    public WorldMask worldMask;
    /***************************************************************************/

    public WorldState()
    {
        trees = GameObject.FindGameObjectsWithTag("Tree");
        rope = GameObject.FindGameObjectsWithTag("Rope");
        sticks = GameObject.FindGameObjectsWithTag("Stick");
        stones = GameObject.FindGameObjectsWithTag("Stone");
        iron = GameObject.FindGameObjectsWithTag("Iron");
        mushrooms = GameObject.FindGameObjectsWithTag("Mushroom");
        water = GameObject.FindGameObjectsWithTag("Water");
        craftingTables = GameObject.FindGameObjectWithTag("CraftingTable");
        firePlace = GameObject.FindGameObjectWithTag("FirePlace");
        tent = GameObject.FindGameObjectWithTag("Tent");
        pot = GameObject.FindGameObjectWithTag("Pot");
        player = GameObject.FindGameObjectWithTag("Player");

        nTress = trees.Length;
    }

    public GameObject GetNearestObject(GameObject[] objercts)
    {
        GameObject nearObject = null;
        float distance = 0.0f;
        float aux = 0.0f;
        foreach (GameObject currentObject in objercts)
        {
            if (currentObject.tag != "Untagged")
            {
                if (distance != 0.0f)
                {
                    aux = Vector3.Distance(player.transform.position, currentObject.transform.position);
                    if (aux < distance)
                    {
                        distance = aux;
                        nearObject = currentObject;
                    }
                }
                else
                {
                    distance = Vector3.Distance(player.transform.position, currentObject.transform.position);
                    nearObject = currentObject;
                }
            }
        }
        return nearObject;
    }
    public void copyState(WorldState n, WorldMask m)
    {
        nStick = n.nStick;
        nStone = n.nStone;
        nRope = n.nRope;
        nTress = n.nTress;
        nWood = n.nWood;
        nMushrooms = n.nMushrooms;
        nFish = n.nFish;
        nIron = n.nIron;
        worldMask = m;
    }
    
    public bool compareState(WorldState n)
    {
        if(nStick == n.nStick && nStone == n.nStone && nRope == n.nRope && nWood == n.nWood && nMushrooms == n.nMushrooms && nFish == n.nFish && nIron == n.nIron && worldMask == n.worldMask)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

}
