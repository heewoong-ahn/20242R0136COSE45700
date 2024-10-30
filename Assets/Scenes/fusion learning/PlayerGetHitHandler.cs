using Fusion;
using UnityEngine;
using System.Collections;

public class PlayerGetHitHandler : NetworkBehaviour
{
    //public Animator anim;
    public LeftPunchHandler leftPunchScript;
    public int health = 55;
    private bool isHit = false; // 이미 맞았는지 확인하는 플래그
    private float hitResetTimer = 0.3f; // 0.3초 유지 //tick에 기반한 네트워크 환경에서의 시간 동기화를 위해 자체적으로 coroutine사용 지양.
    public PlayerTest playerTest;



        public override void FixedUpdateNetwork()
    {
        if (isHit)
        {
            hitResetTimer -= Runner.DeltaTime;

            if (hitResetTimer <= 0)
            {
                playerTest.anim.SetBool("basicPunched 0", false);
                isHit = false; // 상태 초기화
            }
        }
    }

  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melee") && !isHit)
        {
            Debug.Log("attacked");

            LeftPunchHandler leftPunch = other.GetComponent<LeftPunchHandler>();
            ChangeHealth(leftPunch.damage);
            Debug.Log(health);

            isHit = true;


            playerTest.anim.SetBool("basicPunched 0", true);
            hitResetTimer = 0.3f; // 타이머 초기화
        }
    }

    // 체력 변경 로직
    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void ChangeHealth(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            //Die();
        }
    }

}
