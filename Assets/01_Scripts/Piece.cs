using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board Board { get; protected set; }
    public TetrominoData TetrominoData { get; protected set; }
    public Vector3Int Position { get; protected set; }
    public Vector3Int[] Cells { get; protected set; }
    public int RotationIndex { get; protected set; }

    [SerializeField] float _stepDelay = 1f;
    [SerializeField] float _lockDelay = 0.5f;

    float _stepTime;
    float _lockTime;

    public void Initialize(Board board, Vector3Int initialPosition, TetrominoData data)
    {
        Board = board;
        Position = initialPosition;
        TetrominoData = data;
        RotationIndex = 0;
        _stepTime = Time.time + _stepDelay;
        _lockTime = 0f;

        if (Cells == null)
        {
            Cells = new Vector3Int[data.Cells.Length];
        }

        for (int i = 0; i < data.Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)data.Cells[i];
        }
    }

    private void Update()
    {
        Board.Clear(this);
        _lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Rotate(1);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Rotate(-1);
        }

        if (Time.time >= _stepTime)
        {
            Step();
        }

        Board.Set(this);
    }

    private void Step()
    {
        _stepTime = Time.time + _stepDelay;
        Move(Vector2Int.down);

        if (_lockTime >= _lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        Board.SpawnPiece();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        bool isValid = Board.IsValidPosition(this, newPosition);

        if (isValid)
        {
            Position = newPosition;
            _lockTime = 0f;
        }

        return isValid;
    }

    private void Rotate(int direction)
    {
        int originalRotationIndex = RotationIndex;
        RotationIndex = Wrap(RotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(RotationIndex, direction))
        {
            RotationIndex = originalRotationIndex;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            int x, y;
            Vector3 cell = Cells[i];

            switch (TetrominoData.tetromino)
            {
                case Tetromino.I:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < TetrominoData.WallKicks.GetLength(1); i++)
        {
            Vector2Int translation = TetrominoData.WallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationIndex < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, TetrominoData.WallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }
}