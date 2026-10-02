using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0; // Đóng băng thời gian
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1; // Khôi phục lại thời gian khi tiếp tục
    }

    // Nút ở giữa (Mũi tên xoay)
    public void Restart()
    {
        Time.timeScale = 1; // Khôi phục lại thời gian trước khi tải lại màn chơi
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Tải lại Scene hiện tại
    }

    public void Home()
    {
        Time.timeScale = 1; // Khôi phục lại thời gian trước khi về Menu
        SceneManager.LoadScene("Menu");
    }
}