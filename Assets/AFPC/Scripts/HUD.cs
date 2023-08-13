using AFPC;
using UnityEngine;
using UnityEngine.UI;

// 체력, 방어력, 기력 UI조절
// 피격 이미지 컨트롤

public class HUD : MonoBehaviour {

    [Header("References")]
    public Player player; //플레이어
    public Slider slider_Mp; //방어력
    public Slider slider_Hp; //체력
    //public Slider slider_Endurance; //기력
    public Text Hp_Text;
    public Text Mp_Text;


    public Image LvImg; //경험치 이미지
    public Text LvTxt; //레벨 텍스트

    public Text StatTxt;

    public CanvasGroup canvasGroup_DamageFX; //피격 이미지(캔버스 그룹)

    //최대치 적용
    void Awake () {
        updateMaxValue();
        //slider_Endurance.maxValue = player.movement.referenceEndurance;
        //UpdateStatUI();
    }

    public void updateValue()
    {
        updateMaxValue();
        updateCurrentValue();
    }
    void updateMaxValue()
    {
        slider_Mp.maxValue = player.lifecycle.MaxMp;
        slider_Hp.maxValue = player.lifecycle.MaxHp;

    }
    void updateCurrentValue()
    {
        slider_Mp.value = player.lifecycle.GetMpValue();
        slider_Hp.value = player.lifecycle.GetHpValue();

        Hp_Text.text = (slider_Hp.value + "/" + slider_Hp.maxValue);
        Mp_Text.text = (slider_Mp.value + "/" + slider_Mp.maxValue);
    }

    //현재치 적용
    //피격 이미지 불투명도 낮추기
    void Update () {
        //updateCurrentValue();
        //slider_Endurance.value = player.movement.GetEnduranceValue();
        FadeOutDmgFX();
    }

    void FadeOutDmgFX()
    {
        //피격 이미지 불투명도를 서서히 낮춤 (1%이상이면)
        if (canvasGroup_DamageFX.alpha >= 0.01f)
            canvasGroup_DamageFX.alpha =
            Mathf.MoveTowards(canvasGroup_DamageFX.alpha, 0, Time.deltaTime * 2);
        //0%로 만듬 (1% 이하, 0% 이상이면)
        else if (canvasGroup_DamageFX.alpha != 0)
            canvasGroup_DamageFX.alpha = 0;
    }

    //피격 이미지 불투명도 100%
    public void DamageFX () {
        canvasGroup_DamageFX.alpha = 1;
    }
    public void SetExpFill(float value)
    {
        LvImg.fillAmount = value;
    }
    public void SetLvText(string _Lv)
    {
        LvTxt.text = _Lv;
    }

    public void UpdateStatUI(string _SetTxt)
    {
        StatTxt.text = _SetTxt;
    }
}
