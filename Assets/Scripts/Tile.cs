using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Properties

    [SerializeField, Range(0f, 1f)]
    private float disappearDuration = 0.25f;

    PrefabInstancePool<Tile> pool;

    private float disappearProgress;

    private FallingState falling;

    #endregion Properties

    #region FallingState Private Struct

    [System.Serializable]
    private struct FallingState
    {
        public float fromY, toY, duration, progress;
    }

    #endregion

    #region Methods

    public Tile Spawn(Vector3 position)
    {
        Tile instance = pool.GetInstance(this);
        instance.pool = pool;
        instance.transform.localPosition = position;

        instance.transform.localScale = GameDefaults.tileScale;
        instance.disappearProgress = -1f;
        instance.falling.progress = -1f;
        instance.enabled = false; //do not allow tile to update on spawn.

        return instance;
    }

    private void Update() //progress scale to 0 and despawn when done.
    {
        //check if this tile is disappearing
        if (disappearProgress >= 0f)
        {
            disappearProgress += Time.deltaTime;
            if (disappearProgress >= disappearDuration)
            {
                Despawn();
                return;
            }
            transform.localScale = GameDefaults.tileScale * 
                                       (1f - (disappearProgress / disappearDuration));
        }

        //check if this tile is falling
        if (falling.progress >= 0f)
        {
            Vector3 position = transform.localPosition;
            falling.progress += Time.deltaTime;
            
            if (falling.progress >= falling.duration)
            {
                falling.progress = -1f;
                position.y = falling.toY;
                if (disappearProgress < 0f) //disable updating if the
                                            //tile is not disappearing also
                {
                    enabled = false;
                }
            }
            else
            {
                position.y = Mathf.Lerp(falling.fromY, falling.toY,
                    falling.progress / falling.duration);
            }
            transform.localPosition = position;
        }
    }

    public float Fall(float toY, float speed)
    {
        falling.fromY = transform.localPosition.y;
        falling.toY = toY;
        falling.duration = (falling.fromY - toY) / speed;
        falling.progress = 0f;
        enabled = true;
        return falling.duration;
    }

    public float Disappear()
    {
        disappearProgress = 0f;
        enabled = true;
        return disappearDuration;
    }

    public void Despawn()
    {
        pool.Recycle(this);
    }

    #endregion
}
