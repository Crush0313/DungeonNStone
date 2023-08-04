
//뼈대 클래스
[System.Serializable]
public class ItemAttribute
{
    //이름, 값 으로 이뤄진 속성
    //속성 리스트(오브젝터블)에서 저장/관리

    public string attributeName; //이름
    public int attributeValue; //수치

    public ItemAttribute() { }
    public ItemAttribute(string _attributeName, int _attributeValue)
    {
        attributeName = _attributeName;
        attributeValue = _attributeValue;
    }
}

