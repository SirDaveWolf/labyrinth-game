using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CollectableHover : MonoBehaviour
{
    public RawImage CardPreview;
    public Texture CardImage;

    private Collider myCollider;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            if (hit.collider == myCollider)
            {
                CardPreview.texture = CardImage;
            }
        }
    }
}
