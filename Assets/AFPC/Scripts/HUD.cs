using UnityEngine;
using UnityEngine.UI;

// 체력, 방어력, 기력 UI조절
// 피격 이미지 컨트롤

public class HUD : MonoBehaviour {

    [Header("References")]
    public Player player; //플레이어
    public Slider slider_Shield; //방어력
    public Slider slider_Health; //체력
    public Slider slider_Endurance; //기력

    public Image LvImg; //경험치 이미지
    public Text LvTxt; //레벨 텍스트

    public CanvasGroup canvasGroup_DamageFX; //피격 이미지(캔버스 그룹)

    //최대치 적용
    void Awake () {
        slider_Shield.maxValue = player.lifecycle.referenceShield;
        slider_Health.maxValue = player.lifecycle.referenceHealth;
        slider_Endurance.maxValue = player.movement.referenceEndurance;
    }

    //현재치 적용
    //피격 이미지 불투명도 낮추기
    void Update () {
        slider_Shield.value = player.lifecycle.GetShieldValue();
        slider_Health.value = player.lifecycle.GetHealthValue();
        slider_Endurance.value = player.movement.GetEnduranceValue();

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
}
