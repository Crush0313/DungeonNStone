using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AFPC {

    /// <summary>
    /// This class contain currentHp-damage-death cycle.
    /// </summary>
    [System.Serializable]
    public class Lifecycle {

        public bool isDebugLog;
        bool isAvailable = true;
        public string ID = "AFPC";

        [Header("Hp")]
	    public float MaxHp = 100.0f;
	    public float currentHp = 1;
        bool isHpRecovery = true;
        int hpRecoveryRate = 60;
	
        [Header("Mana")]
	    public float MaxMp = 100.0f;
	    public float currentMp = 1;
        bool isMpRecovery = true;
        int mpRecoveryRate = 60;

        bool isFrenzy;
        float frenzyThreshold = 20.0f;

        float epsilon = 0.01f;

        public HUD hud;


        UnityAction healAction;
        UnityAction damageAction;
        UnityAction deathAction;

        /// Set maximum currentHp and Mp in the start.
        public virtual void Initialize () {
	        SetMaximumHpAndMp();
        }

        /// Restore the currentHp and Mp to the maximum.
        public virtual void SetMaximumHpAndMp()
        {
            currentHp = MaxHp;
            currentMp = MaxMp;
            hud.updateValue();
            if (isDebugLog) Debug.Log(ID + ": Set Maximum Hp and Mp.");
        }
        /// Drive the currentHp and Mp values to the 1.
        public virtual void SetMinimumHpAndMp()
        {
            currentHp = 1;
            currentMp = 1;
            hud.updateValue();
            if (isDebugLog) Debug.Log(ID + ": Set Minimum Hp and Mp.");
        }

        // Check the availability of this character.
        // 생존 여부 반환
        public bool Availability() {
            return isAvailable;
        }

        // Activate the character.
        public virtual void Activate () {
            isAvailable = true;
            if (isDebugLog) Debug.Log (ID + ": Active.");
        }
        public virtual void Deactivate () {
            isAvailable = false;
            if (isDebugLog) Debug.Log (ID + ": Not active.");
        }

        /// Current currentHp of the character.
        public float GetHpValue () {
            return currentHp;
        }
        /// The currentHp of the character will increase in 1 every "value" frames.
        public void SetHpRecoveryRate (int value) {
            hpRecoveryRate = value;
        }
        /// Allow this character to recover currentHp.
        public virtual void AllowHpRecovery () {
            isHpRecovery = true;
            if (isDebugLog) Debug.Log (ID + ": Allow Hp Recovery.");
        }
        public virtual void BanHpRecovery () {
            isHpRecovery = false;
            if (isDebugLog) Debug.Log (ID + ": Ban Hp Recovery.");
        }

        /// Current Mp of the character.
        public float GetMpValue () {
            return currentMp;
        }
        /// The Mp of the character will increase in 1 every "value" frames.
        public void SetMpRecoveryRate (int value) {
            mpRecoveryRate = value;
        }
        /// Allow this character to recover currentHp.
        public virtual void AllowMpRecovery () {
            isMpRecovery = true;
            if (isDebugLog) Debug.Log (ID + ": Allow Shiled Recovery.");
        }
        public virtual void BanMpRecovery () {
            isMpRecovery = false;
            if (isDebugLog) Debug.Log (ID + ": Ban Shiled Recovery.");
        }

        /// Check the Frenzy state.
        /// The Frenzy state is used to give your users a special state( when his currentHp level is low.)
        public bool IsFrenzy()
        {
            return isFrenzy;
        }
        /// Set a minimum currentHp threshold for the frenzy state.
        public void SetFrenzyThreshold(float value)
        {
            frenzyThreshold = value;
            if (isDebugLog) Debug.Log(ID + ": Frenzy threshold is: " + value);
        }

        //액션
        /// Perform an action when the character was damaged.
        public void AssignDamageAction(UnityAction action)
        {
            damageAction = action;
        }
        /// Perform an action when the character was healed.
        public void AssignHealAction(UnityAction action)
        {
            healAction = action;
        }
        /// Perform an action when the character dies.
        public void AssignDeathAction(UnityAction action)
        {
            deathAction = action;
        }

        //player update에서 호출
        /// Recovering currentHp, Mp.
	    public virtual void Runtime () {
		    HpRecovery ();
		    MpRecovery ();
	    }

	    void HpRecovery () {
		    if (!isHpRecovery) return;
		    if (hpRecoveryRate == 0) return; //rate 0이면 탈출

		    if (Time.frameCount % hpRecoveryRate == 0 
                && Mathf.Abs(currentHp - MaxHp) > epsilon) {
			    if (currentHp < MaxHp) { //풀체력이 아닐 때
				    currentHp += 1;
				    CheckFrenzy ();
			    }
			    else {
				    currentHp = MaxMp;
                }
                hud.updateValue();
            }
	    }

	    void MpRecovery () {
		    if (!isMpRecovery) return;
		    if (mpRecoveryRate == 0) return;

		    if (Time.frameCount % mpRecoveryRate == 0 && Mathf.Abs(currentMp - MaxMp) > epsilon) {
			    if (currentMp < MaxMp) {
				    if (!isMpRecovery) return;
				    currentMp += 1;
			    }
			    else {
				    currentMp = MaxMp;
                }
                hud.updateValue();
            }


        }

        /// Damage the character. The Mp will be damaged first.
        public virtual void Damage (float value) {
            if (!isAvailable) return;

            float MpDamage = Mathf.Min (currentMp, value);
            float currentHpDamage = Mathf.Min(currentHp, value - MpDamage);
            currentMp -= MpDamage;
            currentHp -= currentHpDamage;
            if (Mathf.Abs(currentHp) < epsilon) {
                Death ();
            }
            damageAction?.Invoke();
            if (isDebugLog) Debug.Log (ID + ": Damaged: " + value);
            hud.updateValue();
        }



        /// Heal the character.
        public virtual void Heal (float value) {
            if (!isAvailable) return;

            float currentHpHeal = Mathf.Min (MaxHp - currentHp, value);
            float MpHeal = Mathf.Min (MaxMp - currentMp, value - currentHpHeal);
            currentHp += currentHpHeal;
            currentMp += MpHeal;
            healAction?.Invoke();

            if (isDebugLog) Debug.Log (ID + ": Healed: " + value);
            hud.updateValue();
        }

        bool CheckFrenzy () {
		    isFrenzy = false;
            isFrenzy |= currentHp < (MaxHp / 100) * frenzyThreshold;
		    return isFrenzy;
	    }

        /// Activate the character and restore currentHp and Mp.
        public virtual void Respawn() {
            if (isAvailable) return;

            Activate();
            AllowHpRecovery();
            AllowMpRecovery();
            SetMaximumHpAndMp();
            CheckFrenzy ();

            if (isDebugLog) Debug.Log (ID + ": Respawn");
        }

        /// Deactivate the character and set currentHp and Mp to the minimum.
        public virtual void Death () {
            if (!isAvailable) return;

            Deactivate();
		    isFrenzy = false;
		    BanHpRecovery();
            BanMpRecovery();
		    SetMinimumHpAndMp();
            deathAction?.Invoke();

            if (isDebugLog) Debug.Log (ID + ": Death");
        }

    }
}
