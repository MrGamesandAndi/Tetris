using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField] TetrominoData[] _tetrominos;
    [SerializeField] Vector3Int _spawnPosition;
    [SerializeField] Vector2Int _boardSize = new Vector2Int(10, 20);

    public Tilemap Tilemap { get; protected set; }
    public Piece ActivePiece { get; protected set; }
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2);
            return new RectInt(position, _boardSize);
        }
    }

    public Vector2Int BoardSize { get => _boardSize; set => _boardSize = value; }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        ActivePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < _tetrominos.Length; i++)
        {
            _tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        TetrominoData data = _tetrominos[Random.Range(0, _tetrominos.Length)];
        ActivePiece.Initialize(this, _spawnPosition, data);

        if (IsValidPosition(ActivePiece, _spawnPosition))
        {
            Set(ActivePiece);
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Tilemap.ClearAllTiles();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.TetrominoData.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;

            if (!Bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (Tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        int row = Bounds.yMin;

        while (row < Bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }

    }

    private void LineClear(int row)
    {
        for (int column = Bounds.xMin; column < Bounds.xMax; column++)
        {
            Vector3Int position = new Vector3Int(column, row, 0);
            Tilemap.SetTile(position, null);
        }

        while (row < Bounds.yMax)
        {
            for (int column = Bounds.xMin; column < Bounds.xMax; column++)
            {
                Vector3Int position = new Vector3Int(column, row + 1, 0);
                TileBase above = Tilemap.GetTile(position);
                position = new Vector3Int(column, row, 0);
                Tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    private bool IsLineFull(int row)
    {
        for (int column = Bounds.xMin; column < Bounds.xMax; column++)
        {
            Vector3Int position = new Vector3Int(column, row, 0);

            if (!Tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }
}
