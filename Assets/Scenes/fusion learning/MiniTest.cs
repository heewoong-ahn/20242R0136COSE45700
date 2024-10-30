using UnityEngine;
using Fusion;
using System.Collections;

public class MiniTest : NetworkBehaviour
{
    [Networked] public int maxHealth { get; set; }
    [Networked] public int curHealth { get; set; }
    [Networked] public float lastDodge { get; set; }
    [Networked] public NetworkBool gFlag { get; set; }
    [Networked] public NetworkBool hitByUpperCut { get; set; }
    [Networked] public NetworkBool isEnd { get; set; }

    [Networked] private bool isDodging { get; set; }

    private Animator anim;
    private NetworkMecanimAnimator networkAnim;
    private Rigidbody rigid;
    private BoxCollider boxCollider;
    private NetworkTransform networkTransform; 

    private float timer = 4f;
    public float speed = 5f;

    private float lastPressTimeForward = 0f;
    private float lastPressTimeBack = 0f;
    private int pressCountForward = 0; // 키 눌림 횟수
    private float pressCountBack = 0;
    private float doublePressInterval = 0.2f; // 회피기 작동 시간

    private float lastBasicPunchTime;
    private float basicPunchTime = 0.7778f;
    public leftPunch leftPunchScript;

        

    private void Awake()
    {
        anim = GetComponent<Animator>();
        networkAnim = GetComponent<NetworkMecanimAnimator>();
        networkTransform = GetComponent<NetworkTransform>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public override void Spawned()
    {
        maxHealth = 55;
        curHealth = maxHealth;
        isEnd = false;
        gFlag = false;
        hitByUpperCut = false;
        lastDodge = -1f;
        lastBasicPunchTime = -basicPunchTime;
    }

    public override void FixedUpdateNetwork()
    {
        timer -= Runner.DeltaTime;

        if (GetInput(out NetworkInputData data))
        {
            //host만 input과 전체 상태 제어를 할 수 있음. 
            if (Object.HasInputAuthority && Object.HasStateAuthority)
            {
                ProcessInput(data);
            }
            //client는 input만 넣을 수 있음. 
            else if (Object.HasInputAuthority && !Object.HasStateAuthority)
            {

            }
          
        }
    }

    private void ProcessInput(NetworkInputData data)
    {
        if ((data.buttons & NetworkInputData.BUTTON_FORWARD) != 0)
        {
            MoveForward();

            
        }
        else if ((data.buttons & NetworkInputData.BUTTON_BACKWARD) != 0)
        {
            MoveBackward();
        }
        else
        {
            anim.SetBool("stepForward", false);
            anim.SetBool("stepBackward", false);
        }

        if ((data.buttons & NetworkInputData.BUTTON_DODGE_FRONT) != 0)
        {
            if (!NoDodge() && Runner.SimulationTime-lastDodge >1) //dodge 쿨타임 1초.
            {
                lastDodge = Runner.SimulationTime;
                DodgeMove(0.2f, true);
            }
        }

        if ((data.buttons & NetworkInputData.BUTTON_DODGE_BACK) != 0)
        {
            if (!NoDodge() && Runner.SimulationTime - lastDodge > 1) //dodge 쿨타임 1초.
            {
                lastDodge = Runner.SimulationTime;
                DodgeMove(0.2f, false);
            }
        }

        if ((data.buttons & NetworkInputData.BUTTON_ATTACK_T) != 0)
        {
            if (Runner.SimulationTime - lastBasicPunchTime >= basicPunchTime && CanActivate())
            {
                leftPunchScript.Use();
                anim.SetTrigger("basicPunch");
                lastBasicPunchTime = Runner.SimulationTime;
            }
        }
    }

    private void MoveForward()
    {
        
        if (timer < 0 && CanActivate()) { 

            Vector3 moveVec = new Vector3(0, 0, 1);
            transform.position += moveVec * speed * Runner.DeltaTime;

            anim.SetBool("stepForward", true);
            anim.SetBool("stepBackward", false);
        }
    }

    private void MoveBackward()
    {
        if (timer<0 && CanActivate())
        {
            Vector3 moveVec = new Vector3(0, 0, -1);
            transform.position += moveVec * speed * Runner.DeltaTime;

            anim.SetBool("stepForward", false);
            anim.SetBool("stepBackward", true);
        }
    }

    private void DodgeMove(float duration, bool forward)
    {
        if (timer < 0 && !IsBasicPunchAnimationPlaying()) {
            if (Object.HasStateAuthority)
            {
                Vector3 dodgeDirection = new Vector3(0, 0, 2); // 회피 방향 설정

                if (!forward)
                {
                    dodgeDirection *= (-1);
                }
               
                Vector3 startPosition = transform.position;
                Vector3 targetPosition = startPosition + dodgeDirection;

                Runner.StartCoroutine(DodgeCoroutine(duration, targetPosition));
            }
        }
        
    }

    //동작을 부드럽게 처리. 
    private IEnumerator DodgeCoroutine(float duration, Vector3 targetPosition)
    {
        float endTime = Runner.SimulationTime + duration;
        while (Runner.SimulationTime < endTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Runner.DeltaTime * 5);
            yield return null;
        }
    }

    bool IsBasicPunchAnimationPlaying()
    {
        // "Base Layer" is the name of basic layer of Animation Controller 
        int BaseLayerIndex = anim.GetLayerIndex("Base Layer");

        // check whether basicPunch animation is playing or not. 
        return anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("basicPunch");
    }

    bool IsBasicPunchedAnimationPlaying()
    {
        int BaseLayerIndex = anim.GetLayerIndex("Base Layer");

        return anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("basicPunched");
    }

    private bool CanActivate()
    {
        return !IsBasicPunchAnimationPlaying() && !IsBasicPunchedAnimationPlaying();
    }

    bool NoDodge()
    {
        int BaseLayerIndex = anim.GetLayerIndex("Base Layer");
        return (anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("rightKick")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("leftKick")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("underKick")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("basicPunch")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("kicked")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("underKicked")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("basicPunched")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("guard")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("rightPunch")
                || anim.GetCurrentAnimatorStateInfo(BaseLayerIndex).IsName("upperCut"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            leftPunch leftPunch = other.GetComponent<leftPunch>();
            anim.SetTrigger("basicPunched");
            // ChangeHealth(-leftPunch.damage);
            //StartCoroutine(ActivateParticleAndLight(particle, light, 0.3f));
        }
    }
}