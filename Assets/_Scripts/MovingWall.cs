using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public enum Direction
    {
        X,
        Y,
        Z,
        NegativeX,
        NegativeY,
        NegativeZ
    }

    [Header("Trap Settings")]
    public Direction moveDirection = Direction.X;
    public float speed = 5f;
    public float chaseDuration = 3f; 
    private bool isChasing = false;
    private float chaseEndTime;

    private Vector3 movementVector;

    void Start()
    {
        switch (moveDirection)
        {
            case Direction.X:
                movementVector = Vector3.right;
                break;
            case Direction.Y:
                movementVector = Vector3.up;
                break;
            case Direction.Z:
                movementVector = Vector3.forward;
                break;
            case Direction.NegativeX:
                movementVector = Vector3.left;
                break;
            case Direction.NegativeY:
                movementVector = Vector3.down;
                break;
            case Direction.NegativeZ:
                movementVector = Vector3.back;
                break;
        }
    }

    private void Update() 
    {
        if (isChasing)
        {
            transform.position += movementVector * speed * Time.deltaTime;
            if (Time.time > chaseEndTime)
            {
                isChasing = false;
            }
        }
    }

    public void MoveWall()
    {
        isChasing = true;
        chaseEndTime = Time.fixedTime + chaseDuration;
    }
}
