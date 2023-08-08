using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Hotbar : MonoBehaviour
{

    [SerializeField]
    public KeyCode[] keyCodesForSlots = new KeyCode[999];
    [SerializeField]
    public int slotsInTotal;

    public RectTransform arrowTF;
    public int addArrowPos;

    public GameObject prefabGO;
    public GameObject R_Hand;

    public Inventory inv;
    Item currentItem;

    public int currentSlotNum;

#if UNITY_EDITOR
    [MenuItem("Master System/Create/Hotbar")]        //creating the menu item
    public static void menuItemCreateInventory()       //create the inventory at start
    {
        GameObject Canvas = null;
        if (GameObject.FindGameObjectWithTag("Canvas") == null)
        {
            GameObject inventory = new GameObject();
            inventory.name = "Inventories";
            Canvas = (GameObject)Instantiate(Resources.Load("Prefabs/Canvas - Inventory") as GameObject);
            Canvas.transform.SetParent(inventory.transform, true);
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - Hotbar") as GameObject);
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            panel.transform.SetParent(Canvas.transform, true);
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject);
            Instantiate(Resources.Load("Prefabs/EventSystem") as GameObject);
            draggingItem.transform.SetParent(Canvas.transform, true);
            Inventory inv = panel.AddComponent<Inventory>();
            panel.AddComponent<InventoryDesign>();
            panel.AddComponent<Hotbar>();
            inv.getPrefabs();
        }
        else
        {
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - Hotbar") as GameObject);
            panel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true);
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Inventory inv = panel.AddComponent<Inventory>();
            panel.AddComponent<Hotbar>();
            DestroyImmediate(GameObject.FindGameObjectWithTag("DraggingItem"));
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject);
            draggingItem.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true);
            panel.AddComponent<InventoryDesign>();
            inv.getPrefabs();
        }
    }
#endif

    private void Start()
    {
        inv = GetComponent<Inventory>();
        SlotChanged(1);
    }
    void Update()
    {
        GetSlotButton();
        GetWheelInput();
    }

    public void GetSlotButton()
    {
        for (int i = 0; i < slotsInTotal; i++)
        {
            if (Input.GetKeyDown(keyCodesForSlots[i])) //슬롯 키를 누름
            {
                SlotChanged(i);
            }
        }
    }
    public void GetWheelInput()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        int targetSlotNum = currentSlotNum;

        // 휠을 밀어 돌렸을 때의 처리 ↑
        if (wheelInput >= 0.1f)
        {
            targetSlotNum++;
            if (targetSlotNum > slotsInTotal)
                targetSlotNum = 0;

            SlotChanged(targetSlotNum);
        }

        // 휠을 당겨 올렸을 때의 처리 ↓
        else if (wheelInput <= -0.1f)
        {
            targetSlotNum--;
            if (targetSlotNum < slotsInTotal)
                targetSlotNum = 0;

            SlotChanged(targetSlotNum);
        }
    }

    void SlotChanged(int TargetSlotNum)
    {
        if(currentItem!=null)   
            inv.UnEquipItem1(currentItem);

        currentSlotNum = TargetSlotNum;

        SetArrowPos();
        SetWeapon();
        UseItem();

        SetCurrnetItem();

        if (currentItem != null)
            inv.EquiptItem(currentItem);
    }

    public void SetWeapon( )
    {
        //기존 무기 오브젝트 파괴
        if (R_Hand.transform.childCount != 0)
            Destroy(R_Hand.transform.GetChild(0).gameObject);
        Debug.Log("11");

        //새 무기 오브젝트 생성
        if (transform.GetChild(1).GetChild(currentSlotNum).childCount != 0)
        {
            prefabGO = transform.GetChild(1).GetChild(currentSlotNum).GetChild(0).GetComponent<ItemOnObject>().item.itemModel;
            GameObject temp = Instantiate(prefabGO);
            temp.transform.SetParent(R_Hand.transform, false);
        }
        
    }

    public void SetArrowPos( )
    {
        Vector3 targetPos = transform.GetChild(1).GetChild(currentSlotNum).transform.position;
        arrowTF.position = targetPos + new Vector3(0, addArrowPos, 0);
    }

    //소비형이면 사용
    public void UseItem( )
    {
        if (transform.GetChild(1).GetChild(currentSlotNum).childCount != 0) //개수가 0이 아닌 이상
        {
            ConsumeItem _consumeItem = transform.GetChild(1).GetChild(currentSlotNum).GetChild(0).GetComponent<ConsumeItem>();
            _consumeItem.consumeIt();
        }
    }

    public int getSlotsInTotal()
    {
        Inventory inv = GetComponent<Inventory>();
        return slotsInTotal = inv.width * inv.height;
    }

    public void SetCurrnetItem()
    {
        Debug.Log("" + currentSlotNum);
        if (transform.GetChild(1).GetChild(currentSlotNum).childCount != 0 )
        {
            Debug.Log("아이템 있음");
            currentItem = transform.GetChild(1).GetChild(currentSlotNum).GetChild(0).GetComponent<ItemOnObject>().item;
        }
        else
        {
            currentItem = null;
            Debug.Log("빈 아이템 ");
        }
    }
}