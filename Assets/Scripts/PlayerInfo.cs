using UnityEngine;


public static class PlayerInfo
{
    public static bool HasData = false;
    public static int CurrentHealth = 0;
    public static int CurrentScore = 0;
    public static int NumRemainingLives = 0;
    public static int GameLoopCount = 1;
    public static bool ForceFreshStart = false;
    public static float StoredGameTime = 0f;
    public static int HighScore = 0;

    public static void Clear()
    {
        HasData = false;
        CurrentHealth = 0;
        CurrentScore = 0;
        NumRemainingLives = 0;
        StoredGameTime = 0f;
    }
}

