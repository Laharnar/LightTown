using UnityEngine;

namespace Fantasy
{
    public class Inventory : MonoBehaviour
    {
        public delegate void ItemAddedHandler(Inventory sender, ItemInfo item, int x, int y);

        public event ItemAddedHandler NewItemAdded;
        public event ItemAddedHandler AddedToStack;

        public int width;
        public int height;
        private ItemInfo[,] items;

        private void Start()
        {
            items = new ItemInfo[width, height];
        }

        public void AddItem(Item item)
        {
            // item stack of given type that is not full is present in inventory
            if (IsItemAlreadyAdded(item, out ItemInfo info, out int xi, out int yi))
            {
                int toMax = info.maxStack - info.stack;
                if (item.stack > toMax)
                {
                    info.stack = item.maxStack;
                    if (AddedToStack != null)
                    {
                        AddedToStack(this, info, xi, yi);
                    }
                    int leftover = item.stack - toMax;

                    Item temp = new Item();
                    temp.name = item.name;
                    temp.width = item.width;
                    temp.height = item.height;
                    temp.maxStack = item.maxStack;
                    temp.stack = leftover;
                    temp.sprite = item.sprite;

                    AddItem(temp);
                }
                else
                {
                    info.stack += item.stack;
                    if (AddedToStack != null)
                    {
                        AddedToStack(this, info, xi, yi);
                    }
                    return;
                }
            }
            else
            {
                for (int y = 0; y < items.GetLength(1) - item.height + 1; y++)
                {
                    for (int x = 0; x < items.GetLength(0) - item.width + 1; x++)
                    {
                        bool isEmpty = true;
                        for (int h = 0; h < item.height; h++)
                        {
                            for (int w = 0; w < item.width; w++)
                            {
                                if (items[x + w, y + h] != null)
                                {
                                    isEmpty = false;
                                    break;
                                }
                            }
                            if (!isEmpty)
                            {
                                break;
                            }
                        }

                        // found empty slot
                        if (isEmpty)
                        {
                            if (item.stack > item.maxStack)
                            {
                                // add full stack
                                ItemInfo info2 = new ItemInfo(item.name, item.width, item.height, item.maxStack, item.maxStack, item.sprite);
                                for (int h = y; h < y + item.height; h++)
                                {
                                    for (int w = x; w < x + item.width; w++)
                                    {
                                        items[w, h] = info2;
                                    }
                                }
                                if (NewItemAdded != null)
                                {
                                    NewItemAdded(this, info2, x, y);
                                }

                                // add leftover
                                Item temp = new Item();
                                temp.name = item.name;
                                temp.width = item.width;
                                temp.height = item.height;
                                temp.maxStack = item.maxStack;
                                temp.stack = item.stack - item.maxStack;
                                temp.sprite = item.sprite;

                                AddItem(temp);
                            }
                            else
                            {
                                ItemInfo info2 = new ItemInfo(item.name, item.width, item.height, item.maxStack, item.stack, item.sprite);
                                for (int h = y; h < y + item.height; h++)
                                {
                                    for (int w = x; w < x + item.width; w++)
                                    {
                                        items[w, h] = info2;
                                    }
                                }
                                if (NewItemAdded != null)
                                {
                                    NewItemAdded(this, info2, x, y);
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }

        private bool IsItemAlreadyAdded(Item item, out ItemInfo itemInfo, out int x, out int y)
        {
            for (int yi = 0; yi < items.GetLength(1); yi++)
            {
                for (int xi = 0; xi < items.GetLength(0); xi++)
                {
                    if (items[xi, yi] != null && (items[xi, yi].name == item.name && items[xi, yi].stack < items[xi, yi].maxStack))
                    {
                        itemInfo = items[xi, yi];
                        x = xi;
                        y = yi;
                        return true;
                    }
                }
            }
            itemInfo = null;
            x = -1;
            y = -1;
            return false;
        }
    }

    public class ItemInfo
    {
        public string name;
        public int width;
        public int height;
        public int maxStack;
        public int stack;
        public Sprite sprite;

        public ItemInfo(string name, int width, int height, int maxStack, int stack, Sprite sprite)
        {
            this.name = name;
            this.width = width;
            this.height = height;
            this.maxStack = maxStack;
            this.stack = stack;
            this.sprite = sprite;
        }
    }
}