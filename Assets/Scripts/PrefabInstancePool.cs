using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object pool which holds the tile gameobjects.
/// Had to research this.
/// </summary>
/// <typeparam name="T">Tiles</typeparam>
public struct PrefabInstancePool<T> where T : MonoBehaviour
{
    #region Properties

    private Stack<T> pool;

    #endregion

    #region Methods

    public T GetInstance(T prefab)
    {
        if (pool == null)
        {
            pool = new();
        }
    #if UNITY_EDITOR
        else if (pool.TryPeek(out T i) && !i)
        {
            //Instances destroyed, assuming due to exiting play mode.
            pool.Clear();
        }
    #endif

        if (pool.TryPop(out T instance))
        {
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Object.Instantiate(prefab);
        }

        return instance;
    }

    public void Recycle(T instance)
    {
    #if UNITY_EDITOR
        if (pool == null)
        {
            //Pool lost, assuming due to hot reload.
            Object.Destroy(instance.gameObject);
            return;
        }
    #endif
        pool.Push(instance);
        instance.gameObject.SetActive(false);
    }

    #endregion
}
