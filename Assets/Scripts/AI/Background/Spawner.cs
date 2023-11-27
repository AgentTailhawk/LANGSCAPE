using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Cameras")]
    public Camera oculusCamera;
    public Camera windowsCamera;

    public ShapeCreator shapeCreator; // Reference to your ShapeCreator

    public GameObject Spawn(GameObject itemPrefab, bool extra = false)
    {


        Vector3 playerPos;
        Vector3 playerDir;
        Quaternion playerRot;

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


        return Instantiate(itemPrefab, spawnPos, playerRot);

    }

    public void Despawn(GameObject gameObject)
    {
        Destroy(gameObject);
        Debug.Log("Item Destroyed");
    }
}
