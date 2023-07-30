using UnityEngine;
using UnityEngine.UI;

// 체력, 방어력, 기력 UI조절
// 피격 이미지 컨트롤

public class HUD : MonoBehaviour {

    [Header("References")]
    public Hero hero; //플레이어
    public Slider slider_Shield; //방어력
    public Slider slider_Health; //체력
    public Slider slider_Endurance; //기력
    public CanvasGroup canvasGroup_DamageFX; //피격 이미지(캔버스 그룹)

    //최대치 적용
    void Awake () {
        slider_Shield.maxValue = hero.lifecycle.referenceShield;
        slider_Health.maxValue = hero.lifecycle.referenceHealth;
        slider_Endurance.maxValue = hero.movement.referenceEndurance;
    }

    //현재치 적용
    //피격 이미지 불투명도 낮추기
    void Update () {
        slider_Shield.value = hero.lifecycle.GetShieldValue();
        slider_Health.value = hero.lifecycle.GetHealthValue();
        slider_Endurance.value = hero.movement.GetEnduranceValue();

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
}
