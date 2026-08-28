using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset;


     void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10f; //VERY IMPORTANT FOR 2D camera 

        transform.position = desiredPosition;

        //transform.position = Vector3.Lerp(
            //transform.position, desiredPosition, smoothSpeed * Time.deltaTime );
    }

}

