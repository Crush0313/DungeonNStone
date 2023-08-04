using UnityEngine;

namespace AFPC {

    //look around, perform some POV effects

    [System.Serializable]
    public class Overview {

        public bool isDebugLog;

        [Header("Inputs")]
        public Vector2 lookingInputValues;
        public bool aimingInputValue;

        [Header("Following")]
        public Vector3 cameraOffset = new Vector3(0,0.8f,0);
	
        [Header ("Looking")]
        bool isLookingAvaialbe = true;
        public bool isHorizontalInverted;
	    public bool isVerticalInverted = true;
	    public float sensitivity = 5.0f;
	    public float verticalRange = 50.0f;
        Vector3 targetRotation;
	
        [Header ("Search")]
	    public LayerMask searchMask = 0;
        public float searchDistance = 1;
	
        [Header ("Aiming")]
        public float defaultFOV = 80.0f;
	    public float aimingFOV = 40.0f;
	    bool isAimingAvailable = true;
	
        [Header ("Shaking")]
	    bool isShakingAvaiable = true;
        public float shakingAmount;
        public int shakingCount;
        int currentShakingCount;
        public Vector2 randomVec;

        [Header("References")]
        public Camera camera;
        public Rigidbody rb;

        // Allow & Ban
            /// Allow the controller to read looking input values and rotate the camera.
            public virtual void AllowLooking () {
                isLookingAvaialbe = true;
                if (isDebugLog) Debug.Log (camera.gameObject.name + ": Allow Looking");
            }
            public virtual void BanLooking () {
                isLookingAvaialbe = false;
                if (isDebugLog) Debug.Log (camera.gameObject.name + ": Ban Looking");
            }

            /// Allow the user to change camera FOV to view far objects.
            public virtual void AllowAiming () {
                isAimingAvailable = true;
                if (isDebugLog) Debug.Log (camera.gameObject.name + ": Allow Aiming");
            }
            /// Ban the user to change camera FOV to view far objects.
            /// The camera FOV value moves forward to the "default FOV" value.
            public virtual void BanAiming () {
                isAimingAvailable = false;
                if (isDebugLog) Debug.Log (camera.gameObject.name + ": Ban Aiming");
            }

            /// Allow camera shaking by lens shifting. Required "Physical camera" mode on.
            public virtual void AllowShaking () {
                isShakingAvaiable = true;
                if (isDebugLog) Debug.Log (camera.gameObject.name + ": Allow Shaking");
            }
            /// Ban camera shaking by lens shifting.
            public virtual void BanShaking () {
                isShakingAvaiable = false;
                if (isDebugLog) Debug.Log (camera.gameObject.name + ": Ban Shaking");
            }


        /// Rotate the camera with looking input values.
        /// Using it as a "Mouse look" in common cases.
        public virtual void Looking () {
		    if (!camera) return;
		    if (!isLookingAvaialbe) return;
            //카메라 없음 / 보기 비활성화

            targetRotation.x -= lookingInputValues.y * sensitivity;
            targetRotation.y = lookingInputValues.x * sensitivity;

			targetRotation.x = Mathf.Clamp (targetRotation.x, -verticalRange, verticalRange);
		    
		    camera.transform.localEulerAngles = new Vector3(targetRotation.x, 0 ,0); //카메라 회전
            Vector3 _characterRotationY = new Vector3(0, targetRotation.y, 0);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(_characterRotationY));
        }

        /// Changing the camera FOV value, or return to the default FOV value;
        public virtual void Aiming () {
            if (!isAimingAvailable) return;
            if (!camera) return;
            //카메라 없거나, 조준 비활성화

		    if (aimingInputValue) { //클릭 누르면 fov를 점점 조절
			    camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, aimingFOV, 10);
		    } //클릭 떼고 정상화(충분히 작은 값)될 때까지 fov 조절
		    else if (System.Math.Abs(camera.fieldOfView - defaultFOV) > Physics.sleepThreshold) {
			    camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, defaultFOV, 10);
		    }
	    }

        /// Raycast in the forward direction to search some objects.
        /// Good practice to use it for shooting or interaction.
        public GameObject Search () {
            if (Physics.Raycast(camera.transform.position + (camera.transform.forward * 0.5f), camera.transform.forward, out RaycastHit hit, searchDistance, searchMask)) {
                if (isDebugLog) Debug.Log ("GameObject found: " + hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }
            return null; //없으면 null 반환
	    }

        /// Shake the camera lens with value.
        public virtual void Shake(float value)
        {
            randomVec = AddRandromSphereVector(camera.lensShift, value);
            camera.lensShift = randomVec; //랜덤지정좌표로 렌즈쉬프트 이동
            currentShakingCount = shakingCount;
            if (isDebugLog) Debug.Log(camera.gameObject.name + ": Shake camera with: " + value + " value.");
        }

        Vector3 AddRandromSphereVector(Vector3 position, float amount)
        {
            return position + Random.insideUnitSphere * amount;
        }

        /// Control the camera lens shift values.
	    public virtual void Shaking () {
		    if (!isShakingAvaiable) return;
            //흔들기 비활성화시 탈출
            
            if (currentShakingCount > 0)
            { //카운트 깎으며 렌즈시프트 점진적 초기화
                camera.lensShift = Vector3.Lerp(camera.lensShift, Vector2.zero, shakingAmount);
                currentShakingCount--;
            }
            else //카운트 0이면 완전 초기화
                camera.lensShift = Vector2.zero;
	    }
    }
}
