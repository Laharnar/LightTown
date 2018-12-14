using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Inventory : MonoBehaviour
{
    public delegate void ItemAddedHandler(Inventory sender, Item item, int x, int y);

    public event ItemAddedHandler ItemAdded;

    public int width;
    public int height;
    public Item[,] items;

    private void Start()
    {
        items = new Item[width, height];
    }

    public void AddItem(Item item)
    {
        for (int x = 0; x < items.GetLength(0); x++)
        {
            for (int y = 0; y < items.GetLength(1); y++)
            {
                bool isEmpty = true;
                for (int h = 0; h < item.height; h++)
                {
                    for (int w = 0; w < item.width; w++)
                    {
                        if (items[x + w, y + h] != null)
                        {
                            isEmpty = false;
                        }
                    }
                    if (!isEmpty)
                    {
                        break;
                    }
                }

                if (isEmpty)
                {
                    for (int h = 0; h < item.height; h++)
                    {
                        for (int w = 0; w < item.width; w++)
                        {
                            int col = x + h;
                            int row = y + w;
                            items[col, row] = item;
                            if (ItemAdded != null)
                            {
                                ItemAdded(this, item, col, row);
                            }
                        }
                    }
                }
            }
        }
    }
}