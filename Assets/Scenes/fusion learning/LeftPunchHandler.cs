using Fusion;
using UnityEngine;

public class LeftPunchHandler : NetworkBehaviour
{
    public enum Type { Melee, Ranged }
    public Type type;

    public int damage;
    public float rate;

    public Collider meleeArea;
    public TrailRenderer trailEffect;

    private bool isPunching = false;
    private float punchTimer = 0f;

    private const float trailEffectStartTime = 0.1f;
    private const float meleeAreaStartTime = 0.222f; // (0.1 + 0.122)
    private const float meleeAreaEndTime = 0.3f;     // (0.1 + 0.122 + 0.0777)
    private const float trailEffectEndTime = 0.422f; // (0.1 + 0.122 + 0.0777 + 0.122)

    public void Use()
    {
        if (type == Type.Melee && !isPunching)
        {
            Debug.Log("called");
            isPunching = true;
            punchTimer = 0f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isPunching)
        {
            punchTimer += Runner.DeltaTime;

            // 0.1초 후에 트레일 효과 활성화
            if (punchTimer >= trailEffectStartTime)
            {
                trailEffect.enabled = true;
            }

            // 0.222초 후에 콜라이더 활성화
            if (punchTimer >= meleeAreaStartTime)
            {
                meleeArea.enabled = true;
            }

            // 0.3초 후에 콜라이더 비활성화
            if (punchTimer >= meleeAreaEndTime)
            {
                meleeArea.enabled = false;
            }

            // 0.422초 후에 트레일 효과 비활성화 및 초기화
            if (punchTimer >= trailEffectEndTime)
            {
                trailEffect.enabled = false;
                isPunching = false; // 공격 상태 초기화
                punchTimer = 0f; // 타이머 초기화
            }
        }
    }
}
