using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverBackground;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        RectTransform rect = gameOverBackground.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(0f, -1500f);
    }

    public void TriggerGameOverBackground()
    {
        RectTransform rect = gameOverBackground.GetComponent<RectTransform>();

        LeanTween.moveY(rect, 0f, 1f)
            .setEaseOutBounce();
    }
}