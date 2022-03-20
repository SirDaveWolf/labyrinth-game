using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public ArrowMoveDirection MoveDirection;

    private Vector3 initialPosition;
    private float direction;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;

        switch(MoveDirection)
        {
            case ArrowMoveDirection.Down:
                direction = -1.0f;
                transform.Rotate(new Vector3(0.0f, -45.0f, 0.0f));
                break;

            case ArrowMoveDirection.Up:
                direction = 1.0f;
                transform.Rotate(new Vector3(0.0f, 135.0f, 0.0f));
                break;

            case ArrowMoveDirection.Left:
                direction = -1.0f;
                transform.Rotate(new Vector3(0.0f, 45.0f, 0.0f));
                break;

            case ArrowMoveDirection.Right:
                direction = 1.0f;
                transform.Rotate(new Vector3(0.0f, -135.0f, 0.0f));
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(MoveDirection)
        {
            case ArrowMoveDirection.Down:
            case ArrowMoveDirection.Up:

                transform.position += new Vector3(0, 0, direction) * Time.deltaTime * 2.5f;

                if (transform.position.z > initialPosition.z + 2)
                {
                    direction = -1;
                }
                else if (transform.position.z < initialPosition.z - 2)
                {
                    direction = 1;
                }

                break;

            case ArrowMoveDirection.Left:
            case ArrowMoveDirection.Right:

                transform.position += new Vector3(direction, 0, 0) * Time.deltaTime * 2.5f;

                if (transform.position.x > initialPosition.x + 2)
                {
                    direction = -1;
                }
                else if (transform.position.x < initialPosition.x - 2)
                {
                    direction = 1;
                }

                break;
        }
    }
}
