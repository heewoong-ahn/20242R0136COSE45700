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

            // 0.1�� �Ŀ� Ʈ���� ȿ�� Ȱ��ȭ
            if (punchTimer >= trailEffectStartTime)
            {
                trailEffect.enabled = true;
            }

            // 0.222�� �Ŀ� �ݶ��̴� Ȱ��ȭ
            if (punchTimer >= meleeAreaStartTime)
            {
                meleeArea.enabled = true;
            }

            // 0.3�� �Ŀ� �ݶ��̴� ��Ȱ��ȭ
            if (punchTimer >= meleeAreaEndTime)
            {
                meleeArea.enabled = false;
            }

            // 0.422�� �Ŀ� Ʈ���� ȿ�� ��Ȱ��ȭ �� �ʱ�ȭ
            if (punchTimer >= trailEffectEndTime)
            {
                trailEffect.enabled = false;
                isPunching = false; // ���� ���� �ʱ�ȭ
                punchTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
    }
}
