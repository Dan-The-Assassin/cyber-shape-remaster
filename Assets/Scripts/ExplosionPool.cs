using UnityEngine;
using UnityEngine.Pool;

public class ExplosionPool : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    public static ObjectPool<GameObject> Instance { get; private set; }

    private void Awake()
    {
        Instance = new ObjectPool<GameObject>(CreatePooledItem, OnGetPooledItem, OnReleasePooledItem, OnDestroyPooledItem);
    }

    private GameObject CreatePooledItem()
    {
        return Instantiate(explosionPrefab);
    }

    private void OnGetPooledItem(GameObject explosion)
    {
        explosion.SetActive(true);
    }

    private void OnReleasePooledItem(GameObject explosion)
    {
        explosion.SetActive(false);
    }

    private void OnDestroyPooledItem(GameObject explosion)
    {
        Destroy(explosion);
    }
}
