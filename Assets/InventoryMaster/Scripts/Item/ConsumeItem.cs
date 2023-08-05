using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ConsumeItem : MonoBehaviour, IPointerDownHandler
{
    public Item item;
    private static Tooltip tooltip;
    public ItemType[] itemTypeOfSlot;
    public static EquipmentSystem eS;

    public GameObject duplication; //어디에 쓰는 물건인고...드래그,인벤,핫바에 쓰임
    public static GameObject mainInventory;

    void Start()
    {
        item = GetComponent<ItemOnObject>().item;

        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();

        if (GameObject.FindGameObjectWithTag("EquipmentSystem") != null)
            eS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
        //장비 시스템이 없지않은 이상
        //장비 시스템 쪽에서 start 때 es에 넣어줌
        if (eS != null)
            itemTypeOfSlot = eS.itemTypeOfSlots;

        if (GameObject.FindGameObjectWithTag("MainInventory") != null)
            mainInventory = GameObject.FindGameObjectWithTag("MainInventory");
    }

    //클릭 시 발동
    public void OnPointerDown(PointerEventData data)
    {
        //장비창에서 클릭이면 무시
        if (this.gameObject.transform.parent.parent.parent.GetComponent<EquipmentSystem>() == null)
        {
            bool gearable = false;

            //이렇게 가져오는 이유는, 메인/장비/핫바 모두 인벤토리를 가지고 있기 때문, 
            Inventory inventory = transform.parent.parent.parent.GetComponent<Inventory>();

            //클릭이 '우클릭'이었을 시
            if (data.button == PointerEventData.InputButton.Right)
            {
                //장비 시스템이 없지않은 이상
                //장비 시스템 쪽에서 start 때 es에 넣어줌
                if (eS != null)
                {
                    for (int i = 0; i < eS.slotsInTotal; i++)
                    {
                        //장비창 슬롯에서 같은 유형의 슬롯을 찾음, = 장비 아이템임 (소비형 아이템이면 작동x)
                        if (itemTypeOfSlot[i].Equals(item.itemType))
                        {
                            //일단 장비템임
                            gearable = true;

                            //해당 장비 슬롯이 비어있음.
                            if (eS.transform.GetChild(1).GetChild(i).childCount == 0)
                            {
                                inventory.EquiptItem(item); //장비에 해당 아이템 장착

                                //장비창에 아이템 자식으로 달고, 위치도 초기화
                                transform.SetParent(eS.transform.GetChild(1).GetChild(i));
                                transform.GetComponent<RectTransform>().localPosition = Vector3.zero;

                                //장비 인벤토리, 메인 인벤토리 변화 업데이트
                                eS.gameObject.GetComponent<Inventory>().updateItemList();
                                inventory.updateItemList();


                                if (duplication != null) //dup이 있으면 파괴
                                    Destroy(duplication.gameObject);
                                break; //for문 탈출
                            }

                            //장비 슬롯이 비어있지 않음
                            else
                            {
                                //기존 장비
                                GameObject otherItemFromCharacterSystem = eS.transform.GetChild(1).GetChild(i).GetChild(0).gameObject;
                                Item otherSlotItem = otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item;

                                //새 장비 착용
                                inventory.EquiptItem(item);

                                //백팩류 아이템이 아닌 이상
                                //기존 장비 탈착
                                if (item.itemType != ItemType.Backpack)
                                    inventory.UnEquipItem1(otherSlotItem);
                                //inventory.UnEquipItem1(otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item);

                                //null이 왜 됨?
                                if (this == null)
                                {
                                    Debug.Log("여기 null 떴어요");
                                    //해당 아이템을 떨어트림

                                    GameObject dropItem = (GameObject)Instantiate(otherSlotItem.itemModel);
                                    dropItem.AddComponent<PickUpItem>();
                                    dropItem.GetComponent<PickUpItem>().item = otherSlotItem;

                                    dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;

                                    //인벤 변화 업데이트
                                    inventory.OnUpdateItemList();
                                }
                                else
                                {
                                    otherItemFromCharacterSystem.transform.SetParent(transform.parent);
                                    otherItemFromCharacterSystem.GetComponent<RectTransform>().localPosition = Vector3.zero;

                                    //핫바라면 복제
                                    if (transform.parent.parent.parent.GetComponent<Hotbar>() != null)
                                        createDuplication(otherItemFromCharacterSystem);

                                    transform.SetParent(eS.transform.GetChild(1).GetChild(i));
                                    transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
                                }


                                //duplication에 뭔가 있으면 그 오브젝트 파괴
                                if (duplication != null)
                                    Destroy(duplication.gameObject);

                                //장비 인벤토리, 쓰는 인벤토리(메인이 아닌 핫바나 창고일 수 있음) 변화 업데이트
                                eS.gameObject.GetComponent<Inventory>().updateItemList();
                                inventory.OnUpdateItemList();
                                break;
                            }
                        }
                    }

                    //장비 아이템이 아닐 시
                    if (!gearable)
                    {

                        //dup에 뭔가 있으면
                        //dup의 아이템 정보를 item from dup로 부름
                        Item itemFromDup = null; //= dup의 아이템 컴포넌트
                        if (duplication != null)
                            itemFromDup = duplication.GetComponent<ItemOnObject>().item;

                        //아이템 사용하고, 개수 차감
                        inventory.ConsumeItem(item);
                        item.itemValue--;

                        //dup에 뭔가 있어서, item from dup이 있으면
                        if (itemFromDup != null)
                        {
                            //dup의 아이템 개수도 차감하고
                            itemFromDup.itemValue--;

                            //dup의 아이템 개수가 0이 되거나 더 작으면
                            if (itemFromDup.itemValue <= 0)
                            {
                                //툴팁이 없지 않은 이상, 툴팁 비활성화
                                if (tooltip != null)
                                    tooltip.deactivateTooltip();

                                //인벤에서 템 없애고, dup 오브젝트 삭제
                                inventory.deleteItemFromInventory(item);
                                Destroy(duplication.gameObject);
                            }
                        }

                        if (item.itemValue <= 0)
                        {
                            //툴팁 없지 않은 이상, 툴팁 비활성화
                            if (tooltip != null)
                                tooltip.deactivateTooltip();

                            inventory.deleteItemFromInventory(item); //인벤토리에서 템 제거
                            Destroy(this.gameObject); //템 오브젝트 제거
                        }
                    }
                }
            }
        }
    }

    //핫바에서만 씀
    public void consumeIt()
    {
        Inventory inventory = transform.parent.parent.parent.GetComponent<Inventory>();

        //장비인가
        bool gearable = false;

        /*
        if (GameObject.FindGameObjectWithTag("EquipmentSystem") != null)
            eS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
        

        //장비 시스템이 null이 아닌 이상... 근데 어차피 참조 형태 아닌가. 한번 하면 됐지
        if (eS != null)
            itemTypeOfSlot = eS.itemTypeOfSlots;
        */

        Item itemFromDup = null;
        //dup이 있으면, item from dup로 아이템 호출어 축약
        if (duplication != null)
            itemFromDup = duplication.GetComponent<ItemOnObject>().item;       


        //장비 시스템이 없지않은 이상
        //장비 시스템 쪽에서 start 때 es에 넣어줌
        if (eS != null)
        {            
            for (int i = 0; i < eS.slotsInTotal; i++)
            {
                if (itemTypeOfSlot[i].Equals(item.itemType)) //장비 슬롯에서 같은 타입을 찾음 (=장비템임)
                {
                    //해당 장비 슬롯이 비어 있음
                    if (eS.transform.GetChild(1).GetChild(i).childCount == 0)
                    {
                        gearable = true;
                        this.gameObject.transform.SetParent(eS.transform.GetChild(1).GetChild(i));
                        this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        eS.gameObject.GetComponent<Inventory>().updateItemList();
                        inventory.updateItemList();
                        inventory.EquiptItem(item);

                        //
                        if (duplication != null)
                            Destroy(duplication.gameObject);

                        break;
                    }
                    //이미 임자있는 장비 슬롯
                    else
                    {
                        GameObject otherItemFromCharacterSystem = eS.transform.GetChild(1).GetChild(i).GetChild(0).gameObject;
                        Item otherSlotItem = otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item;

                        inventory.EquiptItem(item);
                        if (item.itemType != ItemType.Backpack)
                            inventory.UnEquipItem1(otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item);

                        //this == null이 뭔 말이야
                        if (this == null)
                        {
                            Debug.Log("여기 null 떴어요2");
                            GameObject dropItem = (GameObject)Instantiate(otherSlotItem.itemModel);
                            dropItem.AddComponent<PickUpItem>();
                            dropItem.GetComponent<PickUpItem>().item = otherSlotItem;
                            dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
                            
                            inventory.OnUpdateItemList();
                        }
                        else
                        {
                            otherItemFromCharacterSystem.transform.SetParent(this.transform.parent);
                            otherItemFromCharacterSystem.GetComponent<RectTransform>().localPosition = Vector3.zero;
                            if (this.gameObject.transform.parent.parent.parent.GetComponent<Hotbar>() != null)
                                createDuplication(otherItemFromCharacterSystem);

                            this.gameObject.transform.SetParent(eS.transform.GetChild(1).GetChild(i));
                            this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        }

                        if (duplication != null)
                            Destroy(duplication.gameObject);

                        eS.gameObject.GetComponent<Inventory>().updateItemList();
                        inventory.OnUpdateItemList();

                        break;
                    }
                }
            }
        }
        //장비템이 아님
        if (!gearable)
        {
            if (duplication != null)
                itemFromDup = duplication.GetComponent<ItemOnObject>().item;

            inventory.ConsumeItem(item);

            item.itemValue--;

            if (itemFromDup != null)
            {
                duplication.GetComponent<ItemOnObject>().item.itemValue--;
                if (itemFromDup.itemValue <= 0)
                {
                    if (tooltip != null)
                        tooltip.deactivateTooltip();
                    inventory.deleteItemFromInventory(item);
                    Destroy(duplication.gameObject);
                }
            }

            if (item.itemValue <= 0)
            {
                if (tooltip != null)
                    tooltip.deactivateTooltip();
                inventory.deleteItemFromInventory(item);
                Destroy(this.gameObject); 
            }
        }        
    }

    public void createDuplication(GameObject _Item)
    {
        Item item = _Item.GetComponent<ItemOnObject>().item; //아이템 정보를 가져와서
        GameObject dup = mainInventory.GetComponent<Inventory>().addItemToInventory(item.itemID, item.itemValue); //dup에 해당 정보를 복사함
        
        _Item.GetComponent<ConsumeItem>().duplication = dup; //아이템의 dup에 dup을 넣고
        dup.GetComponent<ConsumeItem>().duplication = _Item; //그 dup의 dup에는 item을 넣음
    }
}
