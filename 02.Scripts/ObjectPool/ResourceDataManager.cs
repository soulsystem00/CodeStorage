using System;
using System.Collections.Generic;
using UnityEngine;


public class ResourceDataManager : MonoBehaviour
{
    static private bool initState = false;

    // static public GameObject Sample;
    static public GameObject cannonBall;
    static public GameObject islandStat;
    static public GameObject islandSkill;
    static public GameObject islandBullet;
    static public GameObject islandCannonBall;
    static public GameObject supplyBox;
    static public GameObject electric;

    public static void LoadResourcesData()
    {
        if (!initState)
        {
            initState = true;
            // Sample = Resources.Load("Sample") as GameObject;
            cannonBall = Resources.Load("Ship/CannonBall") as GameObject;
            islandStat = Resources.Load("Island/IslandStat") as GameObject;
            islandSkill = Resources.Load("Island/IslandSkill") as GameObject;
            islandBullet = Resources.Load("Island/IslandBullet") as GameObject;
            islandCannonBall = Resources.Load("Island/CannonBall") as GameObject;
            supplyBox = Resources.Load("SupplyBox") as GameObject;
            electric = Resources.Load("Electric") as GameObject;
        }
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position, Quaternion rotate)
    {
        GameObject obj = ObjectPoolManager.Instance.Instantiate(resource, position, rotate);
        T script = obj.GetComponent<T>();

        return script;
    }

    public static T CreateObjectAndComponent<T>(GameObject resource, Vector3 position)
    {
        return CreateObjectAndComponent<T>(resource, position, Quaternion.identity);
    }

    public static T CreateObjectAndComponent<T>(GameObject resource)
    {
        return CreateObjectAndComponent<T>(resource, Vector3.zero, Quaternion.identity);
    }
}