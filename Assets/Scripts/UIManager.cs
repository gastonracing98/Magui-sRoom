using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static UIManager inst;
    public GameObject PauseScreen;

    public Button ContinueButton;
    public Button MainMenuButton;
    private void Awake()
    {
        inst = this;

        ContinueButton.onClick.AddListener(HidePauseScreen);

        MainMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f; // Asegúrate de reanudar el tiempo antes de cambiar de escena
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScreen");
        });


    }
    public void ShowPauseScreen()
    {
        PauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
    public void HidePauseScreen()
    {
        PauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        Time.timeScale = 1f; 
    }
}
