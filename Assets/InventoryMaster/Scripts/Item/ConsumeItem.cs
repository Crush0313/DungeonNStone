using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using AquariusMax.UPF;

public class ConsumeItem : MonoBehaviour
{
    public static ConsumeItem instance;

    public Status status;
    private static Tooltip tooltip;
    public ItemType[] itemTypeOfSlot;
    public static EquipmentSystem eS;

    public static GameObject mainInventory;

    void Start()
    {
        instance = this;

        //메인 인벤
        if (GameObject.FindGameObjectWithTag("MainInventory") != null)
            mainInventory = GameObject.FindGameObjectWithTag("MainInventory");
        //툴팁
        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
        //장비 시스템
        if (GameObject.FindGameObjectWithTag("EquipmentSystem") != null)
            eS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
        
        //장비 시스템 슬롯들
        //장비 시스템 쪽에서 start 때 es에 넣어줌
        if (eS != null)
            itemTypeOfSlot = eS.itemTypeOfSlots;

    }



    public void ItemUse(Inventory inventory, Item item, Transform TF = null) //TF는 장비일 때만 필요
    {
        bool gearable = false;

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
                        Debug.Log("안빈슬롯");
                        inventory.EquiptItem(item); //장비에 해당 아이템 장착

                        //장비창에 아이템 자식으로 달고, 위치도 초기화
                        TF.SetParent(eS.transform.GetChild(1).GetChild(i));
                        TF.GetComponent<RectTransform>().localPosition = Vector3.zero;

                        //장비 인벤토리, 메인 인벤토리 변화 업데이트
                        eS.gameObject.GetComponent<Inventory>().updateItemList();
                        inventory.updateItemList();

                        break; //for문 탈출
                    }

                    //장비 슬롯이 비어있지 않음
                    else
                    {
                        Debug.Log("빈 슬롯");
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
                            otherItemFromCharacterSystem.transform.SetParent(TF.parent);
                            otherItemFromCharacterSystem.GetComponent<RectTransform>().localPosition = Vector3.zero;


                            TF.SetParent(eS.transform.GetChild(1).GetChild(i));
                            TF.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        }



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
                if (item.itemType == ItemType.Consumable)
                {

                    //아이템 사용하고, 개수 차감
                    inventory.ConsumeItem(item);
                    item.itemValue--;

                    //개수 0이 되면
                    if (item.itemValue <= 0)
                    {
                        //툴팁 없지 않은 이상, 툴팁 비활성화
                        if (tooltip != null)
                            tooltip.deactivateTooltip();

                        inventory.deleteItemFromInventory(item); //인벤토리에서 템 제거
                        Destroy(TF.gameObject); //템 오브젝트 제거
                    }
                }
                else if (item.itemType == ItemType.Essence)
                {
                    if (status.CurrentEss >= status.Lv)
                        return; //정수 총량 초과
                    //중복 불가


                    //아이템 사용하고, 개수 차감
                    inventory.EquiptItem(item);
                    item.itemValue--;

                    //개수 0이 되면
                    if (item.itemValue <= 0)
                    {
                        //툴팁 없지 않은 이상, 툴팁 비활성화
                        if (tooltip != null)
                            tooltip.deactivateTooltip();

                        inventory.deleteItemFromInventory(item); //인벤토리에서 템 제거
                        Destroy(TF.gameObject); //템 오브젝트 제거
                    }
                }
            }
        }
    }
}
