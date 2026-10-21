using UnityEngine;

// 과열 시스템: Space를 누르고 있으면 히트 증가, 떼면 감소, 80 이상이면 과열(2초 뒤 자동 냉각, 20 이하에서 해제)
public class OverheatPlayer : MonoBehaviour
{
    public enum HeatTrend { Rising, Falling, Holding }

    public float heatUpSpeed = 40f;      // 초당 히트 증가량 (0 → 80까지 2초)
    public float coolDownSpeed = 30f;    // 초당 히트 감소량 (과열 냉각도 같은 속도)
    public float overheatThreshold = 80f;
    public float releaseThreshold = 20f;
    public float overheatDelay = 2f;     // 과열 진입 후 자동 감소가 시작되기까지

    public float maxScaleBonus = 0.5f;   // 히트 100일 때 기본 크기의 1.5배
    public float shakeAngle = 8f;        // 흔들림 회전 각도 (±)
    public float shakeFrequency = 25f;   // 흔들림 속도

    Renderer rend;
    Vector3 baseScale;

    float heat;             // 0 ~ 100
    bool isOverheated;
    float overheatTimer;    // 과열 진입 후 흐른 시간
    float shakeTime;        // 흔들림 계산용 누적 시간
    HeatTrend trend = HeatTrend.Holding;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        baseScale = transform.localScale;
    }

    void Start()
    {
        ApplyVisuals();
    }

    void Update()
    {
        Step(Time.deltaTime, Input.GetKey(KeyCode.Space));
    }

    // 한 프레임 처리: dt만큼 히트를 바꾸고 상태 · 연출을 갱신
    public void Step(float dt, bool spaceHeld)
    {
        float before = heat;

        if (isOverheated)
        {
            // 과열 중엔 Space 무시, 2초가 지나야 자동 감소
            overheatTimer += dt;
            if (overheatTimer >= overheatDelay) heat -= coolDownSpeed * dt;
        }
        else if (spaceHeld)
        {
            heat += heatUpSpeed * dt;
        }
        else
        {
            heat -= coolDownSpeed * dt;
        }

        heat = Mathf.Clamp(heat, 0f, 100f);

        if (!isOverheated && heat >= overheatThreshold)
        {
            isOverheated = true;
            overheatTimer = 0f;
            Debug.Log($"과열! (히트 {heat:0.0}) {overheatDelay}초 동안 입력 무시");
        }
        else if (isOverheated && heat <= releaseThreshold)
        {
            isOverheated = false;
            Debug.Log($"과열 해제 (히트 {heat:0.0})");
        }

        if (heat > before) trend = HeatTrend.Rising;
        else if (heat < before) trend = HeatTrend.Falling;
        else trend = HeatTrend.Holding;

        if (trend == HeatTrend.Rising) shakeTime += dt;
        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        rend.material.color = HeatColor(heat);

        // 크기는 히트에 비례 → 증가 중엔 커지고 감소 중엔 작아진다
        transform.localScale = baseScale * (1f + heat / 100f * maxScaleBonus);

        // 증가 중에만 Z축으로 살짝 흔들림, 그 외엔 똑바로
        float z = trend == HeatTrend.Rising ? Mathf.Sin(shakeTime * shakeFrequency) * shakeAngle : 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, z);
    }

    public static Color HeatColor(float heat)
    {
        if (heat < 40f) return Color.blue;     // 0 ~ 39
        if (heat < 80f) return Color.yellow;   // 40 ~ 79
        return Color.red;                      // 80 ~ 100
    }

    // 확인용: 현재 상태 조회
    public float Heat => heat;
    public bool IsOverheated => isOverheated;
    public float OverheatTimer => overheatTimer;
    public HeatTrend Trend => trend;
}
