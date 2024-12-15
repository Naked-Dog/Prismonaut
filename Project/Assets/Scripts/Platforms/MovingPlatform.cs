using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IPlatform
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public CustomPath path;

    private Vector3 nextPosition;
    private int currentPointIndex = 0;
    private int direction = 1;


    private PlatformType _platformType = PlatformType.MovingPlatform;

    public PlatformType PlatformType { get => _platformType; set => _platformType = value; }

    // Start is called before the first frame update
    void Start()
    {
        SetStartPosition();
    }

    private void SetStartPosition()
    {
        transform.position = path.Points[currentPointIndex].position;
        nextPosition = path.Points[currentPointIndex + 1].position;
    }

    // Update is called once per frame
    void Update()
    {
        /* transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);
        if (transform.position == nextPosition)
        {
            if (path.isLoop && path.Points.Count != 2)
            {
                currentPointIndex = (currentPointIndex + 1 == path.Points.Count) ? 0 : currentPointIndex += 1;
                nextPosition = path.Points[currentPointIndex].position;
            }
            else
            {
                if (currentPointIndex + 1 == path.Points.Count) direction *= -1;
                else if (currentPointIndex <= 0) direction = 1;
                currentPointIndex += 1 * direction;
                nextPosition = path.Points[currentPointIndex].position;
            }
        } */
    }

    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);
        if (transform.position == nextPosition)
        {
            if (path.isLoop && path.Points.Count != 2)
            {
                currentPointIndex = (currentPointIndex + 1 == path.Points.Count) ? 0 : currentPointIndex += 1;
                nextPosition = path.Points[currentPointIndex].position;
            }
            else
            {
                if (currentPointIndex + 1 == path.Points.Count) direction *= -1;
                else if (currentPointIndex <= 0) direction = 1;
                currentPointIndex += 1 * direction;
                nextPosition = path.Points[currentPointIndex].position;
            }
        }
    }

    public void PlatformAction(Player2DController player2DController)
    {
        Debug.Log("In moving platform action");
    }
}
