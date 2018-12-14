using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        inv.NewItemAdded += ItemAdded;
        inv.AddedToStack += SetItemCount;
        invUI = Instantiate(invUIPrefab, GameObject.Find("Canvas").transform);
        gridObj = invUI.transform.Find("grid").gameObject;
        Visibility(false);
    }
    
    public void Visibility(bool visibility)
    {
        invUI.SetActive(visibility);
    }

    private void ItemAdded(Inventory sender, ItemInfo item, int x, int y)
    {
        Debug.Log("create");
        Image itemUI = Instantiate(invItemUIPrefab, gridObj.transform).GetComponent<Image>();
        itemUI.sprite = item.sprite;
        itemUI.name = x + "" + y;
        ((RectTransform)itemUI.transform).anchoredPosition = new Vector2(32 * x + x + 1, 32 * -y -y - 1);
        ((RectTransform)itemUI.transform).sizeDelta = new Vector2(item.width * 32, item.height * 32);
        TextMeshProUGUI textMesh = itemUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMesh.text = item.stack.ToString();
        textMesh.rectTransform.sizeDelta = ((RectTransform)textMesh.rectTransform.parent).sizeDelta;
    }
    private void SetItemCount(Inventory sender, ItemInfo item, int x, int y)
    {
        Debug.Log(item.stack);

        gridObj.transform.Find(x + "" + y).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.stack.ToString();
    }
}
