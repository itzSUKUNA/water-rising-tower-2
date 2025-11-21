using UnityEngine;
using UnityEngine.SceneManagement;
public class Scenemanager : MonoBehaviour
{
    public void loadlv(string name)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    // Restart the currently active scene (useful for a UI Restart button)
    public void RestartLevel()
    {
        // Ensure timeScale is normal in case the game was paused (e.g., on death)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 
