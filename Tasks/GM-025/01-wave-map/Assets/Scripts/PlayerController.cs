using UnityEngine;

// WASD 이동(CharacterController, NavMesh와 무관하게 룸 사이를 걸어 다닌다) + Space 근접 공격.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public int maxHp = 100;
    public float moveSpeed = 6f;
    public int attackDamage = 1;
    public float attackCooldown = 0.3f;
    public float attackReach = 1.2f;   // 공격 판정 구의 중심을 앞쪽으로 얼마나 둘지
    public float attackRadius = 1.3f;
    public Renderer body;

    public int Hp { get; private set; }

    // 배치 모드 확인용: 키 입력 대신 이 값으로 움직인다
    [HideInInspector] public bool useBotInput;
    [HideInInspector] public Vector3 botMove;

    CharacterController controller;
    float cooldown;
    float flash;
    Color baseColor;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Hp = maxHp;
        baseColor = body.material.color;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        flash -= Time.deltaTime;
        body.material.color = flash > 0f ? Color.red : baseColor;
        if (!GameManager.Instance.IsPlaying) return;

        Vector3 move = useBotInput ? botMove : new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Move(move);
        if (!useBotInput && Input.GetKeyDown(KeyCode.Space)) Attack();
    }

    void Move(Vector3 dir)
    {
        dir.y = 0f;
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        if (dir.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(dir);
        controller.Move((dir * moveSpeed + Vector3.down * 9.8f) * Time.deltaTime);
    }

    // 앞쪽 구 안의 몬스터 모두에게 피해. 맞힌 수를 돌려준다(쿨다운 중 · 게임 끝이면 -1)
    public int Attack()
    {
        if (cooldown > 0f || !GameManager.Instance.IsPlaying) return -1;
        cooldown = attackCooldown;

        int hits = 0;
        Vector3 center = transform.position + transform.forward * attackReach;
        foreach (Collider c in Physics.OverlapSphere(center, attackRadius))
        {
            Monster m = c.GetComponentInParent<Monster>();
            if (m == null || m.IsDead) continue;
            m.TakeDamage(attackDamage);
            hits++;
        }
        return hits;
    }

    public void TakeDamage(int damage, Monster from)
    {
        if (Hp <= 0) return;
        Hp = Mathf.Max(Hp - damage, 0);
        flash = 0.15f;
        Debug.Log($"플레이어 피격 -{damage} ({from.name}) → HP {Hp}/{maxHp}");
        if (Hp == 0) GameManager.Instance.GameOver();
    }
}
