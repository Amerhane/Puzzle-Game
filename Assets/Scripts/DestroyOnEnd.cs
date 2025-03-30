using UnityEngine;

public class DestroyOnEnd : MonoBehaviour
{
    private ParticleSystem particle;

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
}