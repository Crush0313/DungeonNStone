using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//아이템 데이터베이스
//생성한 아이템들을 저장함
//'아이템' 스크립트의 리스트 형태
//데이터 베이스에서 아이템에 접근 가능하도록, 퍼블릭 함수 제공
//'id'로 해당 아이템의 정보를 가져오거나, '이름'으로 찾아서 가져오는 등

public class ItemDataBaseList : ScriptableObject
{             //The scriptableObject where the Item getting stored which you create(ItemDatabase)

    [SerializeField]
    public List<Item> itemList = new List<Item>();              //List of it

    public Item getItemByID(int id)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemID == id)
                return itemList[i].getCopy();
        }
        return null;
    }

    public Item getItemByName(string name)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemName.ToLower().Equals(name.ToLower()))
                return itemList[i].getCopy();
        }
        return null;
    }
}
