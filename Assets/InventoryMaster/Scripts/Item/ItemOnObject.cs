using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemOnObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler                  //Saves the Item in the slot
{
    public Item item;                                       //Item 
    private Text text;                                      //text for the itemValue
    private Image image;

    public Tooltip tooltip;                                     //The tooltip script

    void Start()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        transform.GetChild(0).GetComponent<Image>().sprite = item.itemIcon;                 //set the sprite of the Item 
        text = transform.GetChild(1).GetComponent<Text>();                                  //get the text(itemValue GameObject) of the item

        if (GameObject.FindGameObjectWithTag("Tooltip") != null) //툴팁이 없지 않은 이상, 툴팁 레퍼런스 찾아 넣음
            tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
    }

    void Update()
    {
        text.text = "" + item.itemValue;                     //sets the itemValue         
        image.sprite = item.itemIcon;
    }

    public void OnPointerEnter(PointerEventData data)                               //if you hit a item in the slot
    {
        if (tooltip != null)
        {
            item = GetComponent<ItemOnObject>().item;                   //we get the item                            //set the item in the tooltip
            tooltip.activateTooltip(item, transform.position);                                  //set all informations of the item in the tooltip
        }
    }

    public void OnPointerExit(PointerEventData data)                //if we go out of the slot with the item
    {
        if (tooltip != null) //툴팁이 없지 않은 이상, 툴팁 비활성화
            tooltip.deactivateTooltip();            //the tooltip getting deactivated
    }

    //클릭 시 발동
    public void OnPointerDown(PointerEventData data)
    {
        //장비창에서 클릭이면 무시
        if (this.gameObject.transform.parent.parent.parent.GetComponent<EquipmentSystem>() == null)
        {
            //이렇게 가져오는 이유는, 메인/장비/핫바 모두 인벤토리를 가지고 있기 때문, 
            Inventory inventory = transform.parent.parent.parent.GetComponent<Inventory>();

            //클릭이 '우클릭'이었을 시
            if (data.button == PointerEventData.InputButton.Right)
            {
                Debug.Log(item.itemName);
                ConsumeItem.instance.ItemUse(inventory, item, transform);
            }
        }
    }
}
