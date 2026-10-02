using UnityEngine;
using UnityEngine.UI;

public class Player_Collectables : MonoBehaviour
{
    private int currentCoins;
    private int currentGreenCollectables;
    public Text currentCoin_Text;
    private void Start()
    {
        currentCoins = 0;
        currentGreenCollectables = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Collectables")
        {
            currentGreenCollectables += 1;
            collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            collision.gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Collected");
            Destroy(collision.gameObject, 2f);
            
        }

        if (collision.gameObject.tag == "Coin")
        {
            currentCoins += 1;
            currentCoin_Text.text = currentCoins.ToString();
            collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            collision.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collect");
            Destroy(collision.gameObject, 1f);
            FindAnyObjectByType<SoundManager>().PlayEatCoinSound();
        }
    }
}
