using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board;
    public TetrominoData tetrominoData; // --- THÊM DÒNG NÀY ĐỂ NHỚ MẶT GẠCH ---
    public float stepDelay = 1f;
    private float moveTimer;

    void Update()
    {
        // 1. Logic rơi tự động theo thời gian
        if (Time.time >= moveTimer)
        {
            Move(Vector2Int.down);
            moveTimer = Time.time + stepDelay;
        }

        // 2. Code điều khiển (ĐÃ XÓA ĐOẠN BỊ NHÂN ĐÔI)
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { Move(Vector2Int.left); }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { Move(Vector2Int.right); }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) { Move(Vector2Int.down); }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) { Rotate(); }
        else if (Input.GetKeyDown(KeyCode.Space)) { HardDrop(); }
        else if (Input.GetKeyDown(KeyCode.C)) { FindFirstObjectByType<Spawner>().Hold(); }
    }

    // --- CÁC HÀM PUBLIC ĐỂ NGÓN TAY ĐIỀU KHIỂN ---
    public void MoveLeft() { Move(Vector2Int.left); }
    public void MoveRight() { Move(Vector2Int.right); }

    // Rơi chậm (Soft Drop): Ép nó nhích xuống 1 ô ngay lập tức và reset lại thời gian rơi
    public void SoftDrop()
    {
        if (Move(Vector2Int.down))
        {
            moveTimer = Time.time + stepDelay;
        }
    }

    bool Move(Vector2Int translation)
    {
        if (!this.enabled) return false;
        Vector3 newPos = transform.position;
        newPos.x += translation.x;
        newPos.y += translation.y;
        transform.position = newPos;

        if (!board.IsValidPosition(this.transform))
        {
            transform.position -= new Vector3(translation.x, translation.y, 0);
            if (translation == Vector2Int.down) Lock();
            return false;
        }
        return true;
    }

    // CHỈ CẦN THÊM CHỮ "public" VÀO ĐÂY LÀ HẾT ĐỎ!
    public void Rotate()
    {
        if (!this.enabled) return;
        transform.eulerAngles -= new Vector3(0, 0, 90);
        if (!board.IsValidPosition(this.transform)) transform.eulerAngles += new Vector3(0, 0, 90);
    }

    public void HardDrop()
    {
        if (!this.enabled) return;
        while (board.IsValidPosition(this.transform)) transform.position += Vector3.down;
        transform.position += Vector3.up;
        Lock();
    }
    void Lock()
    {
        foreach (Transform child in transform)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            board.grid[pos.x, pos.y] = child;
        }
        this.enabled = false;
        board.CheckForLines();
        FindFirstObjectByType<Spawner>().SpawnPiece();
    }
}