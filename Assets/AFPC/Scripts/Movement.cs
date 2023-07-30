using UnityEngine;
using UnityEngine.Events;

namespace AFPC {

    /// allows the user to move.
    [System.Serializable]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Movement {

        public bool isDebugLog;

        [Header("Inputs")]
        public Vector3 movementInputValues;
        public bool runningInputValue;
        public bool jumpingInputValue;

        [Header("Acceleration")]
        public float referenceAcceleration = 2.66f;
        float currentAcceleration = 2.5f;
        float movementSmoothing = 0.3f;
        Vector3 vector3_Reference;
        Vector3 vector3_Target;
        Vector3 delta;
        bool isMovementAvailable = true;
        bool releaseAcceleration = true;

        [Header("Running")]
        public float runningAcceleration = 5.32f;
        bool isRunningAvaiable = true;

        [Header("Endurance")]
        public float referenceEndurance = 20.0f;
        float endurance = 20.0f;

        [Header("Jumping")]
        public float jumpForce = 7.5f;
        bool isJumpingAvailable = true;
        bool isAirControl = true;
        Vector3 groungCheckPosition;
        bool isLandingActionPerformed;
        UnityAction landingAction;
	
        [Header("Physics")]
        public bool isGeneratePhysicMaterial = true;
        public float mass = 70.0f;
        public float drag = 3.0f;
	    [Tooltip ("For Initialize()")] public float height = 1.6f;

        [Header("Looking for ground")]
        public LayerMask groundMask = 1;
        bool isGrounded;

        [Header("References")]
        public Rigidbody rb;
        public CapsuleCollider cc;

        float epsilon = 0.01f;

        /// Initialize the movement. Generate physic material if needed. Prepare the rigidbody.
        public virtual void Initialize () {
            rb.freezeRotation = true;
            rb.mass = mass;
            rb.drag = drag;
            cc.height = height;

            if (isGeneratePhysicMaterial) {
                //물리 머테리얼 생성
                PhysicMaterial physicMaterial = new PhysicMaterial {
                    name = "Generated Material",
                    bounciness = 0.01f,
                    dynamicFriction = 0.0f,
                    staticFriction = 0.0f,
                    frictionCombine = PhysicMaterialCombine.Minimum,
                    bounceCombine = PhysicMaterialCombine.Minimum
                };
                //물리 머테리얼 적용
                cc.material = physicMaterial;
            }
        }

        //allow / ban / 상태변수
            /// Allow the user to move.
            public virtual void AllowMovement ()
            {
                if (isDebugLog) Debug.Log(rb.gameObject.name + ": Allow Movement");
                isMovementAvailable = true;
            }
            //Optional, immediately stop the rigidbody.
            public virtual void BanMovement (bool isStopImmediately = false)
            {
                if (isDebugLog) Debug.Log(rb.gameObject.name + ": Ban Movement");

                isMovementAvailable = true;
                if (isStopImmediately) {
                    rb.velocity = Vector3.zero;
                    if (isDebugLog) Debug.Log (rb.gameObject.name + ": Stop Movement");
                }
            }

            /// Allow the user to move faster.
            public virtual void AllowRunning () {
                isRunningAvaiable = true;
                if (isDebugLog) Debug.Log (rb.gameObject.name + ": Allow Running");
            }
            public virtual void BanRunning () {
                isRunningAvaiable = false;
                if (isDebugLog) Debug.Log (rb.gameObject.name + ": Ban Running");
            }

            /// Allow the user to jump up.
            public virtual void AllowJumping () {
                isJumpingAvailable = true;
                if (isDebugLog) Debug.Log (rb.gameObject.name + ": Allow Jumping");
            }
            public virtual void BanJumping () {
                isJumpingAvailable = false;
                if (isDebugLog) Debug.Log (rb.gameObject.name + ": Ban Jumping");
            }

            // Allow the user to change movement direction in the air.
            // 공중에서 이동방향 전환. 마크는 true (속도는 느림)
            public virtual void AllowAirControl()
            {
                isAirControl = true;
                if (isDebugLog) Debug.Log(rb.gameObject.name + ": Allow Air Control");
            }
            public virtual void BanAirControl()
            {
                isAirControl = false;
                if (isDebugLog) Debug.Log(rb.gameObject.name + ": Ban Air Control");
            }

            /// Perform an action when the character was landed.
            public void AssignLandingAction (UnityAction action) {
                landingAction = action;
            }
            public void ClearLandingAction () {
                landingAction = null;
            }

            /// Current endurance value.
            public float GetEnduranceValue () {
                return endurance;
            }
            /// Is this controller on the ground?
            public bool IsGrounded () {
                return isGrounded;
            }

        /// Jumping state. Better use it in Update.
        public virtual void Jumping()
        {
            if (!isJumpingAvailable) return; //점프 불가 시 탈출

            if (isGrounded) //땅에 있어야
            {
                if (jumpingInputValue) //점프 누르면
                { //velocity 조절로 점프
                    rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                }
            }
        }

        /// Physical movement. Better use it in FixedUpdate.
        public virtual void Accelerate () {
            LookingForGround ();
            MoveTorwardsAcceleration ();

            if (!isMovementAvailable) return;
            if (!rb) return;
            if (System.Math.Abs(movementInputValues.x) < epsilon & System.Math.Abs(movementInputValues.y) < epsilon) return;
            if (!isAirControl) {
                if (!isGrounded) return; //에어컨트롤 비활성화에 공중상태
            }

            //속도에 따라 움직임 보정방법 선택
            if (rb.velocity.magnitude > 1.0f) { //벡터 크기
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
            }
            else {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }

            delta = new Vector3 (movementInputValues.x, 0, movementInputValues.y);
            delta = Vector3.ClampMagnitude (delta, 1);
            delta = rb.transform.TransformDirection (delta) * currentAcceleration;

            vector3_Target = new Vector3 (delta.x, rb.velocity.y, delta.z);
            rb.velocity = Vector3.SmoothDamp (rb.velocity, vector3_Target, ref vector3_Reference, Time.smoothDeltaTime * movementSmoothing);
        }

        /// Running state. Better use it in Update.
	    public virtual void Running () {
		    if (!isRunningAvaiable) return;
		    if (!isGrounded) return;

		    if (runningInputValue && endurance > 0.05f) {
                releaseAcceleration = false;
			    endurance -= Time.deltaTime * 2; //기력 감소
			    currentAcceleration = Mathf.MoveTowards (currentAcceleration, runningAcceleration, Time.deltaTime * 10);
		    }
		    else {
                releaseAcceleration = true;
			    if (System.Math.Abs(endurance - referenceEndurance) > epsilon) {
                    endurance = Mathf.MoveTowards (endurance, referenceEndurance, Time.deltaTime);
                }
		    }
	    }

        void LookingForGround () {
            groungCheckPosition = new Vector3 (cc.transform.position.x, cc.transform.position.y - height / 2, cc.transform.position.z);
           
            if (Physics.CheckSphere (groungCheckPosition, 0.1f, groundMask, QueryTriggerInteraction.Ignore)) {
                isGrounded = true;

                if (!isLandingActionPerformed) { //최초 한번 실행
                    isLandingActionPerformed = true;
                    landingAction?.Invoke();
                }
                rb.drag = drag; //공기 저항력
            }
            else
            { //공중 
                isGrounded = false;
                isLandingActionPerformed = false;
                rb.drag = 0.5f; 
            }
        }

        void MoveTorwardsAcceleration () {
            if (!releaseAcceleration) return;

            //차이가 작아질 때까지
            if (System.Math.Abs(currentAcceleration - referenceAcceleration) > epsilon) {
                currentAcceleration = Mathf.MoveTowards (currentAcceleration, referenceAcceleration, Time.deltaTime * 10);
            }
        }
    }
}