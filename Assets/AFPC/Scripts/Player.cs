using UnityEngine;
using AFPC;

public class Player : MonoBehaviour {

    
    public HUD HUD;// UI 

    //아래 클래스들은 네임스페이스에서 가져옴
    public Lifecycle lifecycle;// Damage, Heal, Death, Respawn... 
    public Movement movement;// Move, Jump, Run... 
    public Overview overview;// Look, Aim, Shake... 
    public CloseWeaponController closeWeaponController;

    public static bool isInv;

    //클래스 초기화 대신 해주기. (네임스페이스에서 가져온거라 그런지 start 늘리기 싫은지, 각 start에서 해결하는 대신 여기서 함수 호출하는 방식)
    //액션에 내용추가
    void Start () {

        // a few apllication settings(for more smooth). This is Optional
        QualitySettings.vSyncCount = 0; //모니터 주파수에 맞게 렌더링 퍼포먼스 조절. 성능 측정시 끔. tearing 현상 방지
        //Cursor.lockState = CursorLockMode.Locked; //중앙 좌표에 고정. 커서 비가시. <=> .None

        /* Initialize lifecycle, add Damage FX */
        lifecycle.Initialize();
        lifecycle.AssignDamageAction(DamageFX); //인자 없어서 그냥 함수 이름으로 넣음

        /* Initialize movement, add camera shake when landing */
        movement.Initialize();
        //movement.AssignLandingAction( ()=> overview.Shake(0.5f)); //인자가 있어서 화살표 함수로 넣음
    }

    void Update () {
        ReadInput();

        if (!lifecycle.Availability()) return; //죽었으면 함수 끝내버림

        if (!isInv)
        {
            overview.Looking();
            //overview.rigidInit(movement.rb);// Mouse look state 
            overview.Aiming();// Change camera FOV state 
            overview.Shaking();// Shake camera state. Required "physical camera" mode on 

            movement.Move();
            movement.TryRun();// Control the speed 
            movement.Jumping();// Control the jumping, ground search... 

            closeWeaponController.TryAttack();
        }
        //체력, 방어력 회복
        lifecycle.Runtime();// Control the health, shield recovery 
    }


    void ReadInput () {
        //테스트용 코드
        if (Input.GetKeyDown (KeyCode.R)) lifecycle.Damage(50);
        if (Input.GetKeyDown (KeyCode.H)) lifecycle.Heal(50);
        if (Input.GetKeyDown (KeyCode.T)) lifecycle.Respawn();


        movement.jumpingInputValue = Input.GetButtonDown("Jump");
        movement.runningInputValue = Input.GetKey(KeyCode.LeftShift);
        movement.runningCancelInputValue = Input.GetKeyUp(KeyCode.LeftShift);

        movement.movementInputValues.x = Input.GetAxisRaw("Horizontal");
        movement.movementInputValues.y = Input.GetAxisRaw("Vertical");


        overview.aimingInputValue = Input.GetKey(KeyCode.V);

        overview.lookingInputValues.x = Input.GetAxisRaw("Mouse X");
        overview.lookingInputValues.y = Input.GetAxisRaw("Mouse Y");
    }

    void DamageFX () {
        if (HUD)
            HUD.DamageFX();
        overview.Shake(0.05f);
    }
}
