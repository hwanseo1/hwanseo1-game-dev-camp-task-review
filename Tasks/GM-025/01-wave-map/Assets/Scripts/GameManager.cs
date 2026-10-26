using UnityEngine;
using UnityEngine.SceneManagement;

// 진행 상태(플레이 중 / 클리어 / 게임오버)와 화면 왼쪽 위 표시, R 재시작.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController player;
    public Room[] rooms;

    public enum State { Playing, Clear, GameOver }
    public State Current { get; private set; } = State.Playing;
    public bool IsPlaying => Current == State.Playing;

    Room currentRoom;
    string message = "WASD move / Space attack";

    void Awake() => Instance = this;

    void Update()
    {
        if (!IsPlaying && Input.GetKeyDown(KeyCode.R)) Restart();
    }

    public void OnRoomEntered(Room room)
    {
        currentRoom = room;
        message = $"{room.name}: {room.data.spawnCount} enemies";
    }

    public void OnRoomCleared(Room room)
    {
        if (room == rooms[rooms.Length - 1])
        {
            Current = State.Clear;
            message = "CLEAR!  (R: restart)";
            Debug.Log("모든 룸 클리어");
        }
        else
        {
            message = $"{room.name} cleared - door open";
        }
    }

    public void GameOver()
    {
        Current = State.GameOver;
        message = "GAME OVER  (R: restart)";
        Debug.Log("게임오버");
    }

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = 22 };
        string room = currentRoom == null ? "-" : $"{currentRoom.name}  enemies {currentRoom.Alive}/{currentRoom.data.spawnCount}";
        GUI.Label(new Rect(16, 12, 800, 32), $"HP {player.Hp}/{player.maxHp}", style);
        GUI.Label(new Rect(16, 44, 800, 32), room, style);
        GUI.Label(new Rect(16, 76, 800, 32), message, style);
    }
}
