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

    List<int> Essences = new List<int>();
    public int CurrentEss = 0; //���� ���� ����
    public int Dmg = 1;

    public HUD hud;

    private void Start()
    {
        SetTargetExp();
        StatUI();
    }
    private void Update()
    {
        StatUI();
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

    public void AddEss(int _ItemID)
    {
        Essences.Add(_ItemID);
    }
    public void RemoveEss(int _ItemID)
    {
        Essences.Remove(_ItemID);
    }
    public bool ChkEss(int _ItemID)
    {
        if (Essences.Contains(_ItemID))
            return true;
        return false;
    }
    public void StatUI()
    {
        hud.UpdateStatUI("����ġ : " + CurrentExp + "/" + TargetExp + "\n"
            + "���ݷ� :" + Dmg);
    }
}
