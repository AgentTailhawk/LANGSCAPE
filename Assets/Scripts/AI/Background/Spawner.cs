using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Cameras")]
    /* OVR >> OVRCameraRig 1 >> TrackingSpace >> CenterEyeAnchor */
    public Camera oculusCamera;
    /* Windows >> Camera */
    public Camera windowsCamera;

    // When Called Should Spawn Item In front of player
    public GameObject Spawn(GameObject itemPrefab, bool Extra = False)
    {
        Vector3 playerPos;
        Vector3 playerDir;
        Quaternion playerRot;

        // 1 - 5 - 7 - 10
        float spawnDist = 0;

#if !UNITY_STANDALONE_WIN
        playerPos = oculusCamera.transform.position;
        playerDir = oculusCamera.transform.forward;
        playerRot = oculusCamera.transform.rotation;
        spawnDist = 5;    
#else
        playerPos = windowsCamera.transform.position;
        playerDir = windowsCamera.transform.forward;
        playerRot = windowsCamera.transform.rotation;
        spawnDist = 10;
#endif

        Vector3 spawnPos = playerPos + playerDir * spawnDist;

        // Currently: If used by Object Manager = True, Otherwise False
        if(Extra){

        }else{
            return (GameObject)Instantiate(itemPrefab, spawnPos, playerRot);
        }
    }
    public void Despawn(GameObject gameObject)
    {
        Destroy(gameObject);
        Debug.Log("Item Destroyed");
    }
}
