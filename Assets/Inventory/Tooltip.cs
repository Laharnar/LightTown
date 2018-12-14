using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool? hover = null;
    private float counter = 0.0f;
    private GameObject tooltipUI;

    public float delayBeforeShow = 0.2f;
    public GameObject tooltipPrefab;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = null;
        if (tooltipUI != null)
        {
            Destroy(tooltipUI);
        }
    }

    private void Update()
    {
        if (hover == false)
        {
            counter += Time.deltaTime;
            if (counter > delayBeforeShow)
            {
                hover = true;
                tooltipUI = Instantiate(tooltipPrefab, GameObject.Find("Canvas").transform);
                tooltipUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
            }
        }
    }
}
