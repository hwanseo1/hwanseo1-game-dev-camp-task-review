using System;
using System.Collections.Generic;
using System.Globalization;

namespace WolnamPpong;

public enum Suit { Spade, Diamond, Club, Heart }

/// <summary>판정 결과. Pair는 앞 두 장의 숫자가 같아 배팅 없이 지는 경우.</summary>
public enum Result { Win, Lose, Pair }

public struct Card
{
    public Suit Suit;
    public int Number;   // 1 ~ 13

    public Card(Suit suit, int number)
    {
        Suit = suit;
        Number = number;
    }

    public override string ToString() => Game.SuitSymbol(Suit) + " " + Game.RankText(Number);
}

/// <summary>월남뽕 규칙과 게임 상태. 입출력과 분리해 두어 단위 테스트가 가능하다.</summary>
public class Game
{
    public const int StartMoney    = 10000;
    public const int MinBet        = 1000;
    public const int PairPenalty   = 1000;   // 앞 두 장이 같은 숫자일 때 무조건 차감
    public const int WinMultiplier = 2;      // 이기면 배팅금의 2배를 얻는다
    public const int DeckSize      = 52;
    public const int CardsPerRound = 3;
    public const int MaxRounds     = DeckSize / CardsPerRound;   // 17판, 1장은 버려진다

    static readonly string[] SuitSymbols = { "♠", "♦", "♣", "♥" };

    public static string SuitSymbol(Suit suit) => SuitSymbols[(int)suit];

    /// <summary>출력 변환: 1 → A, 11 → J, 12 → Q, 13 → K. 나머지는 숫자 그대로.</summary>
    public static string RankText(int number)
    {
        switch (number)
        {
            case 1:  return "A";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
            default: return number.ToString();
        }
    }

    public static string Won(int amount) => amount.ToString("N0", CultureInfo.InvariantCulture) + "원";

    /// <summary>문양 4종 x 1~13 = 52장, 정렬된 상태.</summary>
    public static List<Card> NewDeck()
    {
        List<Card> deck = new List<Card>(DeckSize);
        for (int s = 0; s < SuitSymbols.Length; s++)
            for (int n = 1; n <= 13; n++)
                deck.Add(new Card((Suit)s, n));
        return deck;
    }

    /// <summary>Fisher-Yates 셔플.</summary>
    public static void Shuffle(List<Card> deck, Random rng)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            Card tmp = deck[i];
            deck[i] = deck[j];
            deck[j] = tmp;
        }
    }

    /// <summary>
    /// 세 번째 카드가 앞 두 장 숫자 사이(초과 ~ 미만)에 있으면 승리. 문양은 보지 않는다.
    /// 앞 두 장의 순서는 상관없다 (명세 예시는 작은 수가 앞이지만, 큰 수가 앞에 오는 경우는 정의가 없다).
    /// </summary>
    public static Result Judge(Card first, Card second, Card third)
    {
        if (first.Number == second.Number) return Result.Pair;

        int low  = Math.Min(first.Number, second.Number);
        int high = Math.Max(first.Number, second.Number);
        return third.Number > low && third.Number < high ? Result.Win : Result.Lose;
    }

    /// <summary>
    /// 소지금 변화량. 승리 +배팅금 x2 / 패배 -배팅금 / 같은 숫자 -1,000.
    /// 소지금이 1,000원 미만일 때 같은 숫자가 나오면 남은 소지금만 차감한다 (음수 방지).
    /// </summary>
    public static int Delta(Result result, int bet, int money)
    {
        switch (result)
        {
            case Result.Win:  return bet * WinMultiplier;
            case Result.Lose: return -bet;
            default:          return -Math.Min(PairPenalty, money);
        }
    }

    /// <summary>
    /// 이번 판 최소 배팅. 소지금이 1,000원 미만이면 명세대로는 배팅할 수 없는데 0원이 아니라 종료도 안 된다.
    /// 그래서 남은 소지금 전액을 최소 배팅으로 정했다.
    /// </summary>
    public static int MinBetFor(int money) => Math.Min(MinBet, money);

    public static bool IsValidBet(int bet, int money) => money > 0 && bet >= MinBetFor(money) && bet <= money;

    // ── 상태 ──

    readonly List<Card> deck;

    public int Money { get; private set; } = StartMoney;
    public int Round { get; private set; }          // 지금까지 나눈 판 수 (진행 중인 판 포함)
    public Card First { get; private set; }
    public Card Hidden { get; private set; }        // 가운데 ? 카드 (명세상 "세 번째 카드")
    public Card Second { get; private set; }

    public Game(Random rng)
    {
        deck = NewDeck();
        Shuffle(deck, rng);
    }

    /// <summary>테스트용: 카드 순서를 직접 정한다.</summary>
    public Game(List<Card> deck, int money = StartMoney)
    {
        if (deck.Count != DeckSize) throw new ArgumentException("덱은 52장이어야 한다.");
        this.deck = new List<Card>(deck);
        Money = money;
    }

    public bool IsBankrupt => Money <= 0;
    public bool IsOutOfCards => Round >= MaxRounds;
    public bool CanDeal => !IsBankrupt && !IsOutOfCards;
    public bool IsPair => First.Number == Second.Number;
    public int RemainingCount => DeckSize - Round * CardsPerRound;

    /// <summary>
    /// 덱 위에서 3장을 꺼낸다. 첫째·둘째 장은 앞·뒤 자리에 펼치고, 셋째 장은 가운데 ?에 엎어 둔다.
    /// 명세 출력 예시(♥ A / ? / ♣ K)에서 "세 번째 카드"(판정 대상)가 가운데 ? 자리에 있기 때문이다.
    /// </summary>
    public void Deal()
    {
        if (!CanDeal) throw new InvalidOperationException("더 이상 나눌 수 없다.");
        int top = Round * CardsPerRound;
        First  = deck[top];
        Second = deck[top + 1];
        Hidden = deck[top + 2];
        Round++;
    }

    /// <summary>배팅하고 판정·정산한다. 같은 숫자 판에서는 bet을 보지 않는다.</summary>
    public Result Play(int bet, out int delta)
    {
        Result result = Judge(First, Second, Hidden);
        if (result != Result.Pair && !IsValidBet(bet, Money))
            throw new ArgumentOutOfRangeException(nameof(bet));

        delta = Delta(result, bet, Money);
        Money += delta;
        return result;
    }

    // 폴드는 명세에 "해당 판 종료"만 있고 비용이 없다. 소지금은 그대로, 카드는 버려진다(Round는 이미 증가).

    // ── 치트 ──

    /// <summary>치트 1: 다음 판의 ? 카드. 마지막 판이면 없다.</summary>
    public bool TryPeekNextHidden(out Card card)
    {
        card = default;
        if (IsOutOfCards) return false;
        card = deck[Round * CardsPerRound + 2];
        return true;
    }

    /// <summary>치트 2: 아직 나오지 않은 카드 전체 (이번 판 3장 제외). 문양 → 숫자 순으로 정렬.</summary>
    public List<Card> Remaining()
    {
        List<Card> rest = deck.GetRange(Round * CardsPerRound, RemainingCount);
        rest.Sort((a, b) => a.Suit != b.Suit ? a.Suit.CompareTo(b.Suit) : a.Number.CompareTo(b.Number));
        return rest;
    }

    /// <summary>17판을 모두 마친 뒤 버려지는 마지막 1장.</summary>
    public Card Leftover => deck[DeckSize - 1];
}
