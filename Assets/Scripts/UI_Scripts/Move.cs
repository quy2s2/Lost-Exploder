using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    
    
    [SerializeField] private Transform[] movePoints;

    private int pointIndex = 0;
    void Update()
    {
        if (Vector2.Distance(transform.position, movePoints[pointIndex].position) < .1f)
        {
            pointIndex += 1;
        }

        if (pointIndex == movePoints.Length)
        {
            pointIndex = 0;
        }

        transform.position = Vector2.MoveTowards(transform.position, movePoints[pointIndex].position, moveSpeed * Time.deltaTime);
    }
}
