using UnityEngine;

public class InputController : MonoBehaviour
{
    #region Properties

    [SerializeField]
    private GameController gameController;

    private Vector3 firstTouchPosition;
    private Vector3 lastTouchPosition;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        gameController.StartNewGame();
    }

    private void Update()
    {
        if (gameController.IsPlaying())
        {
            if(!gameController.IsBusy())
            {
                HandleInput();
            }
            gameController.DoWork();
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            gameController.StartNewGame();
        }
    }

    #endregion

    #region Methods

    private void HandleInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase.Equals(TouchPhase.Began))
            {
                firstTouchPosition = touch.position;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase.Equals(TouchPhase.Moved))
            {
                lastTouchPosition = touch.position;
            }
            else if (touch.phase.Equals(TouchPhase.Ended))
            {
                lastTouchPosition = touch.position;
                gameController.EvaluateDrag(firstTouchPosition,
                    lastTouchPosition);
            }
        }
    }

    #endregion
}
