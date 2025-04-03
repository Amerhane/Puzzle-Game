using UnityEngine;

/// <summary>
/// Transfers the input from the player to the game controller.
/// </summary>
public class InputController : MonoBehaviour
{
    #region Properties

    [Header("Game Controller")]
    [SerializeField]
    private GameController gameController;

    //mobile input
    private Vector3 firstTouchPosition;
    private Vector3 lastTouchPosition;

    //computer input
    private Vector3 firstMousePosition;
    private Vector3 lastMousePosition;

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
            gameController.Process();
        }
        //else if(Input.GetKeyDown(KeyCode.Space)) //for testing
        //{
        //    gameController.StartNewGame();
        //}
    }

    #endregion

    #region Methods

    private void HandleInput()
    {
        //Mobile Inputs
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

        /**
         * Uncomment below when using computer or testing in unity client through the "game" mode.
         * Leave commented when building to mobile or using unity simulator.
         */

        //Computer Inputs
        //if (Input.GetMouseButtonDown(0))
        //{
        //    firstMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    lastMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

        //    gameController.EvaluateDrag(firstMousePosition, lastMousePosition);
        //}
    }

    #endregion
}
