using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryUIControl : MonoBehaviour
{
    private Inventory inv;

    public GameObject invUIPrefab;
    public GameObject invItemUIPrefab;
    protected GameObject invUI;
    private GameObject gridObj;

    private void Awake()
    {
        inv = GetComponent<Inventory>();
        inv.ItemAdded += ItemAdded;
        invUI = Instantiate(invUIPrefab, GameObject.Find("Canvas").transform);
        gridObj = invUI.transform.Find("grid").gameObject;
        Visibility(false);
    }

    public void Visibility(bool visibility)
    {
        invUI.SetActive(visibility);
    }

    private void ItemAdded(Inventory sender, Item item, int x, int y)
    {
        Image itemUI = Instantiate(invItemUIPrefab, gridObj.transform).GetComponent<Image>();
        itemUI.sprite = item.sprite;
        ((RectTransform)itemUI.transform).anchoredPosition = new Vector2(32 * 2 * x + 1, 32 * 2 * -y - 1);
        ((RectTransform)itemUI.transform).sizeDelta = new Vector2(item.width * 32, item.height * 32);
    }
}
