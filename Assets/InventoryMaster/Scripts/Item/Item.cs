using UnityEngine;
using System.Collections.Generic;

//뼈대 클래스
[System.Serializable]
public class Item
{
    public string itemName;                                     //itemName of the item
    public int itemID;                                          //itemID of the item
    public string itemDesc;                                     //itemDesc of the item
    public Sprite itemIcon;                                     //itemIcon of the item
    public GameObject itemModel;                                //itemModel of the item
    public int itemValue = 1;                                   //itemValue is at start 1
    public ItemType itemType;                                   //itemType of the Item
    public int maxStack = 1;
    public int indexItemInList = 999;    
    public int rarity;

    //[SerializeField]
    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();  
    
    public Item(){}
    public Item(string name, int id, string desc, Sprite icon, GameObject model, int _maxStack, ItemType type, string sendmessagetext, List<ItemAttribute> _itemAttributes)                 //function to create a instance of the Item
    {
        itemName = name;
        itemID = id;
        itemDesc = desc;
        itemIcon = icon;
        itemModel = model;
        itemType = type;

        maxStack = _maxStack;
        itemAttributes = _itemAttributes;
    }

    public Item getCopy()
    {
        //value 타입은 독립적으로 객체를 복사하고, reference 타입(컴포넌트)은 동일 객체 참조
        return (Item)this.MemberwiseClone();        
    }   
}


