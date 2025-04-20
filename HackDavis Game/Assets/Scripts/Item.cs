using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float rotationSpeed = 2.0f;

    private float lowerBound = -570.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, Time.deltaTime * rotationSpeed);
        if (transform.position.y <= lowerBound)
        {
            Destroy(gameObject);
        }
    }
}
