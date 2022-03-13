using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public enum GameTheme
    {
        Default,
        Ice
    }

    public enum PlayerTags : int
    {
        Player_0 = 0,
        Player_1,
        Player_2,
        Player_3
    }

    public enum TileType : int
    {
        Straight = 0,
        Corner = 1,
        TForm = 2
    }

    public enum LooseTileLocation : int
    {
        None = -1,
        BottomLeft,
        BottomMiddle,
        BottomRight,
        RightBottom,
        RightMiddle,
        RightTop,
        TopRight,
        TopMiddle,
        TopLeft,
        LeftTop,
        LeftMiddle,
        LeftBottom
    }

    public enum LooseTileMoveDirection : int
    {
        Left,
        Right
    }

    public enum Collectable
    {
        Anvil = 0,
        Barrel,
        Book,
        Bricks,
        Candle,
        Chalice,
        Cheese,
        Chest,
        Coin,
        CoinPouch,
        Crystal,
        Diamond,
        Dice,
        Hammer,
        Mushroom,
        Key,
        Perl,
        Ring,
        Scepter,
        Screw,
        Shield,
        Stone,
        Sword,
        WoodenStump
    }

    public enum Objective : int
    {
        Collectable,
        ReturnToStart
    }

    public enum ObjectiveCheckResult : int
    {
        None = 0,
        GameWon,
        Collectable,
        ReturnToStart
    }

    public enum VideoQualityLevels : int
    {
        VeryLow = 0,
        Low,
        Medium,
        High,
        VeryHigh,
        Ultra
    }
}
