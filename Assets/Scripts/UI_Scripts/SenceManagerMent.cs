using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Gọi hàm này khi bấm nút PLAY
    public void PlayGame()
    {
        // Chuyển sang scene1 (Đảm bảo tên Scene trong ngoặc trùng 100% với tên file Scene của bạn)
        SceneManager.LoadScene("SampleScence");
    }

    // Nếu bạn muốn dùng theo cách load theo tên hoặc index biến linh hoạt
    public void QuitGame()
    {
        Debug.Log("Đã thoát game!");
        Application.Quit();
    }
}