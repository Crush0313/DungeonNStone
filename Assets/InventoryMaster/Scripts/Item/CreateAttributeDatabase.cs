using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//실제로 저장되어 속성을 관리하는 부분
public class CreateAttributeDatabase : MonoBehaviour
{
    public static ItemAttributeList asset;                                                  //The List of all Items

#if UNITY_EDITOR
    public static ItemAttributeList createItemAttributeDatabase()                                    //creates a new ItemDatabase(new instance)
    {
        asset = ScriptableObject.CreateInstance<ItemAttributeList>();                       //of the ScriptableObject InventoryItemList

        AssetDatabase.CreateAsset(asset, "Assets/InventoryMaster/Resources/AttributeDatabase.asset");            //in the Folder Assets/Resources/ItemDatabase.asset
        AssetDatabase.SaveAssets();                                                         //and than saves it there        
        return asset;
    }
#endif
}
