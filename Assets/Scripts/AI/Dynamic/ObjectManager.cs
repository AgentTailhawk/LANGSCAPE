using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // Variables
    /* For Keeping Track of Objects */
    const int MaxObjects = 1;
    //ToyBox box;
    //List<GameObject> items;
    GameObject toy;

    [Header("Background Scripts")]
    /* Script for Spawn and Despawning Objects */
    public Spawner spawner;

    [Header("Shape Creation")]
    public ShapeCreator shapeCreator; // Assign this in the Inspector



    /*
        Singleton
    */
    private static ObjectManager _instance;
    public static ObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("BackgroundManager::Instance - BackgroundManager is null");
            }

            return _instance;
        }
    }


    private void Awake()
    {
        // Singleton
        _instance = this;

        // Keeps Track of Objects in World 
        //box = new ToyBox();
        //items = new List<GameObject>();
        toy = null;

    }

    // For Now, Should Spawn New Shape and Despawn Old Shape
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

        // Spawn Object
        spawner.Spawn(toy, true);

        // Summon Langscape Error
        LangscapeError.Instance.ThrowUserError(error);
    }
}
