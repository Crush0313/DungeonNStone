using AFPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    //��ü
    public int Strength; //�ٷ�
    public int Agility;//��ø��
    public int Flexibility;//������
    public int PhisicalResistance;//���� ����
    public int BoneStrength;//�񰭵�
    //����
    public int Mentality;//���ŷ�
    public int SoulPower;//��ȥ��
    public int MagicResistance;//���� ����
    //�̴�
    public int Luck;//���
    public int Restoration;//������
    public int Adaptablity;//������
    public int Penetration;//�����
    public int Sharpness;//�����
    //��Ÿ
    public int Fame;//��

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
            Lv++; //������
            SetTargetExp();
            CurrentExp = TargetExp - CurrentExp; //������ �̿�
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
