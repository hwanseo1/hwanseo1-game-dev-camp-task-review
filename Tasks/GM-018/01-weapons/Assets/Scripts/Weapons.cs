// 기본 문제 1단계: 데이터 설계 (Struct vs Class)

// 값 타입: 대입하면 필드 전체가 복사된다.
public struct NormalWeapon
{
    public string name;
    public int power;
}

// 참조 타입: 대입하면 Heap에 있는 같은 객체를 가리키는 주소만 복사된다.
public class LegendaryWeapon
{
    public string name;
    public int power;
}
