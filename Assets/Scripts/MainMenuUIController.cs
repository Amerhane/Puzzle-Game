using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the UI of the main menu of the game.
/// </summary>
public class MainMenuUIController : MonoBehaviour
{
    #region Properties

    [Header("Menu Panels")]
    [SerializeField]
    private GameObject mainMenuItems;
    [SerializeField]
    private GameObject creditMenuItems;

    [Header("Audio")]
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioClip swapToPanelClip;
    [SerializeField]
    private AudioClip swapFromPanelClip;

    #endregion

    #region Unity Methods

    private void Start()
    {
        mainMenuItems.SetActive(true);
        creditMenuItems.SetActive(false);
    }

    #endregion

    #region Button Events

    public void OnPlayButtonPress()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCreditsButtonPress()
    {
        sfxSource.PlayOneShot(swapToPanelClip);
        mainMenuItems.SetActive(false);
        creditMenuItems.SetActive(true);
    }

    public void OnBackButtonPress()
    {
        sfxSource.PlayOneShot(swapFromPanelClip);
        mainMenuItems.SetActive(true);
        creditMenuItems.SetActive(false);
    }

    #endregion
}