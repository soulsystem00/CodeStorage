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
        objectPoolManager.CreatePool(ResourceDataManager.zeusThunder, 10, 2);
        objectPoolManager.CreatePool(ResourceDataManager.CharlesImage, 10, 2);
        objectPoolManager.CreatePool(ResourceDataManager.zeusCircleThunder, 10, 2);
        objectPoolManager.CreatePool(ResourceDataManager.explosionBarrel, 10, 5);
        objectPoolManager.CreatePool(ResourceDataManager.rollingBarrel, 10, 5);
        objectPoolManager.CreatePool(ResourceDataManager.turret, 10, 10);
        objectPoolManager.CreatePool(ResourceDataManager.bullet, 20, 10);
        objectPoolManager.CreatePool(ResourceDataManager.mine, 30, 10);
        objectPoolManager.CreatePool(ResourceDataManager.bombard, 10, 5);
        objectPoolManager.CreatePool(ResourceDataManager.sniper, 10, 5);
        objectPoolManager.CreatePool(ResourceDataManager.bombBot, 20, 10);
        objectPoolManager.CreatePool(ResourceDataManager.meteor, 10, 5);
        objectPoolManager.CreatePool(ResourceDataManager.itemBox, 10, 5);
        objectPoolManager.CreatePool(ResourceDataManager.booster, 30, 10);
        objectPoolManager.CreatePool(ResourceDataManager.timer, 1, 1);
        objectPoolManager.CreatePool(ResourceDataManager.followingThunder, 5, 3);
    }
}
