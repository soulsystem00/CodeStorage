using UnityEngine;

public class ObjPoolInitializer : MonoBehaviour
{
    void Awake()
    {
        ResourceDataManager.LoadResourcesData();
        InitObjectPool();
    }
    private void InitObjectPool()
    {
        ObjectPoolManager objectPoolManager = ObjectPoolManager.Instance;
        // objectPoolManager.CreatePool(ResourceDataManager.zeusThunder, 10, 2);
        objectPoolManager.CreatePool(ResourceDataManager.cannonBall, 30, 10);
        objectPoolManager.CreatePool(ResourceDataManager.islandStat, 20, 10);
        objectPoolManager.CreatePool(ResourceDataManager.islandSkill, 20, 10);
        objectPoolManager.CreatePool(ResourceDataManager.islandBullet, 20, 10);
        objectPoolManager.CreatePool(ResourceDataManager.islandCannonBall, 20, 20);
        objectPoolManager.CreatePool(ResourceDataManager.supplyBox, 10, 5);
        objectPoolManager.CreatePool(ResourceDataManager.electric, 30, 10);
    }
}
