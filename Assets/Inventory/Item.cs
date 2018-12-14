using UnityEngine;

public class Item : MonoBehaviour
{
    new public string name;
    public int width; 
    public int height;
    public int maxStack;
    public int stack;
    public Sprite sprite;

    private void Awake()
    {
        base.name = name;
    }

    private void OnTriggerEnter(Collider other)
    {
        Inventory inv = other.GetComponent<Inventory>();
        if (inv != null)
        {
            inv.AddItem(this);
        }
    }
}
