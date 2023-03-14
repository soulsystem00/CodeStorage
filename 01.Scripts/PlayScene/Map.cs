using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] GameObject cube;
    [SerializeField] float size;

    void Awake()
    {
        function();
    }

    private void function()
    {
        for (float i = -radius; i < radius; i = i + size)
        {
            for (float j = -radius; j < radius; j = j + size)
            {
                var pos = new Vector3(i, -0.5f, j);
                if (pos.magnitude <= radius)
                {
                    var obj = Instantiate(cube, pos, Quaternion.identity * Quaternion.Euler(90f, 0f, 0f));
                    obj.transform.SetParent(transform);
                    obj.transform.localScale = new Vector3(size, size, size);
                }
            }
        }
    }
}
