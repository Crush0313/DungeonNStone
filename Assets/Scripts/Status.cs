using AFPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    //육체
    public int Strength; //근력
    public int Agility;//민첩성
    public int Flexibility;//유연성
    public int PhisicalResistance;//물리 방어력
    public int BoneStrength;//골강도
    //정신
    public int Mentality;//정신력
    public int SoulPower;//영혼력
    public int MagicResistance;//마법 방어력
    //이능
    public int Luck;//행운
    public int Restoration;//수복력
    public int Adaptablity;//적응력
    public int Penetration;//관통력
    public int Sharpness;//절삭력
    //기타
    public int Fame;//명성

    public int Lv = 1;
    public int TargetExp;
    public int CurrentExp;

    public HUD hud;
    public GameObject prefabGO;
    public GameObject R_Hand;

    private void Start()
    {
        SetTargetExp();
    }

    public void SetTargetExp()
    {
        TargetExp = (int)(0.7f * (Lv * Lv + 10 * Lv) + 20);
        hud.SetLvText(Lv.ToString());
    }

    public void ExpSum(int _exp)
    {
        CurrentExp += _exp;
        if (CurrentExp >= TargetExp)
        {
            Lv++; //레벨업
            SetTargetExp();
            CurrentExp = TargetExp - CurrentExp; //나머지 이월
        }
        hud.SetExpFill((float)CurrentExp / TargetExp);
    }

    public void SetWeapon()
    {
        if (R_Hand.transform.GetChild(0)!=null)
            Destroy(R_Hand.transform.GetChild(0).gameObject);

        GameObject prefab = prefabGO.GetComponent<ItemOnObject>().item.itemModel;
        GameObject temp = Instantiate(prefab);
        temp.transform.SetParent(R_Hand.transform, false);
    }
}
