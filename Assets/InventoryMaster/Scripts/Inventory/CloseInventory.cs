using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//인벤토리 닫기 버튼용. 인벤토리(부모 객체)를 닫음
public class CloseInventory : MonoBehaviour, IPointerDownHandler
{
    Inventory inv;

    void Start()
    {
        inv = transform.parent.GetComponent<Inventory>();

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inv.closeInventory();
        }
    }
}
