using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyLoader : MonoBehaviour
{
    [SerializeField] private string easyScene = "Game_Easy";
    [SerializeField] private string mediumScene = "Game_Medium";
    [SerializeField] private string hardScene = "Game_Hard";
    [SerializeField] private AudioSource clickSound;

    public void LoadEasy()
    {
        PlaySoundAndLoad(easyScene);
    }

    public void LoadMedium()
    {
        PlaySoundAndLoad(mediumScene);
    }

    public void LoadHard()
    {
        PlaySoundAndLoad(hardScene);
    }

    public void LoadMainMenu()
    {
        PlaySoundAndLoad("Main Menu");
    }

    private void PlaySoundAndLoad(string sceneName)
    {
        if (clickSound != null)
        {
            clickSound.Play();
            DontDestroyOnLoad(clickSound.gameObject); // 🔊 keep it alive across scene
            Destroy(clickSound.gameObject, clickSound.clip.length); // cleanup after sound finishes
        }

        SceneManager.LoadScene(sceneName);
    }
}
