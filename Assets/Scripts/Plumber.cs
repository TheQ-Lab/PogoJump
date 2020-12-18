using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plumber : MonoBehaviour
{
    [Header("Aktuelle Leben/Startleben")]
    [Tooltip("Range: 0-5")]
    public int Lives = 3;

    [Header("Stärke-Konstanten")]
    [Tooltip("Hart unnötig zu verändern, Bro - Internal, access from PlumberInput")]
    public float MaxPower = 500f;
    [Tooltip("Stärke des voll aufgeladenen Pfeils, unter normalen Umständen")]
    public float MaxPowerStandard = 780f;
    [Tooltip("Stärke des voll aufgeladenen Pfeils, wenn PowerUp intus")]
    public float PoweredUpPower = 1030f;

    [Header("Referenzen")]
    private Rigidbody2D rBody;
    private PlumberInput plumberInput;
    private AudioHandler audioHandler;
    private MusicHandler bgmHandler;
    private Animator animator;
    private Transform rightArm, leftArm;
    private UiLives livesIcons;

    [Header("Internals, public for ease of access from other scrips or debugging")]
    [Tooltip("<used by PlumberInput>")]
    public bool attached = false;
    private char attachedSide = 'l';

    
    private float lastTimeInCollisionStay = 0f, startTimeCollision = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        plumberInput = GetComponent<PlumberInput>();
        audioHandler = GetComponentInChildren<AudioHandler>();
        bgmHandler = GetComponentInChildren<MusicHandler>();
        animator = GetComponent<Animator>();

        rightArm = transform.Find("ArmRight"); //transform.Find for children
        leftArm = transform.Find("ArmLeft");

        livesIcons = GameObject.Find("Lives").GetComponent<UiLives>(); //transform.Find for entire scene, maybe even multiple scenes :/ https://docs.unity3d.com/ScriptReference/GameObject.Find.html
        livesIcons.SetLives(Lives);
        MaxPower = MaxPowerStandard;
    }

    // Update is called once per frame
    void Update()
    {

        TimeRoutines();
        if (!GameManager.Instance.IsGameplayActive)
        {
            //rBody.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }
    }

    private float timerLastHit = 0, timerPowerUp = 0;
    private void TimeRoutines()
    {
        float delta = Time.deltaTime;
        timerLastHit -= delta;
        if (timerPowerUp>0f)
            timerPowerUp -= delta;

        animator.SetFloat("TimerInvincible", timerLastHit);
        if(timerPowerUp < 0)
        {
            plumberInput.LockLaunch();
            MaxPower = MaxPowerStandard;
            plumberInput.SetMaxLaunchArrow(MaxPower, UiArrow.ArrowColors.standard);
            timerPowerUp = 0f;
            audioHandler.SetAndPlay("PowerDown");
        }
    }

    private void FixedUpdate()
    {
        if (Lives <= 0) { 
            GameManager.Instance.GameOver();
            audioHandler.SetAndPlay("GameOver");
            bgmHandler.SetAndPlay(MusicHandler.BGMType.GameOver);
        }
        wiggleCollider();

    }

    private void wiggleCollider()
    {
        //Wiggle impercievably to keep Collider2D awake
        int currentFixedFrame = Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
        if(currentFixedFrame%2 == 0)
        {
            rBody.rotation += 0.004f;
        } else
        {
            rBody.rotation -= 0.004f;
        }
    }


    private void orientArmsTowards(Vector2 collisionPoint)
    {
        if (collisionPoint.x >= this.transform.position.x)
        {
            // it collides on the Right side of plumber

            Vector2 stretchRArmCollision = collisionPoint - (Vector2)rightArm.position;
            float angle = Vector2.SignedAngle(Vector2.up, stretchRArmCollision);
            //Debug.Log(angle);
            angle -= 8; //accounting for the angle difference from straight up
            //Debug.Log("Angle RIGHT: " + angle);
            angle = Mathf.Clamp(angle, -130, -8);
            rightArm.rotation = Quaternion.Euler(new Vector3(0,0,angle));

            attachedSide = 'r';
            animator.SetBool("FacingLeft", true);
        } 
        else
        {
            //Left Side Collision

            Vector2 stretchLArmCollision = collisionPoint - (Vector2)leftArm.position;
            float angle = Vector2.SignedAngle(Vector2.up, stretchLArmCollision);
            //Debug.Log(angle);
            angle += 8; //accounting for the angle difference from straight up
            //Debug.Log("Angle LEFT: " + angle);
            angle = Mathf.Clamp(angle, 8, 130);
            leftArm.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            attachedSide = 'l';
            animator.SetBool("FacingLeft", false);
        }
    }

    const int standardArmsUpAngle = 8, maxArmsAngle = 130;
    public void orientArm(float launchAngle)
    {
        if (attachedSide == 'l')
        {
            float angle = launchAngle - 8; //accounting for the angle difference from straight up
            angle = Mathf.Clamp(angle, -130, -8);
            rightArm.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else if (attachedSide == 'r')
        {
            float angle = launchAngle + 8; //accounting for the angle difference from straight up
            angle = Mathf.Clamp(angle, 8, 130);
            leftArm.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void LaunchCharacter(float angle, float power)
    {
        //Debug.Log("Jump");
        power = CoolFunctions.Remap(power, 0, MaxPower, 280, MaxPower);
        Vector2 directionalVector = Quaternion.Euler(0, 0, angle) * Vector2.up;
        Vector2 launchVector = directionalVector * power;
        rBody.constraints = RigidbodyConstraints2D.FreezeRotation; //only Rotation frozen
        rBody.AddForce(launchVector);
        audioHandler.SetAndPlay("Launch");
        attached = false;
        animator.SetBool("IsJumping", true);
        animator.SetTrigger("LaunchSpring");
        if (directionalVector.x < 0)
            animator.SetBool("FacingLeft", true);
        else if (directionalVector.x > 0)
            animator.SetBool("FacingLeft", false);
    }

    public void DamagePlumber(int damage)
    {
        Lives = Mathf.Clamp(Lives - damage, 0, livesIcons.maxLives);
        livesIcons.SetLives(Lives);
        if (damage > 0)
        {
            timerLastHit = 2.5f;
            audioHandler.SetAndPlay("Damage");
            if (Lives == 1)
                bgmHandler.SetAndPlay(MusicHandler.BGMType.Danger);
        } 
        else if(damage < 0)
        {
            audioHandler.SetAndPlay("ExtraLife");
            if (bgmHandler.currentlyPlaying.bGMType == MusicHandler.BGMType.Danger)
                bgmHandler.SetAndPlay(MusicHandler.BGMType.Cave);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameManager.Instance.IsGameplayActive) return;

        if (collision.transform.name == "SlimeCollider")
        {
            // Slime Obstacle Event here
            attached = true;
            animator.SetBool("IsJumping", false);
            //Debug.LogError("SlimeCollision");
            rBody.velocity = Vector2.zero;
            orientArmsTowards(collision.contacts[0].point);
            audioHandler.SetAndPlay("Land");
        }
        else if (collision.transform.name == "WallLevel")
        {
            // Hitting a Wall
            attached = true;
            animator.SetBool("IsJumping", false);
            rBody.constraints = RigidbodyConstraints2D.FreezeAll;
            rBody.velocity = new Vector2(0f, 0.1962f);//exakt ausgleichsgeschwindigkeit zu Gravity
            orientArmsTowards(collision.contacts[0].point);
            audioHandler.SetAndPlay("Land");
        }
        
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //IF OnCollisionStay2D() ever not called any more, even though it sould, constantly wiggle xPos or zRot a bit.
        // For resetting attatched when staying at wall
        //Debug.Log("OCS");
        if (!attached)
        {
            float currentTime = Time.fixedTime;
            //Debug.Log(currentTime);
            if (lastTimeInCollisionStay+0.1f<currentTime)
            {
                //Debug.LogWarning("Start" + lastTimeInCollisionStay + ", " + currentTime);
                startTimeCollision = currentTime;
                lastTimeInCollisionStay = currentTime;
            } else
            {
                lastTimeInCollisionStay = currentTime;
                //Debug.Log(currentTime + "   " + startTimeCollision);
                if(startTimeCollision + 1f < currentTime)
                {
                    Debug.Log("RESET attached");
                    startTimeCollision = currentTime;
                    attached = true;
                    animator.SetBool("IsJumping", false);
                }
            }
        }

        if(collision.collider.name == "SlimeCollider" && attached)
        {
            rBody.velocity = new Vector2(0f, -0.3f);
        }
        else if (collision.transform.name == "WallLevel" && attached)
        {
            //Debug.Log("colliding wall");
            rBody.velocity = new Vector2(0f,0.1962f);//exakt ausgleichsgeschwindigkeit zu Gravity
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.name == "SlimeCollider")
        {
            attached = false;
            plumberInput.LockLaunch();
        }
        /*
        else if (collision.collider.name == "WallLevel")
        {
            attached = false;
            plumberInput.LockLaunch();
        }*/
    }


    // ---------------TRIGGER--------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsGameplayActive) return;

        if (collision.transform.name == "ExtraLifeCollider")
        {
            DamagePlumber(-1);
            collision.transform.parent.gameObject.SetActive(false);
        }
        else if (collision.transform.name == "PowerUpCollider")
        {
            plumberInput.LockLaunch();
            MaxPower = PoweredUpPower;
            plumberInput.SetMaxLaunchArrow(MaxPower, UiArrow.ArrowColors.poweredUp);
            timerPowerUp = 10f;
            collision.transform.parent.gameObject.SetActive(false);
            audioHandler.SetAndPlay("PowerUp");
        }
        else if (collision.transform.name == "MushroomCollider")
        {
            rBody.velocity = Vector2.zero;
            Vector2 path = collision.transform.parent.Find("LaunchDirection").transform.localPosition * 150;
            path = collision.transform.parent.rotation * path;
            rBody.AddForce(path);
            audioHandler.SetAndPlay("Launch");
            animator.SetBool("IsJumping", true);
            animator.SetTrigger("LaunchSpring");
        }

        if (timerLastHit>0)
            return;

        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsGameplayActive) return;

        if (timerLastHit > 0)
            return;
        /*
        if (collision.transform.name == "Acid")
        {
            rBody.velocity = Vector2.zero;
            float bounceAngle = Random.Range(-30f, 30f);
            LaunchCharacter(bounceAngle, MaxPower);
            DamagePlumber(1);
        }*/

        if (collision.transform.name == "Acid" || collision.transform.name == "DeathPlain")
        {
            rBody.velocity = Vector2.zero;
            float bounceAngle = Random.Range(-30f, 30f);
            LaunchCharacter(bounceAngle, MaxPower);
            DamagePlumber(1);
        }
        else if (collision.transform.name == "FuzzyCollider")
        {
            rBody.velocity = Vector2.zero;

            Vector2 posFuzzy = collision.transform.position;

            float bounceAngle = 0f;
            if (this.transform.position.x <= posFuzzy.x)
                bounceAngle = 150f;
            else
                bounceAngle = -150f;
            LaunchCharacter(180f - bounceAngle, MaxPower);
            DamagePlumber(1);
        }
        /*else if (collision.transform.name == "DeathPlain")
        {
            //replaced up there with 1-Damage Acid-Logic
            DamagePlumber(3);
        }*/
        else if (collision.transform.name == "MaskCollider")
        {
            rBody.velocity = Vector2.zero;
            Vector2 posFuzzy = collision.transform.position;
            float bounceAngle = 0f;
            if (this.transform.position.x <= posFuzzy.x)
                bounceAngle = +150f;
            else
                bounceAngle = -150f;
            LaunchCharacter(180f - bounceAngle, MaxPower);
            DamagePlumber(1);
        }
    }
}
