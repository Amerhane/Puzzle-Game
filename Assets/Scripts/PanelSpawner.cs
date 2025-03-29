using System.Collections;
using UnityEngine;

public class PanelSpawner : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float scaleFactor = 0.1f;

    private float maxScale = 1f;

    private void OnEnable()
    {
        StartCoroutine(Scale());
    }

    private IEnumerator Scale()
    {
        do
        {
            transform.localScale += Vector3.one * Time.deltaTime * scaleFactor;
            yield return null;
        }
        while (maxScale > transform.localScale.x);

        //in case scale factor scales panel too much.
        transform.localScale = Vector3.one;
    }
}