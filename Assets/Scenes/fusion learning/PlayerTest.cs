
using UnityEngine;
using Fusion;

public class PlayerTest : NetworkBehaviour//host���� ������ �޾Ƽ� ��. 
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
        //������ �ִ� object�� control�� �� �ְԲ���. �ƴϸ� ��� client�� �����ϰ� ��.
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
                leftPunchScript.Use(); //�ڷ�ƾ ȣ�� ���� boxcollider �ӽ� Ȱ��ȭ.
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
