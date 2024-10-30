using Fusion;
using UnityEngine;
using System.Collections;

public class PlayerGetHitHandler : NetworkBehaviour
{
    //public Animator anim;
    public LeftPunchHandler leftPunchScript;
    public int health = 55;
    private bool isHit = false; // �̹� �¾Ҵ��� Ȯ���ϴ� �÷���
    private float hitResetTimer = 0.3f; // 0.3�� ���� //tick�� ����� ��Ʈ��ũ ȯ�濡���� �ð� ����ȭ�� ���� ��ü������ coroutine��� ����.
    public PlayerTest playerTest;



        public override void FixedUpdateNetwork()
    {
        if (isHit)
        {
            hitResetTimer -= Runner.DeltaTime;

            if (hitResetTimer <= 0)
            {
                playerTest.anim.SetBool("basicPunched 0", false);
                isHit = false; // ���� �ʱ�ȭ
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
            hitResetTimer = 0.3f; // Ÿ�̸� �ʱ�ȭ
        }
    }

    // ü�� ���� ����
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
