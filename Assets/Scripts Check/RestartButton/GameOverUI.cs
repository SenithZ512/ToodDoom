using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ใช้ใน Scene GameOver
/// ผูกปุ่ม Restart กับ method RestartGame()
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Scene Settings")]
    public int gameSceneIndex = 1;   // index ของ SceneCheck ใน Build Settings

    void Start()
    {
        // ปลดล็อค cursor เมื่อมาถึง Game Over Scene
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        // Reset timescale ถ้าเคย pause ไว้
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}