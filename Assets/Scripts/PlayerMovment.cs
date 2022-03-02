using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private List<NodePathfinding> path;
    public float speed = 2.5f;
    int targetIndex = 0;
    Vector3 currentWaypoint = Vector3.zero;
    private bool arriveToTaget = false;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(path != null)
        {
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Count)
                {
                    arriveToTaget = true;
                    targetIndex = 0;
                    path = null;

                }
                else
                {
                    currentWaypoint = path[targetIndex].mWorldPosition;
                    currentWaypoint.y = transform.position.y;
                }

            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            transform.LookAt(currentWaypoint);
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }


    }
    public void SetPath(List<NodePathfinding> p)
    {
        if(path!=null)  path.Clear();
        path = p;
        arriveToTaget = false;
        currentWaypoint = path[targetIndex].mWorldPosition;
        currentWaypoint.y = transform.position.y;
    }
    public bool GetArriveToTarget()
    {
        return arriveToTaget;
    }
}
