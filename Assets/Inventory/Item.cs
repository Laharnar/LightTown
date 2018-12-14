using UnityEngine;

public class Item : MonoBehaviour
{
    public int width; 
    public int height;
    public Sprite sprite;

    private void OnTriggerEnter(Collider other)
    {
        Inventory inv = other.GetComponent<Inventory>();
        if (inv != null)
        {
            width = Random.Range(1, 3);
            height = Random.Range(1, 3);
            inv.AddItem(this);
        }
    }
}
