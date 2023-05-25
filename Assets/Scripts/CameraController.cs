using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    // Update is called once per frame
    void LateUpdate()
    {
        //Move the camera when the hero reaches the boundaries
        CheckCameraBounds();
    }



    private void CheckCameraBounds()
    {
        //transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z + 4);
        if (player.transform.position.x - transform.position.x > 3.5f)
        {
            transform.position = new Vector3(player.transform.position.x - 3.5f, transform.position.y, transform.position.z);
        }
        if (player.transform.position.x - transform.position.x < -3.5f)
        {
            transform.position = new Vector3(player.transform.position.x + 3.5f, transform.position.y, transform.position.z);
        }
        if (player.transform.position.z - transform.position.z < -6)
         {
             transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + 6);
         }
        if (player.transform.position.z - transform.position.z > -2)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + 2);
        }
    }
}
