using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FloatAndRotate : MonoBehaviour
{
    private float direction;
    private float initialYPosition;
    private Vector3 initialScale;
    private bool ishovered;
    private Collider myCollider;

    // Start is called before the first frame update
    void Start()
    {
        direction = 1.0f;
        initialYPosition = transform.position.y;
        initialScale = transform.localScale;
        ishovered = false;
        myCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ishovered)
        {
            transform.localScale = initialScale * 10;
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        }
        else
        {
            transform.localScale = initialScale;
            if (transform.position.y > 5)
            {
                transform.position = new Vector3(transform.position.x, initialYPosition, transform.position.z);
            }
        }

        if (false == ishovered)
        {
            transform.Rotate(new Vector3(0, 45 * Time.deltaTime, 0));
            transform.position += new Vector3(0, direction, 0) * Time.deltaTime;

            if (transform.position.y > initialYPosition + 1)
            {
                direction = -1;
            }
            else if(transform.position.y < initialYPosition)
            {
                direction = 1;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            if (hit.collider == myCollider)
            {
                ishovered = true;
                break;
            }
            else
            {
                ishovered = false;
            }
        }
    }

    public void UnHover()
    {
        ishovered = false;
    }
}
