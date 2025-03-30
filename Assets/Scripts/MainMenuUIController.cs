using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIController : MonoBehaviour
{
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

    private void Start()
    {
        mainMenuItems.SetActive(true);
        creditMenuItems.SetActive(false);
    }

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
}