using UnityEngine;

/// <summary>
/// Destorys a gameobject when the particle system stops playing.
/// </summary>
public class DestroyOnEnd : MonoBehaviour
{
    #region Properties

    private ParticleSystem particle;

    #endregion

    #region Unity Methods

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (particle.isStopped)
        {
            Destroy(this.gameObject);
        }
    }

    #endregion
}