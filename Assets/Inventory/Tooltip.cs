using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Fantasy
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool? hover = null;
        private float counter = 0.0f;
        private GameObject tooltipUI;

        public float delayBeforeShow = 0.2f;
        public GameObject tooltipPrefab;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("enter");
            hover = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hover = null;
            if (tooltipUI != null)
            {
                Debug.Log("exit");
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
                    tooltipUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name.Split(' ')[2];
                    ((RectTransform)tooltipUI.transform).position = Input.mousePosition;
                }
            }
            else if (hover == true)
            {
                ((RectTransform)tooltipUI.transform).position = Input.mousePosition;
            }
        }
    }
}
