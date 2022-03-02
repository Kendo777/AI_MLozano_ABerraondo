using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public GameObject objectToFollow;
    public GameObject ObjectToLook;
    public float speed = 0.5f;


    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, objectToFollow.transform.position, speed * Time.deltaTime);
        Vector3 targetDirec = ObjectToLook.transform.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirec), Time.time * speed);
    }
}
