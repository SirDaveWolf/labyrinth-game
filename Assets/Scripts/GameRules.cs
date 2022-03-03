using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class PlayerObjective
    {
        public PlayerObjective()
        {
            Objective = Objective.Collectable;
            Collectable = Collectable.Coin;
        }

        public PlayerObjective(Objective objective, Collectable collectable)
        {
            Objective = objective;
            Collectable = collectable;
        }

        public Objective Objective { get; set; }
        public Collectable Collectable { get; set; }
    }

    public class PlayerStats
    {
        public PlayerObjective CurrentObjective { get; set; } = new PlayerObjective();
        public String LastTouchedObjective { get; set; } = String.Empty;
        public Queue<Collectable> RemainingCollectables { get; set; } = new Queue<Collectable>();
    }

    public static class GameRules
    {
        public static int PlayerCount { get; set; } = 2;
        public static GameTheme GameTheme { get; set; } = GameTheme.Default;
        private static PlayerStats[] PlayerStats { get; set; } = new PlayerStats[4];

        private static PlayerTags CurrentPlayer { get; set; } = PlayerTags.Player_0;
        public static bool IsInPossessionMode { get; private set; } = false;

        public static bool IsPushingTiles { get; private set; } = false;

        public static void GoIntoPossessionMode()
        {
            IsInPossessionMode = true;
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;
        }

        public static void LeavePossessionMode()
        {
            IsInPossessionMode = false;
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
        }

        public static void StartPushing()
        {
            IsPushingTiles = true;
        }

        public static void StopPushing()
        {
            IsPushingTiles = false;
        }

        public static void SetNextPlayer()
        {
            CurrentPlayer++;
            if ((int)CurrentPlayer > GameRules.PlayerCount - 1)
                CurrentPlayer = 0;
        }

        public static int GetCurrentPlayer()
        {
            return (int)CurrentPlayer;
        }

        public static void AssignRandomCollectablesToAllPlayers()
        {
            var random = new Random();
            var allCollectables = new List<Collectable>(Enum.GetValues(typeof(Collectable)).Cast<Collectable>());
            var shuffledCollectables = new Queue<Collectable>(allCollectables.OrderBy(x => random.Next()));
            for(var playerId = 0; playerId < PlayerCount; playerId++)
            {
                PlayerStats[playerId] = new PlayerStats();

                for (var i = 0; i < 6; i++)
                {
                    if (shuffledCollectables.Any())
                    {
                        PlayerStats[playerId].RemainingCollectables.Enqueue(shuffledCollectables.Dequeue());
                    }
                }
            }
        }

        public static ObjectiveCheckResult CheckPlayerObjective(int playerId)
        {
            var checkResult = ObjectiveCheckResult.None;

            if (IsValidPlayer(playerId))
            {
                if (PlayerStats[playerId].CurrentObjective.Objective == Objective.ReturnToStart)
                {
                    if (PlayerStats[playerId].LastTouchedObjective.Contains("PlayerStart"))
                    {
                        var playerStartNumber = Globals.GetPlayerId(PlayerStats[playerId].LastTouchedObjective);
                        if (playerId == playerStartNumber)
                        {
                            checkResult = ObjectiveCheckResult.GameWon;
                        }
                    }
                }
                else
                {
                    var objectiveNameAsString = Globals.GetEnumAsName(PlayerStats[playerId].CurrentObjective.Collectable);
                    if (PlayerStats[playerId].LastTouchedObjective.Contains(objectiveNameAsString))
                    {
                        SetPlayerNextObjective(playerId);

                        switch (PlayerStats[playerId].CurrentObjective.Objective)
                        {
                            case Objective.ReturnToStart:
                                checkResult = ObjectiveCheckResult.ReturnToStart;
                                break;

                            case Objective.Collectable:
                                checkResult = ObjectiveCheckResult.Collectable;
                                break;
                        }
                    }
                }

                SetPlayerLastTouched(playerId, String.Empty);
            }

            return checkResult;
        }

        private static void SetPlayerNextObjective(int playerId)
        {
            if (IsValidPlayer(playerId))
            {
                if (PlayerStats[playerId].RemainingCollectables.Any())
                {
                    PlayerStats[playerId].CurrentObjective = new PlayerObjective(Objective.Collectable, PlayerStats[playerId].RemainingCollectables.Dequeue());
                }
                else
                {
                    PlayerStats[playerId].CurrentObjective = new PlayerObjective(Objective.ReturnToStart, Collectable.Coin);
                }
            }
        }

        private static bool IsValidPlayer(int playerId)
        {
            return playerId < 4 && playerId >= 0;
        }

        public static void SetPlayerLastTouched(int playerId, string value)
        {
            if(IsValidPlayer(playerId))
            {
                PlayerStats[playerId].LastTouchedObjective = value;
            }
        }

        public static void Reset()
        {
            IsInPossessionMode = false;
            CurrentPlayer = PlayerTags.Player_0;
        }
    }
}