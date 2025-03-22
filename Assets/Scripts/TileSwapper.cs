using UnityEngine;

[System.Serializable]
public class TileSwapper
{
    #region Properties

    [SerializeField, Range(0.1f, 10f)]
    private float duration = 0.25f; //length of animation

    [SerializeField, Range(0f, 1f)]
    private float maxDepthOffset = 0.5f; //controls Z dimention movement
                                         //to avoid tiles intersecting

    private Tile tileA, tileB;

    private Vector3 positionA, positionB;

    private float progess = -1f; //-1 means inactive

    private bool reverse;

    #endregion

    #region Methods

    public void Update()
    {
        if (progess < 0f)
        {
            return; //this is inactive
        }

        progess += Time.deltaTime;
        if (progess >= duration)
        {
            if (reverse)
            {
                progess -= duration;
                reverse = false;

                Tile temp = tileA;
                tileA = tileB;
                tileB = temp;
            }
            else
            {
                progess = -1f;
                tileA.transform.localPosition = positionB;
                tileB.transform.localPosition = positionA;
                return;
            }
        }

        //Linearly interpolate both tile positions.
        float time = progess / duration;
        //Displace tiles in the z dimention to prevent overlap.
        float z = Mathf.Sin(Mathf.PI * time) * maxDepthOffset;

        Vector3 position = Vector3.Lerp(positionA, positionB, time);
        position.z = -z;
        tileA.transform.localPosition = position;

        position = Vector3.Lerp(positionA, positionB, 1f - time);
        position.z = z;
        tileB.transform.localPosition = position;
    }

    public float Swap(Tile a, Tile b, bool reverse)
    {
        tileA = a;
        tileB = b;
        positionA = a.transform.localPosition;
        positionB = b.transform.localPosition;
        this.reverse = reverse;
        progess = 0f;

        if (reverse)
        {
            return 2f * duration; //need twice as long to play animation
        }
        else
        {
            return duration;
        }
    }

    #endregion
}