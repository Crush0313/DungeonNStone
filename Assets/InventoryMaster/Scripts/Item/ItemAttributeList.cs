using UnityEngine;
using System.Collections.Generic;

//속성을 저장/관리 찰 수 있도록 만든 스크립터블 오브젝트
//'속성' 스크립트의 리스트 형태
//체력, 마나, 방어력 등등


public class ItemAttributeList : ScriptableObject
{
    public List<ItemAttribute> itemAttributeList = new List<ItemAttribute>();
}
