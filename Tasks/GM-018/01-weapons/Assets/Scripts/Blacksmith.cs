using System;
using UnityEngine;

public class Blacksmith : MonoBehaviour
{
    // W에서 만든 전설 무기를 E에서 다시 쓰므로 필드로 둔다.
    private LegendaryWeapon original;
    private LegendaryWeapon linked;

    // E에서 "Heap의 실체가 남아 있는지"를 확인하기 위한 약한 참조.
    // 약한 참조는 GC가 객체를 치우는 것을 막지 않는다.
    private WeakReference heapTracker;

    void Update()
    {
        // [Q] 일반 무기 복사 테스트
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestStructCopy();
        }

        // [W] 전설 무기 공유 테스트
        if (Input.GetKeyDown(KeyCode.W))
        {
            TestClassShare();
        }

        // [E] 메모리 청소 시뮬레이션
        if (Input.GetKeyDown(KeyCode.E))
        {
            ReleaseLinked();
        }
    }

    // 2단계 [Q]
    public void TestStructCopy()
    {
        NormalWeapon original = new NormalWeapon();
        original.name = "강철 검";
        original.power = 10;

        NormalWeapon copy = original;
        copy.power = 50;

        Debug.Log($"[Q 일반 무기] 원본 {original.name}: 공격력 {original.power} / 복사본 {copy.name}: 공격력 {copy.power}");
        Debug.Log(original.power == 10
            ? "[Q 일반 무기] 원본은 그대로입니다. (값 복사 → 서로 다른 무기)"
            : "[Q 일반 무기] 원본이 바뀌었습니다!");
    }

    // 3단계 [W]
    public void TestClassShare()
    {
        original = new LegendaryWeapon();
        original.name = "엑스칼리버";
        original.power = 100;

        linked = original;
        linked.power = 999;

        heapTracker = new WeakReference(original);

        Debug.Log($"[W 전설 무기] 원본 {original.name}: 공격력 {original.power} / 연결 {linked.name}: 공격력 {linked.power}");
        Debug.Log(original.power == 999
            ? "[W 전설 무기] 둘 다 바뀌었습니다. (참조 복사 → 같은 무기를 가리킴)"
            : "[W 전설 무기] 원본은 그대로입니다!");
    }

    // 4단계 [E]
    public void ReleaseLinked()
    {
        if (heapTracker == null)
        {
            // W 전에 E를 누르면 정리할 연결이 없다. 문구는 명세에 없어 정함
            Debug.Log("[E 메모리] 아직 전설 무기가 없습니다. W를 먼저 누르세요.");
            return;
        }

        linked = null;
        Debug.Log($"[E 메모리] linked = null 실행 → linked {(linked == null ? "null" : "살아 있음")}");

        // 강제로 GC를 돌려도 original이 아직 가리키고 있으면 Heap의 객체는 치워지지 않는다.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Debug.Log($"[E 메모리] GC 후 Heap의 전설 무기: {(heapTracker.IsAlive ? "남아 있음" : "사라짐")}");

        if (original != null)
        {
            Debug.Log($"[E 메모리] original로 여전히 접근 가능: {original.name} 공격력 {original.power}");
        }
    }
}
