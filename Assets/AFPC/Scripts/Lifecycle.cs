using UnityEngine;
using UnityEngine.Events;

namespace AFPC {

    /// <summary>
    /// This class contain health-damage-death cycle.
    /// </summary>
    [System.Serializable]
    public class Lifecycle {

        public bool isDebugLog;
        bool isAvailable = true;
        public string ID = "AFPC";

        [Header("Health")]
	    public float referenceHealth = 100.0f;
	    float health = 1;
        bool isHealthRecovery = true;
        int healthRecoveryRate = 60;
	
        [Header("Shield")]
	    public float referenceShield = 100.0f;
	    float shield = 1;
        bool isShieldRecovery = true;
        int shieldRecoveryRate = 60;

        bool isFrenzy;
        float frenzyThreshold = 20.0f;

        float epsilon = 0.01f;

        
        UnityAction healAction;
        UnityAction damageAction;
        UnityAction deathAction;

        /// Set maximum health and shield in the start.
        public virtual void Initialize () {
	        SetMaximumHealthAndShield();
        }

        /// Restore the health and shield to the maximum.
        public virtual void SetMaximumHealthAndShield()
        {
            health = referenceHealth;
            shield = referenceShield;
            if (isDebugLog) Debug.Log(ID + ": Set Maximum Health and Shield.");
        }
        /// Drive the health and shield values to the 1.
        public virtual void SetMinimumHealthAndShield()
        {
            health = 1;
            shield = 1;
            if (isDebugLog) Debug.Log(ID + ": Set Minimum Health and Shield.");
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

        /// Current health of the character.
        public float GetHealthValue () {
            return health;
        }
        /// The health of the character will increase in 1 every "value" frames.
        public void SetHealthRecoveryRate (int value) {
            healthRecoveryRate = value;
        }
        /// Allow this character to recover health.
        public virtual void AllowHealthRecovery () {
            isHealthRecovery = true;
            if (isDebugLog) Debug.Log (ID + ": Allow Health Recovery.");
        }
        public virtual void BanHealthRecovery () {
            isHealthRecovery = false;
            if (isDebugLog) Debug.Log (ID + ": Ban Health Recovery.");
        }

        /// Current shield of the character.
        public float GetShieldValue () {
            return shield;
        }
        /// The shield of the character will increase in 1 every "value" frames.
        public void SetShieldRecoveryRate (int value) {
            shieldRecoveryRate = value;
        }
        /// Allow this character to recover health.
        public virtual void AllowShieldRecovery () {
            isShieldRecovery = true;
            if (isDebugLog) Debug.Log (ID + ": Allow Shiled Recovery.");
        }
        public virtual void BanShieldRecovery () {
            isShieldRecovery = false;
            if (isDebugLog) Debug.Log (ID + ": Ban Shiled Recovery.");
        }

        /// Check the Frenzy state.
        /// The Frenzy state is used to give your users a special state( when his health level is low.)
        public bool IsFrenzy()
        {
            return isFrenzy;
        }
        /// Set a minimum health threshold for the frenzy state.
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
        /// Recovering health, shield.
	    public virtual void Runtime () {
		    HealthRecovery ();
		    ShieldRecovery ();
	    }

	    void HealthRecovery () {
		    if (!isHealthRecovery) return;
		    if (healthRecoveryRate == 0) return; //rate 0이면 탈출

		    if (Time.frameCount % healthRecoveryRate == 0 
                && Mathf.Abs(health - referenceHealth) > epsilon) {
			    if (health < referenceHealth) { //풀체력이 아닐 때
				    health += 1;
				    CheckFrenzy ();
			    }
			    else {
				    health = referenceShield;
			    }
		    }
	    }

	    void ShieldRecovery () {
		    if (!isShieldRecovery) return;
		    if (shieldRecoveryRate == 0) return;

		    if (Time.frameCount % shieldRecoveryRate == 0 && Mathf.Abs(shield - referenceShield) > epsilon) {
			    if (shield < referenceShield) {
				    if (!isShieldRecovery) return;
				    shield += 1;
			    }
			    else {
				    shield = referenceShield;
			    }
		    }
	    }

        /// Damage the character. The shield will be damaged first.
        public virtual void Damage (float value) {
            if (!isAvailable) return;

            float shieldDamage = Mathf.Min (shield, value);
            float healthDamage = Mathf.Min (health, value - shieldDamage);
            shield -= shieldDamage;
            health -= healthDamage;
            if (Mathf.Abs(health) < epsilon) {
                Death ();
            }
            damageAction?.Invoke();
            if (isDebugLog) Debug.Log (ID + ": Damaged: " + value);
        }



        /// Heal the character.
        public virtual void Heal (float value) {
            if (!isAvailable) return;

            float healthHeal = Mathf.Min (referenceHealth - health, value);
            float shieldHeal = Mathf.Min (referenceShield - shield, value - healthHeal);
            health += healthHeal;
            shield += shieldHeal;
            healAction?.Invoke();

            if (isDebugLog) Debug.Log (ID + ": Healed: " + value);
        }

        bool CheckFrenzy () {
		    isFrenzy = false;
            isFrenzy |= health < (referenceHealth / 100) * frenzyThreshold;
		    return isFrenzy;
	    }

        /// Activate the character and restore health and shield.
        public virtual void Respawn() {
            if (isAvailable) return;

            Activate();
            AllowHealthRecovery();
            AllowShieldRecovery();
            SetMaximumHealthAndShield();
            CheckFrenzy ();

            if (isDebugLog) Debug.Log (ID + ": Respawn");
        }

        /// Deactivate the character and set health and shield to the minimum.
        public virtual void Death () {
            if (!isAvailable) return;

            Deactivate();
		    isFrenzy = false;
		    BanHealthRecovery();
            BanShieldRecovery();
		    SetMinimumHealthAndShield();
            deathAction?.Invoke();

            if (isDebugLog) Debug.Log (ID + ": Death");
        }

    }
}
