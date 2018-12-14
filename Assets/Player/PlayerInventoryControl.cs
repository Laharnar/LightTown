using UnityEngine;

public class PlayerInventoryControl : InventoryUIControl
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Visibility(!invUI.gameObject.activeSelf);
        }
	}
}
