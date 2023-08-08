using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I = 0,
    O = 1,
    T = 2,
    J = 3,
    L = 4,
    S = 5,
    Z = 6
}

[Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;

    public Vector2Int[] Cells { get; private set; }
    public Vector2Int[,] WallKicks { get; private set; }

    public void Initialize()
    {
        Cells = Data.Cells[tetromino];
        WallKicks = Data.WallKicks[tetromino];
    }
}