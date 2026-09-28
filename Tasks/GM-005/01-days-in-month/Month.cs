namespace DaysInMonth;

/// <summary>월별 일수 계산. 입출력과 분리해 두어 단위 테스트가 가능하다.</summary>
public static class Month
{
    /// <summary>
    /// 해당 월의 일수를 반환한다. 1~12가 아니면 0을 반환하므로 호출부에서 예외처리한다.
    /// 2월은 과제 조건에 따라 28일 고정 (윤년 계산 없음).
    /// </summary>
    public static int GetDays(int month)
    {
        // if-else 체인보다 switch의 case 묶음이 중복 없이 짧아 선택.
        switch (month)
        {
            case 1:
            case 3:
            case 5:
            case 7:
            case 8:
            case 10:
            case 12:
                return 31;

            case 4:
            case 6:
            case 9:
            case 11:
                return 30;

            case 2:
                return 28;

            default:
                return 0;
        }
    }
}
