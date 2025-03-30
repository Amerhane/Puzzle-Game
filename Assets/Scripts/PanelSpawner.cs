using System.Collections;
using UnityEngine;

public class PanelSpawner : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float scaleFactor = 0.1f;

    private float maxScale = 1f;

    [SerializeField]
    private bool win;
    [SerializeField]
    private GameObject particleEffect;

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
        if (win)
        {
            Instantiate(particleEffect, this.transform.localPosition + (Vector3.right * 4),
                Quaternion.identity);
            Instantiate(particleEffect, this.transform.localPosition + (Vector3.left * 4),
                Quaternion.identity);
        }
    }
}