//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Unit : MonoBehaviour
//{
//    public Transform target;
//    float speed = 30;
//    Vector3[] path;
//    int targetIndex;

//    private void Awake()
//    {
//        PathManager.RequestPath(transform.position,target.transform.position,OnPathFound);
//    }

//    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
//    {
//        if (pathSuccessful)
//        {
//            path = newPath;
//            StopCoroutine("FollowPath");
//            StartCoroutine("FollowPath");
//        }
//    }

//    IEnumerator FollowPath()
//    {
//        Vector3 currentWaypint = path[0];

//        while (true)
//        {
//            if(transform.position == currentWaypint)
//            {
//                targetIndex++;
//                if(targetIndex >= path.Length)
//                {
//                    yield break;
//                }
//                currentWaypint = path[targetIndex];
//            }
//            transform.position = Vector3.MoveTowards(transform.position, currentWaypint, Time.deltaTime * speed);
//            yield return null;
//        }
//    }

//    public void OnDrawGizmos()
//    {
//        if(path != null)
//        {
//            for(int i = targetIndex; i <path.Length; i++)
//            {
//                Gizmos.color = Color.black;
//                Gizmos.DrawCube(path[i], Vector3.one);

//                if(i == targetIndex)
//                {
//                    Gizmos.DrawLine(transform.position, path[i]);
//                }
//                else
//                {
//                    Gizmos.DrawLine(path[i - 1], path[i]);
//                }
//            }
//        }
//    }

//}
