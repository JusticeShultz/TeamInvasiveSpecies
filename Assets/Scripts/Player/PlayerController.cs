using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour
{
    public enum FireMode { Sniper, Automatic }

    public static GameObject Player;
    public static PlayerController Player_Controller;

    [Header("Player Controls")]

    [Header("Input")]
    public SteamVR_Action_Vector2 joystickAction;
    public SteamVR_Action_Boolean cameraAction;
    public SteamVR_Action_Boolean waterBombAction;
    public SteamVR_Input_Sources MoveHand;
    public SteamVR_Input_Sources RotateHand;
    public SteamVR_Input_Sources CameraHand;
    public SteamVR_Input_Sources WaterBombHand;
    public Rigidbody controllerRigidbody;
    public GameObject RightRemote;
    public GameObject OffsetObject;
    public GameObject WaterBombObject;
    public Animator animator;

    [Header("Values")]
    public float acceleration = 2.0f;
    public float MaxVelocity = 3;
    public float MaxFallSpeed = 35;
    public float RotationThreshhold = 0.7f;
    public float RotationSensitivity = 0.35f;
    [Range(0.5f, 20f)] public float WaterBombCooldown = 10f;
    [ReadOnlyField] public Vector3 moving = Vector3.zero;
    [ReadOnlyField] public float rotation;

    [Header("Player Shooting Controls")]

    [Header("Input")]
    public SteamVR_Action_Boolean triggerAction;
    public SteamVR_Input_Sources ShootingHand;
    public SteamVR_Action_Boolean switchGunTypeAction;
    public SteamVR_Input_Sources SwitchGunTypeHand;
    public GameObject BulletObject_Sniper;
    public GameObject BulletObject_Automatic;
    public GameObject ShotPoint;
    public GameObject ShotPoint_Automatic;
    public MeshRenderer ChargeUpDrawer_Renderer;
    public ScrollingUV ChargeUpDrawer_UV;
    public ParticleSystem CooldownSystem;
    public SteamVR_Action_Vibration hapticFlash = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");
    public Animator railgunAnimator;
    public GameObject railgun_Auto_FX;

    [Header("Values")]
    public float BulletDamage_Sniper = 100.0f;
    public float BulletDamage_Automatic = 100.0f;
    public float GunCooldownTime = 1.5f;
    public float ShotChargeRate_Sniper = 1.0f;
    public float ShotChargeRate_Automatic = 0.1f;
    public float ShotFlySpeed_Sniper = 80f;
    public float ShotFlySpeed_Automatic = 40f;
    public int BulletPoolSize = 50;
    public FireMode GunFireMode;
    private bool LastSwitchState = false;

    [ReadOnlyField] public float local_ChargeTime;
    [ReadOnlyField] public float local_CooldownTime_Sniper;
    [ReadOnlyField] public float local_CooldownTime_Automatic;
    [ReadOnlyField] public List<PlayerBullet> bulletPool_Sniper = new List<PlayerBullet>();
    [ReadOnlyField] public List<PlayerBullet> bulletPool_Automatic = new List<PlayerBullet>();

    [Header("Player Health System")]
    [Header("Values")]
    public float MaxHealth = 10f;
    public float PlayerArmor = 1.0f;
    public float CurrentHealth = 10f;

    [Header("Hand Detection System")]
    [Header("Objects")]
    public GameObject HMDPos;
    public GameObject LeftHandPos;
    public SteamVR_Input_Sources LeftHand_InputSource;
    public SteamVR_Input_Sources RightHand_InputSource;
    public GameObject LoseObject;
    public GameObject WinObject;

    private GameObject LeftHand;
    private GameObject RightHand;
    private int LookPoint = 0;
    private bool GripToggle = false;
    private float localWaterBombCooldown;

    void Start()
    {
        for (int i = 0; i < BulletPoolSize; ++i)
        {
            PlayerBullet obj = Instantiate(BulletObject_Sniper, Vector3.zero, Quaternion.identity).GetComponent<PlayerBullet>();
            bulletPool_Sniper.Add(obj);
            obj.gameObject.SetActive(false);

            PlayerBullet obj1 = Instantiate(BulletObject_Automatic, Vector3.zero, Quaternion.identity).GetComponent<PlayerBullet>();
            bulletPool_Automatic.Add(obj1);
            obj1.gameObject.SetActive(false);
        }

        if (Player) Destroy(Player);
        if (Player_Controller) Destroy(Player_Controller.gameObject);

        Player = gameObject;
        Player_Controller = this;
    }

    void Update()
    {
        Application.targetFrameRate = 144;

        #region Hand Decider
        /*
        Vector3 heading = LeftHandPos.transform.position - HMDPos.transform.position;
        float dirNum = AngleDir(transform.forward, heading, transform.up);

        if(dirNum == 1)
        {
            MoveHand = LeftHand_InputSource;
            RotateHand = RightHand_InputSource;
        }
        else
        {
            MoveHand = RightHand_InputSource;
            RotateHand = LeftHand_InputSource;
        }
        */
        #endregion

        #region Movement
        controllerRigidbody.velocity = new Vector3(controllerRigidbody.velocity.x, Mathf.Clamp(controllerRigidbody.velocity.y, -MaxFallSpeed, MaxFallSpeed), controllerRigidbody.velocity.z);
        Vector2 input1 = joystickAction.GetAxis(MoveHand);

        moving = Vector3.zero;

        moving += Vector3.ProjectOnPlane(RightRemote.transform.forward, transform.up) * input1.y;
        moving += Vector3.ProjectOnPlane(RightRemote.transform.right, transform.up) * input1.x;
        float wishSpeed = Mathf.Min(MaxVelocity, moving.magnitude * MaxVelocity);
        moving.Normalize();
        Vector3 addVelocity = moving * acceleration;
        float preSpeed = controllerRigidbody.velocity.magnitude;
        float postSpeed = (controllerRigidbody.velocity + addVelocity).magnitude;
        if (postSpeed <= preSpeed || //don't stop the player from slowing down. Why would you?
            postSpeed <= wishSpeed) //acceleration does not break cap
        {
            //everything is fine. apply acceleration
            controllerRigidbody.velocity = controllerRigidbody.velocity + addVelocity; 
        } else //acceleration and will exceede bounds;
        {
            //clamp to max
            controllerRigidbody.velocity = (controllerRigidbody.velocity + addVelocity).normalized * Mathf.Max(wishSpeed,preSpeed) + new Vector3(0, controllerRigidbody.velocity.y, 0);
        }
        //controllerRigidbody.velocity

        if (input1.y > 0.1 && input1.y > input1.x) //Up
            animator.SetInteger("Direction", 1);
        else if (input1.x > 0.1 && input1.x > input1.y) //Right
            animator.SetInteger("Direction", 2);
        else if (input1.y < -0.1 && input1.y < input1.x) //Backwards
            animator.SetInteger("Direction", 3);
        else if (input1.x < -0.1 && input1.x < input1.y) //Left
            animator.SetInteger("Direction", 4);
        else animator.SetInteger("Direction", 0);

        if (animator.GetInteger("Direction") == 0)
            animator.SetBool("Moving", false);
        else animator.SetBool("Moving", true);

        Debug.DrawLine(transform.position, transform.position + (RightRemote.transform.forward * 4), Color.red);
        Debug.DrawLine(transform.position, transform.position + (moving * 4), Color.yellow);

        #endregion

        #region Rotation

        Vector2 input2 = joystickAction.GetAxis(RotateHand);
        rotation += input2.x * RotationSensitivity;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotation, 0), RotationSensitivity);
        #endregion

        #region Shooting
        bool triggerDown = triggerAction.GetState(RotateHand);

        bool switchGun = switchGunTypeAction.GetState(SwitchGunTypeHand);

        if (switchGun)
        {
            if(!LastSwitchState)
            {
                LastSwitchState = true;

                if (GunFireMode == FireMode.Sniper)
                {
                    GunFireMode = FireMode.Automatic;
                    railgunAnimator.SetTrigger("SwitchToMinigun");
                }
                else if (GunFireMode == FireMode.Automatic)
                {
                    GunFireMode = FireMode.Sniper;
                    railgunAnimator.SetTrigger("SwitchToRailgun");
                }
            }
        }
        else LastSwitchState = false;

        if (triggerDown)
            animator.SetBool("IsChargingWeapon", true);
        else animator.SetBool("IsChargingWeapon", false);

        railgunAnimator.SetBool("IsFiring", triggerDown);

        if (GunFireMode == FireMode.Sniper)
        {
            if (triggerDown && local_CooldownTime_Sniper <= 0)
            {
                local_ChargeTime += Time.deltaTime;

                //Do charge effect here
                ChargeUpDrawer_Renderer.enabled = true;
                ChargeUpDrawer_UV.uvAnimationRate = new Vector2(local_ChargeTime * 1.5f, local_ChargeTime);

                if (local_ChargeTime >= ShotChargeRate_Sniper)
                {
                    //Do fire effect here

                    foreach (PlayerBullet bullet in bulletPool_Sniper) if (!bullet.gameObject.activeSelf)
                    {
                        bullet.gameObject.SetActive(true);
                        bullet.GetComponent<TrailRenderer>().Clear();
                        bullet.transform.position = ShotPoint.transform.position;
                        bullet.transform.rotation = transform.rotation;
                        bullet.FireVector = transform.forward;
                        bullet.ShotSpeed = ShotFlySpeed_Sniper;
                        bullet.Damage = BulletDamage_Sniper;
                        bullet.GetComponent<TrailRenderer>().Clear();
                        hapticFlash.Execute(0, 1.5f, 50000.0f, 50000, ShootingHand);
                        hapticFlash.Execute(0, 1.5f, 50000.0f, 50000, RotateHand);
                        break;
                    }

                    local_CooldownTime_Sniper = GunCooldownTime;
                    local_ChargeTime = 0f;
                }
            }
            else
            {
                ChargeUpDrawer_UV.uvAnimationRate = Vector2.zero;
                ChargeUpDrawer_Renderer.enabled = false;

                local_ChargeTime = 0f;

                if (local_CooldownTime_Sniper > 0)
                {
                    //Do steam cooldown effect here
                    local_CooldownTime_Sniper -= Time.deltaTime;

                    var emission = CooldownSystem.emission;
                    emission.rateOverTime = 50;
                }
                else
                {
                    var emission = CooldownSystem.emission;
                    emission.rateOverTime = 0;
                }
            }
        }

        if (triggerDown && GunFireMode == FireMode.Automatic) railgun_Auto_FX.SetActive(true);
        else railgun_Auto_FX.SetActive(false);

        if (GunFireMode == FireMode.Automatic)
        {
            var emission0 = CooldownSystem.emission;
            emission0.rateOverTime = 0;

            if (triggerDown && local_CooldownTime_Automatic <= 0)
            {
                foreach (PlayerBullet bullet in bulletPool_Automatic) if (!bullet.gameObject.activeSelf)
                {
                    bullet.gameObject.SetActive(true);
                    bullet.GetComponent<TrailRenderer>().Clear();
                    bullet.transform.position = ShotPoint_Automatic.transform.position + 
                                                ((transform.right * Random.Range(-0.3f, 0.3f)) + 
                                                (transform.up * Random.Range(-0.3f, 0.3f)));
                    bullet.transform.rotation = transform.rotation;
                    bullet.FireVector = transform.forward;
                    bullet.ShotSpeed = ShotFlySpeed_Automatic;
                    bullet.Damage = BulletDamage_Automatic;
                    bullet.GetComponent<TrailRenderer>().Clear();
                    local_CooldownTime_Automatic = ShotChargeRate_Automatic;
                    hapticFlash.Execute(0, 0.02f, 100.0f, 50, ShootingHand);
                    break;
                }
            }
            else
            {
                ChargeUpDrawer_Renderer.enabled = false;
                local_CooldownTime_Automatic -= Time.deltaTime;
            }
        }

        #endregion

        #region Camera Controls

        if (LookPoint == 0)
            OffsetObject.transform.localPosition = Vector3.Lerp(OffsetObject.transform.localPosition, new Vector3(0, 1f, -2.8f), 0.1f);
        if (LookPoint == 1)
            OffsetObject.transform.localPosition = Vector3.Lerp(OffsetObject.transform.localPosition, new Vector3(-0.5f, 3f, -4.8f), 0.1f);
        if (LookPoint == 2)
            OffsetObject.transform.localPosition = Vector3.Lerp(OffsetObject.transform.localPosition, new Vector3(-0.5f, 11f, -7f), 0.1f);
        if (LookPoint == 3)
            OffsetObject.transform.localPosition = Vector3.Lerp(OffsetObject.transform.localPosition, new Vector3(-0.5f, 15f, -8.9f), 0.1f);

        bool gripDown = cameraAction.GetState(MoveHand);

        if (!GripToggle)
        {
            if (gripDown)
            {
                GripToggle = true;

                ++LookPoint;

                if (LookPoint > 3) LookPoint = 0;
            }
        }
        else if (!gripDown) GripToggle = false;

        #endregion

        #region Water Bomb

        localWaterBombCooldown += Time.deltaTime;

        if (waterBombAction.GetState(WaterBombHand))
        {
            if (localWaterBombCooldown >= WaterBombCooldown)
            {
                localWaterBombCooldown = 0;
                GameObject bomb = Instantiate(WaterBombObject, transform.position + transform.forward * 1.5f, Quaternion.identity);
                bomb.GetComponent<Rigidbody>().AddForce(transform.forward * 15 + (Vector3.up * 6), ForceMode.VelocityChange);
            }
        }

        #endregion
    }

    public void TakeDamage(float Damage, Vector3 HitPoint, bool IsMushroom = false)
    {
        if(!IsMushroom)
            BloodPool.Splatter(HitPoint, 25, BloodPool.BloodColor.Red);
        
        CurrentHealth -= Mathf.Clamp(Damage - PlayerArmor, 0, 100000);

        #region Health Management

        if (CurrentHealth <= 0)
        {
            //Do death effect here
            print("I'm big dead");
            animator.SetTrigger("Die");
            enabled = false;
            LoseObject.SetActive(true);
            WinObject.SetActive(false);
        }

        #endregion
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f) return 1f;
        else if (dir < 0f) return -1f;
        else return 0f;
    }

    public void EndMySuffering()
    {
        Destroy(gameObject);
    }
}