using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{
    private Vector2 pointerOffset;
    private RectTransform rectTransform;
    private RectTransform rectTransformSlot;
    private CanvasGroup canvasGroup;
    private GameObject oldSlot;
    private Inventory inventory;
    private Transform draggedItemBox;

    public delegate void ItemDelegate();
    public static event ItemDelegate updateInventoryList;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransformSlot = GameObject.FindGameObjectWithTag("DraggingItem").GetComponent<RectTransform>();
        inventory = transform.parent.parent.parent.GetComponent<Inventory>();
        draggedItemBox = GameObject.FindGameObjectWithTag("DraggingItem").transform;
    }


    public void OnDrag(PointerEventData data)
    {
        if (rectTransform == null)
            return;

        //좌클릭 중
        if (data.button == PointerEventData.InputButton.Left )
        {
            rectTransform.SetAsLastSibling();
            transform.SetParent(draggedItemBox);
            Vector2 localPointerPosition;
            canvasGroup.blocksRaycasts = false;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformSlot, Input.mousePosition, data.pressEventCamera, out localPointerPosition))
            {
                rectTransform.localPosition = localPointerPosition - pointerOffset;
                // dup 파괴
                if (transform.GetComponent<ConsumeItem>().duplication != null)
                    Destroy(transform.GetComponent<ConsumeItem>().duplication);
            }
        }
        //인벤 업데이트
        inventory.OnUpdateItemList();
    }



    public void OnPointerDown(PointerEventData data)
    {
        //좌클릭 시
        if (data.button == PointerEventData.InputButton.Left)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out pointerOffset);
            oldSlot = transform.parent.gameObject;
        }

        if (updateInventoryList != null)
            updateInventoryList();
    }

    //매개변수로 가져온 아이템의 복제본을 만들어 메인 인벤에 넣고, 아이템의 dup에 넣음, dup의 dup에 아이템을 넣어 서로 참조.
    public void createDuplication(GameObject Item)
    {
        Item item = Item.GetComponent<ItemOnObject>().item;

        //메인 인벤토리에 템을 넣음
        //그리고 스텍 세팅 실행
        GameObject duplication = GameObject.FindGameObjectWithTag("MainInventory").GetComponent<Inventory>().addItemToInventory(item.itemID, item.itemValue);
        duplication.transform.parent.parent.parent.GetComponent<Inventory>().stackableSettings();
        
        //item의 dup으로 설정하고, dup의 dup으로 item을 설정함
        Item.GetComponent<ConsumeItem>().duplication = duplication;
        duplication.GetComponent<ConsumeItem>().duplication = Item;
    }

    public void OnEndDrag(PointerEventData data)
    {
        //좌클릭 드래그가 끝나면
        if (data.button == PointerEventData.InputButton.Left)
        {
            canvasGroup.blocksRaycasts = true;

            //드래그가 인벤토리 창에서 끝남 (hud의 dmg fx 이미지라면 무시)
            Transform newSlot = null;
            if (data.pointerEnter != null)
                newSlot = data.pointerEnter.transform;

            if (newSlot != null)
            {
                //getting the items from the slots, GameObjects and RectTransform
                GameObject firstItemGameObject = this.gameObject;
                GameObject secondItemGameObject = newSlot.parent.gameObject;

                RectTransform firstItemRectTransform = this.gameObject.GetComponent<RectTransform>();
                RectTransform secondItemRectTransform = newSlot.parent.GetComponent<RectTransform>();

                Item firstItem = rectTransform.GetComponent<ItemOnObject>().item;
                Item secondItem = new Item();

                if (newSlot.parent.GetComponent<ItemOnObject>() != null)
                    secondItem = newSlot.parent.GetComponent<ItemOnObject>().item;

                //get some informations about the two items
                bool sameItem = firstItem.itemName == secondItem.itemName;
                bool sameItemRerferenced = firstItem.Equals(secondItem);
                bool secondItemStack = false;
                bool firstItemStack = false;
                if (sameItem)
                {
                    firstItemStack = firstItem.itemValue < firstItem.maxStack;
                    secondItemStack = secondItem.itemValue < secondItem.maxStack;
                }

                //어떤 인벤창에서 끝났는지
                GameObject Inventory = secondItemRectTransform.parent.gameObject;
                if (Inventory.tag == "Slot")
                    Inventory = secondItemRectTransform.parent.parent.parent.gameObject;

                if (Inventory.tag.Equals("Slot"))
                    Inventory = Inventory.transform.parent.parent.gameObject;


                //dragging in an Inventory      
                if (Inventory.GetComponent<Hotbar>() == null &&
                    Inventory.GetComponent<EquipmentSystem>() == null)
                {
                    //you cannot attach items to the resultslot of the craftsystem
                    if (
                        newSlot.transform.parent.parent.tag == "ResultSlot" ||
                        newSlot.transform.parent.tag == "ResultSlot" ||
                        newSlot.transform.tag == "ResultSlot" )
                    {
                        firstItemGameObject.transform.SetParent(oldSlot.transform);
                        firstItemRectTransform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        int newSlotChildCount = newSlot.transform.parent.childCount;
                        bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";
                        //dragging on a slot where allready is an item on
                        if (newSlotChildCount != 0 && isOnSlot)
                        {
                            //check if the items fits into the other item
                            bool fitsIntoStack = false;
                            if (sameItem)
                                fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack;
                            //if the item is stackable checking if the firstitemstack and seconditemstack is not full and check if they are the same items

                            if (inventory.stackable && sameItem && firstItemStack && secondItemStack)
                            {
                                //if the item does not fit into the other item
                                if (fitsIntoStack && !sameItemRerferenced)
                                {
                                    secondItem.itemValue = firstItem.itemValue + secondItem.itemValue;
                                    secondItemGameObject.transform.SetParent(newSlot.parent.parent);
                                    Destroy(firstItemGameObject);
                                    secondItemRectTransform.localPosition = Vector3.zero;

                                    //세컨템의 dup이 null이 아닌 이상
                                    if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null)
                                    {
                                        //세컨템 dup의 갯수를 세컨템의 갯수로 설정, 세컨템 dup이 있는 인벤 업뎃
                                        GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication;
                                        dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue;
                                        //dup.GetComponent<SplitItem>().inv.stackableSettings();
                                        dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList();
                                    }
                                }

                                else
                                {
                                    //creates the rest of the item
                                    int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                    //fill up the other stack and adds the rest to the other stack 
                                    if (!fitsIntoStack && rest > 0)
                                    {
                                        firstItem.itemValue = firstItem.maxStack;
                                        secondItem.itemValue = rest;

                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                        secondItemGameObject.transform.SetParent(oldSlot.transform);

                                        firstItemRectTransform.localPosition = Vector3.zero;
                                        secondItemRectTransform.localPosition = Vector3.zero;
                                    }
                                }

                            }
                            //if does not fit
                            else
                            {
                                //creates the rest of the item
                                int rest = 0;
                                if (sameItem)
                                    rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                //fill up the other stack and adds the rest to the other stack 
                                if (!fitsIntoStack && rest > 0)
                                {
                                    secondItem.itemValue = firstItem.maxStack;
                                    firstItem.itemValue = rest;

                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);

                                    firstItemRectTransform.localPosition = Vector3.zero;
                                    secondItemRectTransform.localPosition = Vector3.zero;
                                }
                                //if they are different items or the stack is full, they get swapped
                                else if (!fitsIntoStack && rest == 0)
                                {
                                    //if you are dragging an item from equipmentsystem to the inventory and try to swap it with the same itemtype
                                    if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType == secondItem.itemType)
                                    {
                                        newSlot.transform.parent.parent.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
                                        oldSlot.transform.parent.parent.GetComponent<Inventory>().EquiptItem(secondItem);

                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                        secondItemGameObject.transform.SetParent(oldSlot.transform);
                                        secondItemRectTransform.localPosition = Vector3.zero;
                                        firstItemRectTransform.localPosition = Vector3.zero;

                                        //dup이 있으면 파괴
                                        if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null)
                                            Destroy(secondItemGameObject.GetComponent<ConsumeItem>().duplication);

                                    }
                                    //if you are dragging an item from the equipmentsystem to the inventory and they are not from the same itemtype they do not get swapped.                                    
                                    else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType != secondItem.itemType)
                                    {
                                        firstItemGameObject.transform.SetParent(oldSlot.transform);
                                        firstItemRectTransform.localPosition = Vector3.zero;
                                    }
                                    //swapping for the rest of the inventorys
                                    else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null)
                                    {
                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                        secondItemGameObject.transform.SetParent(oldSlot.transform);
                                        secondItemRectTransform.localPosition = Vector3.zero;
                                        firstItemRectTransform.localPosition = Vector3.zero;
                                    }
                                }

                            }

                        }

                        //empty slot
                        else
                        {
                            if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon")
                            {
                                firstItemGameObject.transform.SetParent(oldSlot.transform);
                                firstItemRectTransform.localPosition = Vector3.zero;
                            }
                            else
                            {                                
                                firstItemGameObject.transform.SetParent(newSlot.transform);
                                firstItemRectTransform.localPosition = Vector3.zero;

                                if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
                                    oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
                            }
                        }
                    }
                }

                //dragging into a Hotbar            
                if (Inventory.GetComponent<Hotbar>() != null)
                {
                    int newSlotChildCount = newSlot.transform.parent.childCount;
                    bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";
                    //dragging on a slot where allready is an item on
                    if (newSlotChildCount != 0 && isOnSlot)
                    {
                        //check if the items fits into the other item
                        bool fitsIntoStack = false;
                        if (sameItem)
                            fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack;
                        //if the item is stackable checking if the firstitemstack and seconditemstack is not full and check if they are the same items

                        if (inventory.stackable &&
                            sameItem &&
                            firstItemStack &&
                            secondItemStack)
                        {
                            //if the item does not fit into the other item
                            if (fitsIntoStack && !sameItemRerferenced)
                            {
                                secondItem.itemValue = firstItem.itemValue + secondItem.itemValue;
                                secondItemGameObject.transform.SetParent(newSlot.parent.parent);
                                Destroy(firstItemGameObject);
                                secondItemRectTransform.localPosition = Vector3.zero;

                                //세컨템의 dup이 null이 아닌 이상
                                if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null)
                                {
                                    //세컨템 dup의 갯수를 세컨템의 갯수로 설정, 세컨템 dup이 있는 인벤 업뎃
                                    GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication;
                                    dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue;
                                    //dup.GetComponent<SplitItem>().inv.stackableSettings();
                                    dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList();
                                }
                            }

                            else
                            {
                                //creates the rest of the item
                                int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                //fill up the other stack and adds the rest to the other stack 
                                if (!fitsIntoStack && rest > 0)
                                {
                                    firstItem.itemValue = firstItem.maxStack;
                                    secondItem.itemValue = rest;

                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);

                                    firstItemRectTransform.localPosition = Vector3.zero;
                                    secondItemRectTransform.localPosition = Vector3.zero;

                                    createDuplication(this.gameObject);
                                    secondItemGameObject.GetComponent<ConsumeItem>().duplication.GetComponent<ItemOnObject>().item = secondItem;
                                    //secondItemGameObject.GetComponent<SplitItem>().inv.stackableSettings();

                                }
                            }

                        }
                        //if does not fit
                        else
                        {
                            //creates the rest of the item
                            int rest = 0;
                            if (sameItem)
                                rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                            bool fromEquip = oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null;

                            //fill up the other stack and adds the rest to the other stack 
                            if (!fitsIntoStack && rest > 0)
                            {
                                secondItem.itemValue = firstItem.maxStack;
                                firstItem.itemValue = rest;

                                createDuplication(this.gameObject);

                                firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                secondItemGameObject.transform.SetParent(oldSlot.transform);

                                firstItemRectTransform.localPosition = Vector3.zero;
                                secondItemRectTransform.localPosition = Vector3.zero;

                            }
                            //if they are different items or the stack is full, they get swapped
                            else if (!fitsIntoStack && rest == 0)
                            {
                                if (!fromEquip)
                                {
                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);
                                    secondItemRectTransform.localPosition = Vector3.zero;
                                    firstItemRectTransform.localPosition = Vector3.zero;

                                    //구 슬롯이 메인 인벤일 때
                                    if (oldSlot.transform.parent.parent.gameObject.Equals(GameObject.FindGameObjectWithTag("MainInventory")))
                                    {
                                        //세컨템 dup 파괴, 퍼스트템 dup 생성 
                                        Destroy(secondItemGameObject.GetComponent<ConsumeItem>().duplication);
                                        createDuplication(firstItemGameObject);
                                    }
                                    else
                                    {//퍼스트템 dup 생성 
                                        createDuplication(firstItemGameObject);
                                    }
                                }
                                else
                                {
                                    firstItemGameObject.transform.SetParent(oldSlot.transform);
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                }
                            }

                        }
                    }
                    //empty slot
                    else
                    {
                        if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon")
                        {
                            firstItemGameObject.transform.SetParent(oldSlot.transform);
                            firstItemRectTransform.localPosition = Vector3.zero;
                        }
                        else
                        {                            
                            firstItemGameObject.transform.SetParent(newSlot.transform);
                            firstItemRectTransform.localPosition = Vector3.zero;

                            if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
                                oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
                            //첫 템 dup 생성
                            createDuplication(firstItemGameObject);
                        }
                    }

                }

                //dragging into a equipmentsystem/charactersystem
                if (Inventory.GetComponent<EquipmentSystem>() != null)
                {
                    ItemType[] itemTypeOfSlots = GameObject.FindGameObjectWithTag("EquipmentSystem").GetComponent<EquipmentSystem>().itemTypeOfSlots;
                    int newSlotChildCount = newSlot.transform.parent.childCount;
                    bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";
                    bool sameItemType = firstItem.itemType == secondItem.itemType;
                    bool fromHot = oldSlot.transform.parent.parent.GetComponent<Hotbar>() != null;

                    //dragging on a slot where allready is an item on
                    if (newSlotChildCount != 0 && isOnSlot)
                    {
                        //items getting swapped if they are the same itemtype
                        if (sameItemType && !sameItemRerferenced) //
                        {
                            Transform temp1 = secondItemGameObject.transform.parent.parent.parent;
                            Transform temp2 = oldSlot.transform.parent.parent;                            

                            firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                            secondItemGameObject.transform.SetParent(oldSlot.transform);
                            secondItemRectTransform.localPosition = Vector3.zero;
                            firstItemRectTransform.localPosition = Vector3.zero;

                            if (!temp1.Equals(temp2))
                            {
                                Inventory.GetComponent<Inventory>().EquiptItem(firstItem);
                                if (secondItem.itemType != ItemType.Backpack)
                                    Inventory.GetComponent<Inventory>().UnEquipItem1(secondItem);
                            }

                            //핫바에서 왔으면, 세컨템의 dup 생성
                            if (fromHot)
                                createDuplication(secondItemGameObject);

                        }
                        //if they are not from the same Itemtype the dragged one getting placed back
                        else
                        {
                            firstItemGameObject.transform.SetParent(oldSlot.transform);
                            firstItemRectTransform.localPosition = Vector3.zero;

                            if (fromHot)
                                createDuplication(firstItemGameObject);
                        }

                    }
                    //if the slot is empty
                    else
                    {
                        for (int i = 0; i < newSlot.parent.childCount; i++)
                        {
                            if (newSlot.Equals(newSlot.parent.GetChild(i)))
                            {
                                //checking if it is the right slot for the item
                                if (itemTypeOfSlots[i] == transform.GetComponent<ItemOnObject>().item.itemType)
                                {
                                    transform.SetParent(newSlot);
                                    rectTransform.localPosition = Vector3.zero;

                                    if (!oldSlot.transform.parent.parent.Equals(newSlot.transform.parent.parent))
                                        Inventory.GetComponent<Inventory>().EquiptItem(firstItem);

                                }
                                //else it get back to the old slot
                                else
                                {
                                    transform.SetParent(oldSlot.transform);
                                    rectTransform.localPosition = Vector3.zero;
                                    if (fromHot)
                                        createDuplication(firstItemGameObject);
                                }
                            }
                        }
                    }

                }


            }

            //인벤토리 슬롯,창 외에 드래그 아웃 함.
            //아이템을 오브젝트 형태로 생성함
            else
            {
                //아이템 오브젝트 생성, 컴포넌트 및 아이템 정보 적용
                GameObject dropItem = (GameObject)Instantiate(GetComponent<ItemOnObject>().item.itemModel);
                dropItem.AddComponent<PickUpItem>();
                dropItem.GetComponent<PickUpItem>().item = this.gameObject.GetComponent<ItemOnObject>().item;               
                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
                
                //인벤 업데이트 
                inventory.OnUpdateItemList();

                //장비 창에 있던 아이템 이었으면 장비해제 함수 호출
                if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
                    inventory.GetComponent<Inventory>().UnEquipItem1(dropItem.GetComponent<PickUpItem>().item);
                
                //본 아이템 파괴
                Destroy(this.gameObject);
            }
        }
        //인벤 업데이트
        inventory.OnUpdateItemList();
    }

}
