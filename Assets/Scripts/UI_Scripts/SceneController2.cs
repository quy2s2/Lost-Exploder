using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Thêm thư viện UI

public class SceneController2 : MonoBehaviour
{
    public static SceneController2 instance;
    [SerializeField] Animator transitionAnim;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Lấy CanvasGroup trên cùng GameObject hoặc Panel hiệu ứng
            canvasGroup = transitionAnim.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false; // Mặc định không chặn click
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        // Bật chặn click khi bắt đầu hiệu ứng chuyển scene
        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;

        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        // Chờ scene tải xong hoàn toàn
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        transitionAnim.SetTrigger("Start");

        // Tắt chặn click để người chơi tương tác với UI của scene mới
        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}