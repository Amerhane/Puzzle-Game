using System.Collections;
using UnityEngine;

/// <summary>
/// Scales a panel up to full size when enabled,
/// and then plays the particle system attached to the panel
/// at the end, if the player won.
/// </summary>
public class PanelSpawner : MonoBehaviour
{
    #region Properties

    [Header("Adjustable Values")]
    [SerializeField, Min(0f)]
    private float scaleFactor = 0.4f;
    [SerializeField]
    private bool win;

    [Header("Particle System")]
    [SerializeField]
    private GameObject particleEffect;

    private float maxScale = 2f;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        StartCoroutine(Scale());
    }

    #endregion

    #region Coroutine

    private IEnumerator Scale()
    {
        do
        {
            transform.localScale += Vector3.one * Time.deltaTime * scaleFactor;
            yield return null;
        }
        while (maxScale > transform.localScale.x);

        //in case scale factor scales panel too much.
        transform.localScale = Vector3.one * 2f;

        if (win) //if the player won, play the particle effects
        {
            Instantiate(particleEffect, this.transform.localPosition + (Vector3.right * 4),
                Quaternion.identity);
            Instantiate(particleEffect, this.transform.localPosition + (Vector3.left * 4),
                Quaternion.identity);
        }
    }

    #endregion
}