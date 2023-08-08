using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    [SerializeField] Tile _tile;
    [SerializeField] Board _board;
    [SerializeField] Piece _trackingPiece;

    public Tilemap Tilemap { get; protected set; }
    public Vector3Int[] Cells { get; protected set; }
    public Vector3Int Position { get; protected set; }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = _trackingPiece.Cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = _trackingPiece.Position;
        int currentRow = position.y;
        int bottom = (-_board.BoardSize.y / 2) - 1;
        _board.Clear(_trackingPiece);

        for (int row = currentRow; row >= bottom; row--)
        {
            position.y = row;

            if (_board.IsValidPosition(_trackingPiece, position))
            {
                Position = position;
            }
            else
            {
                break;
            }
        }

        _board.Set(_trackingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, _tile);
        }
    }
}
