using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMainMenu();
        }
        else
        {
            Debug.LogWarning("SoundManager Instance is NULL");
        }
    }
    public void PlayGame()
    {
        //SoundManager.Instance.ResetDialogue();
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}