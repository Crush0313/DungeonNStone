using UnityEngine;
using AFPC;

//인벤토리가 아니라 플레이어에 부착됨
public class PlayerInventory : MonoBehaviour
{
    //메인 인벤토리
    public GameObject inventory;
    public Inventory mainInventory;

    //장비 인벤토리
    public GameObject characterSystem;
    private Inventory characterSystemInventory;

    private Tooltip toolTip;

    private InputManager inputManagerDatabase;

    public GameObject HPMANACanvas;
    Lifecycle lifecycle;
    Status status;

    //float lifecycle.MaxHp = 100;
    //float lifecycle.MaxHp = 100;
    //float Damgage = 0;
    float maxArmor = 0;

    //public float lifecycle.currentHp = 60;
    //float lifecycle.currentHp = 100;
    //float currentDamgage = 0;
    float currentArmor = 0;

    int normalSize = 3;

    void Start()
    {
        status = GetComponent<Status>();
        lifecycle = GetComponent<Player>().lifecycle;
        lifecycle.hud.updateValue();

        if (inputManagerDatabase == null)
            inputManagerDatabase = (InputManager)Resources.Load("InputManager");

        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            toolTip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
        if (inventory != null)
            mainInventory = inventory.GetComponent<Inventory>();
        if (characterSystem != null)
            characterSystemInventory = characterSystem.GetComponent<Inventory>();
    }

    void Update()
    {
        if (Input.GetKeyDown(inputManagerDatabase.CharacterSystemKeyCode))
        {
            if (!characterSystem.activeSelf)
            {
                characterSystemInventory.openInventory();
            }
            else
            {
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                characterSystemInventory.closeInventory();
            }
        }

        if (Input.GetKeyDown(inputManagerDatabase.InventoryKeyCode))
        {
            if (!inventory.activeSelf)
            {
                mainInventory.openInventory();
            }
            else
            {
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                mainInventory.closeInventory();
            }
        }
    }
    public void OnEnable()
    {
        //배낭
        Inventory.ItemEquip += OnBackpack;
        Inventory.UnEquipItem += UnEquipBackpack;
        //장비
        Inventory.ItemEquip += OnGearItem;
        Inventory.UnEquipItem += OnUnEquipItem;
        //소비형
        Inventory.ItemConsumed += OnConsumeItem;
        //무기
        Inventory.ItemEquip += EquipWeapon;
        Inventory.UnEquipItem += UnEquipWeapon;
    }

    //플레이어에 부착되니 아마 비활성될 일은 없을 듯
    public void OnDisable()
    {
        Inventory.ItemEquip -= OnBackpack;
        Inventory.UnEquipItem -= UnEquipBackpack;

        Inventory.ItemEquip -= OnGearItem;
        Inventory.UnEquipItem -= OnUnEquipItem;

        Inventory.ItemConsumed -= OnConsumeItem;

        Inventory.ItemEquip -= EquipWeapon;
        Inventory.UnEquipItem -= UnEquipWeapon;
    }

    void EquipWeapon(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            //add the weapon if you unequip the weapon
        }
    }

    void UnEquipWeapon(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            //delete the weapon if you unequip the weapon
        }
    }

    void OnBackpack(Item item)
    {
        if (item.itemType == ItemType.Backpack)
        {
            for (int i = 0; i < item.itemAttributes.Count; i++)
            {
                if (mainInventory == null)
                    mainInventory = inventory.GetComponent<Inventory>();
                mainInventory.sortItems();
                if (item.itemAttributes[i].attributeName == "Slots")
                    changeInventorySize(item.itemAttributes[i].attributeValue);
            }
        }
    }

    void UnEquipBackpack(Item item)
    {
        if (item.itemType == ItemType.Backpack)
            changeInventorySize(normalSize);
    }

    void changeInventorySize(int size)
    {
        dropTheRestItems(size);

        if (mainInventory == null)
            mainInventory = inventory.GetComponent<Inventory>();
        if (size == 3)
        {
            mainInventory.width = 3;
            mainInventory.height = 1;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
        if (size == 6)
        {
            mainInventory.width = 3;
            mainInventory.height = 2;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
        else if (size == 12)
        {
            mainInventory.width = 4;
            mainInventory.height = 3;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
        else if (size == 16)
        {
            mainInventory.width = 4;
            mainInventory.height = 4;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
        else if (size == 24)
        {
            mainInventory.width = 6;
            mainInventory.height = 4;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
    }

    //인벤이 작아지면 나머지 아이템들을 오브젝트로 뱉음
    void dropTheRestItems(int size)
    {
        if (size < mainInventory.ItemsInInventory.Count)
        {
            for (int i = size; i < mainInventory.ItemsInInventory.Count; i++)
            {
                GameObject dropItem = (GameObject)Instantiate(mainInventory.ItemsInInventory[i].itemModel);
                dropItem.AddComponent<PickUpItem>();
                dropItem.GetComponent<PickUpItem>().item = mainInventory.ItemsInInventory[i];
                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
            }
        }
    }



    public void OnConsumeItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            if (item.itemAttributes[i].attributeName == "Health")
            {
                if ((lifecycle.currentHp + item.itemAttributes[i].attributeValue) > lifecycle.MaxHp)
                    lifecycle.currentHp = lifecycle.MaxHp;
                else
                    lifecycle.currentHp += item.itemAttributes[i].attributeValue;
            }
            if (item.itemAttributes[i].attributeName == "Mana")
            {
                if ((lifecycle.currentHp + item.itemAttributes[i].attributeValue) > lifecycle.MaxHp)
                    lifecycle.currentHp = lifecycle.MaxHp;
                else
                    lifecycle.currentHp += item.itemAttributes[i].attributeValue;
            }
            if (item.itemAttributes[i].attributeName == "Armor")
            {
                if ((currentArmor + item.itemAttributes[i].attributeValue) > maxArmor)
                    currentArmor = maxArmor;
                else
                    currentArmor += item.itemAttributes[i].attributeValue;
            }
            /*
            if (item.itemAttributes[i].attributeName == "Damgage")
            {
                if ((currentDamgage + item.itemAttributes[i].attributeValue) > Damgage)
                    currentDamgage = Damgage;
                else
                    currentDamgage += item.itemAttributes[i].attributeValue;
            }*/
        }
        lifecycle.hud.updateValue();
    }

    //장비 착용
    public void OnGearItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            if (item.itemAttributes[i].attributeName == "Health")
                lifecycle.MaxHp += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Mana")
                lifecycle.MaxHp += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Armor")
                maxArmor += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Damgage")
                status.Dmg += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Strength")
                status.Strength += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Agility")
                status.Agility += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Flexibility")
                status.Flexibility += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "PhisicalResistance")
                status.PhisicalResistance += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "BoneStrength")
                status.BoneStrength += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Mentality")
                status.Mentality += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "SoulPower")
                status.SoulPower += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "MagicResistance")
                status.MagicResistance += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Luck")
                status.Luck += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Restoration")
                status.Restoration += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Adaptablity")
                status.Adaptablity += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Penetration")
                status.Penetration += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Sharpness")
                status.Sharpness += item.itemAttributes[i].attributeValue;
        }
        lifecycle.hud.updateValue();
    }

    //장비해제
    public void OnUnEquipItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            if (item.itemAttributes[i].attributeName == "Health")
                lifecycle.MaxHp -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Mana")
                lifecycle.MaxHp -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Armor")
                maxArmor -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Damgage")
                status.Dmg -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Strength")
                status.Strength -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Agility")
                status.Agility -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Flexibility")
                status.Flexibility -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "PhisicalResistance")
                status.PhisicalResistance -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "BoneStrength")
                status.BoneStrength -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Mentality")
                status.Mentality -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "SoulPower")
                status.SoulPower -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "MagicResistance")
                status.MagicResistance -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Luck")
                status.Luck -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Restoration")
                status.Restoration -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Adaptablity")
                status.Adaptablity -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Penetration")
                status.Penetration -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Sharpness")
                status.Sharpness -= item.itemAttributes[i].attributeValue;
        }
        lifecycle.hud.updateValue();
    }




}
