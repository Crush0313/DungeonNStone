﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//실제로 저장되어 아이템 관리하는 부분
public class CreateItemDatabase
{
    public static ItemDataBaseList asset;                                                  //The List of all Items

#if UNITY_EDITOR
    public static ItemDataBaseList createItemDatabase()                                    //creates a new ItemDatabase(new instance)
    {
        asset = ScriptableObject.CreateInstance<ItemDataBaseList>();                       //of the ScriptableObject InventoryItemList

        AssetDatabase.CreateAsset(asset, "Assets/InventoryMaster/Resources/ItemDatabase.asset");            //in the Folder Assets/Resources/ItemDatabase.asset
        AssetDatabase.SaveAssets();                                                         //and than saves it there
        asset.itemList.Add(new Item());
        return asset;
    }
#endif
}
