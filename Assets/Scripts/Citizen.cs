using UnityEngine;

public class Citizen : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Vector2 boundaryMin;
    public Vector2 boundaryMax;

    private Vector2 targetPosition;

    void Start()
    {
        SetRandomTargetPosition();
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the target position has been reached
        if ((Vector2)transform.position == targetPosition)
        {
            SetRandomTargetPosition();
        }
    }

    private void SetRandomTargetPosition()
    {
        // Generate a random position within the boundaries
        float randomX = Random.Range(boundaryMin.x, boundaryMax.x);
        float randomY = Random.Range(boundaryMin.y, boundaryMax.y);
        targetPosition = new Vector2(randomX, randomY);
    }
}
