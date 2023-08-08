using System.Linq.Expressions;
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
        public bool runningCancelInputValue;
        public bool jumpingInputValue;

        [Header("Acceleration")]
        bool isMovementAvailable = true;

        [Header("Running")]
        public float runningAcceleration = 5.32f;
        bool isRunningAvaiable = true;
        public float applySpeed;
        public float walkSpeed;
        public Vector3 _velocity;
        public bool isRun = false;
        Overview overview;
        public float FovAmmount;

        [Header("Endurance")]
        public float referenceEndurance = 20.0f;
        float endurance = 20.0f;

        [Header("Jumping")]
        public float jumpForce = 7.5f;
        bool isJumpingAvailable = true;
        bool isAirControl = true;
	
        [Header("Physics")]
        public bool isGeneratePhysicMaterial = true;
        public CapsuleCollider collider;
        public float drag = 3.0f;

        [Header("Looking for ground")]
        public LayerMask groundMask = 1;
        public bool isGrounded;

        [Header("References")]
        public Rigidbody rb;
        public CapsuleCollider cc;

        float epsilon = 0.01f;

        /// Initialize the movement. Generate physic material if needed. Prepare the rigidbody.
        public virtual void Initialize (Overview _overview) {
            rb.freezeRotation = true;
            rb.drag = drag;
            applySpeed = walkSpeed;
            overview = _overview;

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
                    rb.velocity = rb.transform.up * jumpForce; ;
                }
            }
        }



        //이동
        public void Move()
        {
            IsGround();

            if (!isMovementAvailable) return;
            if (!rb) return;
            if (!isAirControl)
            {
                if (!isGrounded) return; //에어컨트롤 비활성화에 공중상태
            }

            //Vector3 _velocity = new Vector3(movementInputValues.x, 0, movementInputValues.y).normalized * applySpeed;
            //rb.MovePosition(rb.transform.position + _velocity * Time.deltaTime);
            Vector3 _moveHorizontal = rb.transform.right * movementInputValues.x;
            Vector3 _moveVertical = rb.transform.forward * movementInputValues.y;
             _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
            rb.MovePosition(rb.transform.position + _velocity * Time.deltaTime);
        }
        //점프
        private void IsGround()
        {
            isGrounded = Physics.Raycast(rb.transform.position, Vector3.down, collider.bounds.extents.y + 0.1f); //미끄러지는 수준은 무시
            
            if (isGrounded)
                rb.drag = drag;
            else
                rb.drag = 0.5f;

        }
        //달리기
        public void TryRun()
        {
            if (runningInputValue)
            {
                Runnung();
            }
            if (runningCancelInputValue)
            {
                runningCancel();
            }



        }
        void Runnung()
        {
            if (!isRunningAvaiable) return;
            if (!isGrounded) return;

            if (!isRun)
            {
                isRun = true;
                applySpeed = walkSpeed * 2f;
                overview.aimingFOV += FovAmmount;
                overview.defaultFOV += FovAmmount;
            }

            endurance -= Time.deltaTime * 2; //기력 감소

        }
        public void runningCancel()
        {
            if (isRun)
            {
                isRun = false;
                applySpeed = walkSpeed;
                overview.aimingFOV -= FovAmmount;
                overview.defaultFOV -= FovAmmount;
            }
        }

    }

}