using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ItemController : MonoBehaviour
{
    public GameObject itemPrefab;
    public Sprite[] sprites;
    public float leftBound;
    public float rightBound;
    public float spawnY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnItem(int itemIndex)
    {
        GameObject item = Instantiate(itemPrefab, transform);
        item.GetComponent<Image>().sprite = sprites[itemIndex - 1];
        item.transform.position = new Vector2(Random.Range(leftBound, rightBound), spawnY);
        item.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.0f, -5.0f), 0.0f));
    }
}
