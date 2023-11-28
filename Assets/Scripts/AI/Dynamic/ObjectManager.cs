using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    const int MaxObjects = 1;
    GameObject toy;

    [Header("Background Scripts")]
    public Spawner spawner;

    [Header("Shape Creation")]
    public ShapeCreator shapeCreator; // Assign this in the Inspector
    [Header("Camera")]
    public Camera oculusCamera;
    public Camera windowsCamera;
    private static ObjectManager _instance;
    public static ObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ObjectManager::Instance - ObjectManager is null");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        toy = null;
    }

    public void Execute(string jsonString)
    {
        int error = LangscapeError.CMD_VALID.code;
        
        // Despawn Object if Currently Existing
        if (toy != null)
        {
            spawner.Despawn(toy);
        }

        // Call DynamicObject Method, Returns GameObject
        toy = shapeCreator.CreateMeshFromJson(jsonString);

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

        // Set toy position and rotation
        if (toy != null)
        {
            toy.transform.position = spawnPos;
            toy.transform.rotation = playerRot;
            // If you want to set the direction as well, you might need to adjust the code accordingly.
            // toy.transform.forward = playerDir;
        }
        else
        {
            Debug.LogError("Toy is null. Make sure it's instantiated or obtained correctly.");
        }

        // Spawn Object
        // spawner.Spawn(toy, true);

        // Summon Langscape Error
        LangscapeError.Instance.ThrowUserError(error);
    }
}