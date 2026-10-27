using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 스킬 쿨타임: Space로 스킬 사용 → 색 변경 + 5초 쿨타임(코루틴), C로 쿨타임 취소, 게이지 UI · 초 단위 로그
public class SkillCooldown : MonoBehaviour
{
    public float cooldownTime = 5f;
    public Color readyColor = Color.white;                      // 스킬 사용 가능
    public Color skillColor = new Color(0.2f, 0.45f, 1f);       // 스킬 사용 직후 ~ 쿨타임 끝

    public Image gaugeFill;   // Image Type = Filled, 비었다가 차오르고 가득 차면 사용 가능
    public Text gaugeLabel;

    Renderer rend;
    Coroutine cooldownRoutine;
    float remaining;          // 남은 쿨타임 (초)

    public bool IsOnCooldown => cooldownRoutine != null;
    public float Remaining => remaining;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        rend.material.color = readyColor;
        UpdateGauge();
        Debug.Log("스킬 사용 가능 (Space: 스킬, C: 쿨타임 취소)");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TryUseSkill();
        if (Input.GetKeyDown(KeyCode.C)) CancelCooldown();
    }

    public bool TryUseSkill()
    {
        if (IsOnCooldown)
        {
            Debug.Log($"쿨타임 중이라 스킬 사용 불가 ({remaining:0.0}초 남음)");
            return false;
        }

        rend.material.color = skillColor;
        Debug.Log("스킬 사용!");
        cooldownRoutine = StartCoroutine(CooldownRoutine());
        return true;
    }

    public void CancelCooldown()
    {
        if (!IsOnCooldown)
        {
            Debug.Log("취소할 쿨타임 없음 (이미 사용 가능)");
            return;
        }

        StopCoroutine(cooldownRoutine);
        Debug.Log($"쿨타임 취소 ({remaining:0.0}초 남아 있었음)");
        EndCooldown();
    }

    IEnumerator CooldownRoutine()
    {
        remaining = cooldownTime;
        int lastLogged = Mathf.CeilToInt(remaining);
        Debug.Log($"쿨타임 {lastLogged}초 남음");
        UpdateGauge();

        // 매 프레임 줄여서 게이지는 부드럽게, 로그는 정수 초가 바뀔 때만
        while (remaining > 0f)
        {
            yield return null;
            remaining -= Time.deltaTime;

            int sec = Mathf.CeilToInt(remaining);
            if (sec < lastLogged && sec > 0)
            {
                lastLogged = sec;
                Debug.Log($"쿨타임 {sec}초 남음");
            }
            UpdateGauge();
        }

        EndCooldown();
    }

    void EndCooldown()
    {
        cooldownRoutine = null;
        remaining = 0f;
        rend.material.color = readyColor;
        UpdateGauge();
        Debug.Log("스킬 사용 가능");
    }

    void UpdateGauge()
    {
        if (gaugeFill != null) gaugeFill.fillAmount = 1f - remaining / cooldownTime;
        if (gaugeLabel != null) gaugeLabel.text = IsOnCooldown ? $"쿨타임 {remaining:0.0}초" : "스킬 사용 가능 (Space)";
    }
}
