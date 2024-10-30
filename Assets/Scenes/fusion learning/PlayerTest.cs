
using UnityEngine;
using Fusion;

public class PlayerTest : NetworkBehaviour//host에서 정보를 받아서 씀. 
{
    private NetworkCharacterControllerCustom characterControllerCustom;
    public LeftPunchHandler leftPunchScript;
    // private NetworkMecanimAnimator networkMecanimAnimator;
    public Animator anim;
    public static PlayerTest Local { get; set; }

    private void Awake()

    {
        characterControllerCustom = GetComponent<NetworkCharacterControllerCustom>();
        //networkMecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        //anim = GetComponent<Animator>();
    }

    /*public override void Spawned()
    {
        //권한이 있는 object만 control할 수 있게끔함. 아니면 모든 client를 조종하게 됨.
       if (Object.HasInputAuthority)
        {
            Local = this;
        }
    }*/

    public override void FixedUpdateNetwork()
    {
        //every tick
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            if (data.direction.z > 0)
            {
                anim.SetBool("stepForward", true);
                anim.SetBool("stepBackward", false);

            }
            else if (data.direction.z < 0)
            {
                anim.SetBool("stepForward", false);
                anim.SetBool("stepBackward", true);
            }

            else if (data.direction.z == 0)
            {
                anim.SetBool("stepForward", false);
                anim.SetBool("stepBackward", false);
            }
            characterControllerCustom.Move(data.direction*10*Runner.DeltaTime);

            //anim.SetBool("stepForward", false);
            //anim.SetBool("stepBackward", false);

            if (data.leftPunch)
            {
                Debug.Log("calling");
                leftPunchScript.Use(); //코루틴 호출 통한 boxcollider 임시 활성화.
                anim.SetBool("basicPunch 0", true);
                data.leftPunch = false; 
            }
            else if (!data.leftPunch)
            {
                anim.SetBool("basicPunch 0", false);
            }
        }
    }
}
