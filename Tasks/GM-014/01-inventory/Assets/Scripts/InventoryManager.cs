using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 1단계: 인벤토리와 필드 아이템 선언
    string[] inventory = new string[5];
    string[] fieldItems = { "물약", "녹슨 검", "금화", "독사과", "방패" };
    int fieldIndex = 0; // 필드에서 몇 번째 아이템을 주울 차례인지

    void Start()
    {
        // 인벤토리 초기화
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = "비어있음";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowInventory();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            PickUp();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UseFirstSlot();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DiscardPoisonApple();
        }
    }

    // 2단계: [I] 인벤토리 확인 (배열 순회)
    public void ShowInventory()
    {
        Debug.Log("=== 인벤토리 상태 ===");
        for (int i = 0; i < inventory.Length; i++)
        {
            Debug.Log($"[{i + 1}번 슬롯] : {inventory[i]}");
        }
    }

    // 3단계: [G] 필드 아이템 획득 (검색 및 수정)
    public void PickUp()
    {
        // 샘플 코드의 순서대로 필드가 비었는지 먼저 본다.
        // 필드 아이템 5개 = 인벤토리 5칸이라, 이 검사가 먼저면 "가방이 가득 찼습니다!"는 나올 수 없다.
        if (fieldIndex >= fieldItems.Length)
        {
            Debug.Log("필드에 더 이상 아이템이 없습니다.");
            return;
        }

        bool isPicked = false;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == "비어있음")
            {
                inventory[i] = fieldItems[fieldIndex];
                // 획득 로그 문구는 명세에 없어 정함
                Debug.Log($"{fieldItems[fieldIndex]}(을)를 주웠습니다! ({i + 1}번 슬롯)");
                fieldIndex++;
                isPicked = true;
                break;
            }
        }

        if (!isPicked)
        {
            Debug.Log("가방이 가득 찼습니다!");
        }
    }

    // 4단계: [U] 1번 슬롯 아이템 사용 (인덱스 접근)
    // 1번 슬롯이 비어 있을 때의 동작은 명세에 없어 아무것도 출력하지 않는다.
    public void UseFirstSlot()
    {
        if (inventory[0] != "비어있음")
        {
            Debug.Log(inventory[0] + "(을)를 사용했습니다!");
            inventory[0] = "비어있음";

            // 심화: 뒤에 있는 아이템을 한 칸씩 앞으로 당긴다
            for (int i = 0; i < inventory.Length - 1; i++)
            {
                inventory[i] = inventory[i + 1];
            }
            inventory[inventory.Length - 1] = "비어있음";
        }
    }

    // 5단계: [S] "독사과" 모두 버리기 (검색)
    // 독사과가 없을 때의 동작은 명세에 없어 아무것도 출력하지 않는다.
    public void DiscardPoisonApple()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == "독사과")
            {
                inventory[i] = "비어있음";
                // 로그 문구는 명세에 없어 정함
                Debug.Log($"독사과를 버렸습니다! ({i + 1}번 슬롯)");
            }
        }
    }
}
