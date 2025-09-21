using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;

    public void Play()
    {
        if (clickSound != null)
        {
            clickSound.Play();
            DontDestroyOnLoad(clickSound.gameObject); // keep playing after scene change
        }

        SceneManager.LoadScene("Difficulty");
    }

    public void Exit()
    {
        if (clickSound != null)
        {
            clickSound.Play();
            DontDestroyOnLoad(clickSound.gameObject);
        }

        Application.Quit();
    }
}
