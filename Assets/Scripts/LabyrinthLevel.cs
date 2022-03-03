/*

Enchanted by Keys of Moon | https://soundcloud.com/keysofmoon
Music promoted by https://www.chosic.com/free-music/all/
Creative Commons CC BY 4.0
https://creativecommons.org/licenses/by/4.0/

Contemplate the stars by Meydän | https://linktr.ee/meydan
Music promoted by https://www.chosic.com/free-music/all/
Creative Commons CC BY 4.0
https://creativecommons.org/licenses/by/4.0/

 * */

using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LabyrinthLevel : MonoBehaviour
{
    public List<Material> Skyboxes;
    public List<Color> SkyLightColors;

    public Light SkyLight;

    public List<AudioClip> BackgroundMusic;

    public List<GameObject> StraightTiles;
    public List<GameObject> CornerTiles;
    public List<GameObject> TFormTiles;

    public List<GameObject> Borders;

    public GameObject PlayerBase;

    public Material RedMaterial;
    public Material BlueMaterial;
    public Material GreenMaterial;
    public Material YellowMaterial;

    public AudioClip RockMoveSoundA;
    public AudioClip RockMoveSoundB;

    public AudioClip RockPushSound;

    public AudioClip RockRotateSound;

    public Camera GameCamera;

    public GameObject EscapeMenu;
    private EscapeMenuControls _escapeMenuControls;
    public GameObject IngameMenu;

    public float TileWidth = 10;
    public float TileHeight = 5;

    public List<GameObject> Collectables;

    public InputActionMap LevelControls;

    private GameObject[] _players;

    private GameObject _border;
    private GameObject[,] _tiles;

    private GameObject _looseTile;
    private LooseTileLocation _currentLooseTileLocation;
    private LooseTileLocation _invalidLooseTileLocation;

    private System.Random _random;

    private bool _isMoveAxisInUse;

    // Start is called before the first frame update
    void Start()
    {
        _random = new System.Random();
        _players = new GameObject[4];
        _tiles = new GameObject[7, 7];

        RenderSettings.skybox = Skyboxes[(int)GameRules.GameTheme];
        SkyLight.GetComponent<Light>().color = SkyLightColors[(int)GameRules.GameTheme];

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = BackgroundMusic[(int)GameRules.GameTheme];
        audioSource.Play();

        if(GameRules.GameTheme == GameTheme.Ice)
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.65f, 0.65f, 0.65f);
        }

        GameCamera.transform.position = new Vector3(TileWidth * 3, TileWidth * 9, TileWidth * 3);
        GameCamera.transform.Rotate(new Vector3(90, 0, 0));

        _currentLooseTileLocation = LooseTileLocation.BottomLeft;
        _invalidLooseTileLocation = LooseTileLocation.None;

        _isMoveAxisInUse = false;

        _escapeMenuControls = EscapeMenu.GetComponent<EscapeMenuControls>();

        GameRules.AssignRandomCollectablesToAllPlayers();

        GenerateMap();

        LevelControls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var player in _players)
        {
            if (player)
            {
                CheckAndRepositionPlayerIfOutOfBounds(player);

                var playerId = Globals.GetPlayerId(player.tag);
                var result = GameRules.CheckPlayerObjective(playerId);

                switch (result)
                {
                    case ObjectiveCheckResult.None:
                        break;

                    case ObjectiveCheckResult.GameWon:

                        // Do something with UI and pause the game

                        break;

                    case ObjectiveCheckResult.Collectable:

                        // Change card on UI 

                        break;

                    case ObjectiveCheckResult.ReturnToStart:

                        // Display on UI that all cards have been collected and that the player needs to return home now

                        break;
                }
            }
        }

        if(LevelControls["Escape"].WasPressedThisFrame())
        {
            if (_escapeMenuControls.IsActive())
            {
                _escapeMenuControls.GoBackToGame();
            }
            else
            {
                _escapeMenuControls.ShowEscapeMenu();
            }
        }

        if (EscapeMenu.activeSelf == false)
        {
            if (GameRules.IsInPossessionMode)
            {
                if (LevelControls["Push"].WasPressedThisFrame())
                {
                    UnpossessPlayer(_players[GameRules.GetCurrentPlayer()]);
                    GameRules.SetNextPlayer();
                }
            }
            else
            {
                if (false == GameRules.IsPushingTiles)
                {
                    if (LevelControls["Rotate"].WasPressedThisFrame())
                    {
                        PlayRockRotateSound();
                        _looseTile.transform.Rotate(0, 90, 0);
                    }

                    var horizontalMovement = LevelControls["Horizontal"].ReadValue<float>();
                    if (horizontalMovement > 0)
                    {
                        if (false == _isMoveAxisInUse)
                        {
                            RepositionLoseTile(LooseTileMoveDirection.Right);
                            _isMoveAxisInUse = true;
                        }
                    }

                    if (horizontalMovement < 0)
                    {
                        if (false == _isMoveAxisInUse)
                        {
                            RepositionLoseTile(LooseTileMoveDirection.Left);
                            _isMoveAxisInUse = true;
                        }
                    }

                    if (horizontalMovement == 0)
                    {
                        _isMoveAxisInUse = false;
                    }

                    if (LevelControls["Push"].WasPressedThisFrame())
                    {
                        PushLooseTile(() =>
                        {
                            GameRules.StopPushing();

                            PossessPlayer(_players[GameRules.GetCurrentPlayer()]);
                        });
                    }
                }
            }
        }
    }

    private void GenerateMap()
    {
        int straightTilesLeft = 12;
        int cornerTilesLeft = 16;
        int tFormTilesLeft = 18;

        _border = Instantiate(Borders[(int)GameRules.GameTheme], new Vector3(30, 2.5f, 30), Quaternion.identity);
        _border.SetActive(false);

        _tiles[0, 0] = Instantiate(CornerTiles[(int)GameRules.GameTheme], new Vector3(0, 0, 0), Quaternion.identity);
        _tiles[0, 0].transform.Rotate(0, 90, 0);

        _players[(int)PlayerTags.Player_0] = Instantiate(PlayerBase, new Vector3(0, 1, 0), Quaternion.identity);
        _players[(int)PlayerTags.Player_0].GetComponentInChildren<MeshRenderer>().material = new Material(RedMaterial);
        _players[(int)PlayerTags.Player_0].GetComponent<MovementHandler>().SetEscapeMenu(_escapeMenuControls);
        _players[(int)PlayerTags.Player_0].GetComponent<MouseHandler>().SetEscapeMenu(_escapeMenuControls);
        _players[(int)PlayerTags.Player_0].tag = Globals.GetEnumAsName(PlayerTags.Player_0);

        _tiles[6, 0] = Instantiate(CornerTiles[(int)GameRules.GameTheme], new Vector3(6 * TileWidth, 0, 0), Quaternion.identity);

        if (GameRules.PlayerCount >= 2)
        {
            _players[(int)PlayerTags.Player_1] = Instantiate(PlayerBase, new Vector3(6 * TileWidth, 1, 0.25f), Quaternion.identity);
            _players[(int)PlayerTags.Player_1].GetComponentInChildren<MeshRenderer>().material = new Material(GreenMaterial);
            _players[(int)PlayerTags.Player_1].GetComponent<MovementHandler>().SetEscapeMenu(_escapeMenuControls);
            _players[(int)PlayerTags.Player_1].GetComponent<MouseHandler>().SetEscapeMenu(_escapeMenuControls);
            _players[(int)PlayerTags.Player_1].tag = Globals.GetEnumAsName(PlayerTags.Player_1);
        }

        _tiles[6, 6] = Instantiate(CornerTiles[(int)GameRules.GameTheme], new Vector3(6 * TileWidth, 0, 6 * TileWidth), Quaternion.identity);
        _tiles[6, 6].transform.Rotate(0, 270, 0);

        if (GameRules.PlayerCount >= 3)
        {
            _players[(int)PlayerTags.Player_2] = Instantiate(PlayerBase, new Vector3(6 * TileWidth, 1, 6 * TileWidth), Quaternion.identity);
            _players[(int)PlayerTags.Player_2].GetComponentInChildren<MeshRenderer>().material = new Material(BlueMaterial);
            _players[(int)PlayerTags.Player_2].GetComponent<MovementHandler>().SetEscapeMenu(_escapeMenuControls);
            _players[(int)PlayerTags.Player_2].GetComponent<MouseHandler>().SetEscapeMenu(_escapeMenuControls);
            _players[(int)PlayerTags.Player_2].tag = Globals.GetEnumAsName(PlayerTags.Player_2);
        }

        _tiles[0, 6] = Instantiate(CornerTiles[(int)GameRules.GameTheme], new Vector3(0, 0, 6 * TileWidth), Quaternion.identity);
        _tiles[0, 6].transform.Rotate(0, 180, 0);

        if (GameRules.PlayerCount >= 4)
        {
            _players[(int)PlayerTags.Player_3] = Instantiate(PlayerBase, new Vector3(0.25f, 1, 6 * TileWidth), Quaternion.identity);
            _players[(int)PlayerTags.Player_3].GetComponentInChildren<MeshRenderer>().material = new Material(YellowMaterial);
            _players[(int)PlayerTags.Player_3].GetComponent<MovementHandler>().SetEscapeMenu(_escapeMenuControls);
            _players[(int)PlayerTags.Player_3].GetComponent<MouseHandler>().SetEscapeMenu(_escapeMenuControls);
            _players[(int)PlayerTags.Player_3].tag = Globals.GetEnumAsName(PlayerTags.Player_3);
        }

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if (x == 0 && y == 0 ||
                   x == 6 && y == 0 ||
                   x == 6 && y == 6 ||
                   x == 0 && y == 6)
                {
                    continue;
                }
                else
                {
                    bool successfulGenerationOfTile = false;
                    while (false == successfulGenerationOfTile)
                    {
                        TileType tileType = (TileType)_random.Next(0, 3);
                        switch (tileType)
                        {
                            case TileType.Straight:
                                if (straightTilesLeft > 0)
                                {
                                    _tiles[x, y] = Instantiate(StraightTiles[(int)GameRules.GameTheme], new Vector3(x * TileWidth, 0, y * TileWidth), Quaternion.identity);
                                    straightTilesLeft--;
                                    successfulGenerationOfTile = true;
                                }
                                break;

                            case TileType.Corner:
                                if (cornerTilesLeft > 0)
                                {
                                    _tiles[x, y] = Instantiate(CornerTiles[(int)GameRules.GameTheme], new Vector3(x * TileWidth, 0, y * TileWidth), Quaternion.identity);
                                    cornerTilesLeft--;
                                    successfulGenerationOfTile = true;
                                }
                                break;

                            case TileType.TForm:
                                if (tFormTilesLeft > 0)
                                {
                                    _tiles[x, y] = Instantiate(TFormTiles[(int)GameRules.GameTheme], new Vector3(x * TileWidth, 0, y * TileWidth), Quaternion.identity);
                                    tFormTilesLeft--;
                                    successfulGenerationOfTile = true;
                                }
                                break;
                        }
                    }

                    var randomRotation = _random.Next(0, 4);
                    _tiles[x, y].transform.Rotate(0, 90 * randomRotation, 0);
                }
            }
        }

        var takenPositions = new List<Vector3>();
        takenPositions.Add(new Vector3(0 * TileWidth, 0.5f, 0 * TileWidth));
        takenPositions.Add(new Vector3(6 * TileWidth, 0.5f, 0 * TileWidth));
        takenPositions.Add(new Vector3(6 * TileWidth, 0.5f, 6 * TileWidth));
        takenPositions.Add(new Vector3(0 * TileWidth, 0.5f, 6 * TileWidth));

        foreach(var collectable in Collectables)
        {
            bool wasSet = false;

            while (wasSet == false)
            {
                var x = _random.Next(0, 7);
                var y = _random.Next(0, 7);
                var location = new Vector3(x * TileWidth, 0.5f, y * TileWidth);

                if (takenPositions.Contains(location) == false)
                {
                    var collectableInstance = Instantiate(collectable, location, Quaternion.identity);
                    collectableInstance.transform.parent = _tiles[x, y].transform;
                    takenPositions.Add(location);
                    wasSet = true;
                }
            }
        }

        if(tFormTilesLeft > 0)
            _looseTile = Instantiate(TFormTiles[(int)GameRules.GameTheme], new Vector3(TileWidth, 0, -1 * TileWidth), Quaternion.identity);
        else if(straightTilesLeft > 0)
            _looseTile = Instantiate(StraightTiles[(int)GameRules.GameTheme], new Vector3(TileWidth, 0, -1 * TileWidth), Quaternion.identity);
        else 
            _looseTile = Instantiate(CornerTiles[(int)GameRules.GameTheme], new Vector3(TileWidth, 0, -1 * TileWidth), Quaternion.identity);
    }

    private void RepositionLoseTile(LooseTileMoveDirection moveDirection)
    {
        PlayRockMoveSound();

        if (moveDirection == LooseTileMoveDirection.Left)
        {
            _currentLooseTileLocation--;
            if (_invalidLooseTileLocation == _currentLooseTileLocation)
                _currentLooseTileLocation--;

            if (_currentLooseTileLocation < LooseTileLocation.BottomLeft)
                _currentLooseTileLocation = LooseTileLocation.LeftBottom;
        }
        else
        {
            _currentLooseTileLocation++;
            if (_invalidLooseTileLocation == _currentLooseTileLocation)
                _currentLooseTileLocation++;

            if (_currentLooseTileLocation > LooseTileLocation.LeftBottom)
                _currentLooseTileLocation = LooseTileLocation.BottomLeft;
        }

        switch(_currentLooseTileLocation)
        {
            case LooseTileLocation.BottomLeft:
                _looseTile.transform.position = new Vector3(TileWidth, 0, -1 * TileWidth);
                break;

            case LooseTileLocation.BottomMiddle:
                _looseTile.transform.position = new Vector3(3 * TileWidth, 0, -1 * TileWidth);
                break;

            case LooseTileLocation.BottomRight:
                _looseTile.transform.position = new Vector3(5 * TileWidth, 0, -1 * TileWidth);
                break;

            case LooseTileLocation.RightBottom:
                _looseTile.transform.position = new Vector3(7 * TileWidth, 0, TileWidth);
                break;

            case LooseTileLocation.RightMiddle:
                _looseTile.transform.position = new Vector3(7 * TileWidth, 0, 3 * TileWidth);
                break;

            case LooseTileLocation.RightTop:
                _looseTile.transform.position = new Vector3(7 * TileWidth, 0, 5 * TileWidth);
                break;

            case LooseTileLocation.TopRight:
                _looseTile.transform.position = new Vector3(5 * TileWidth, 0, 7 * TileWidth);
                break;

            case LooseTileLocation.TopMiddle:
                _looseTile.transform.position = new Vector3(3 * TileWidth, 0, 7 * TileWidth);
                break;

            case LooseTileLocation.TopLeft:
                _looseTile.transform.position = new Vector3(TileWidth, 0, 7 * TileWidth);
                break;

            case LooseTileLocation.LeftTop:
                _looseTile.transform.position = new Vector3(-1 * TileWidth, 0, 5 * TileWidth);
                break;

            case LooseTileLocation.LeftMiddle:
                _looseTile.transform.position = new Vector3(-1 * TileWidth, 0, 3 * TileWidth);
                break;

            case LooseTileLocation.LeftBottom:
                _looseTile.transform.position = new Vector3(-1 * TileWidth, 0, TileWidth);
                break;
        }
    }

    private bool PushLooseTile(System.Action onPushDone)
    {
        if (_currentLooseTileLocation == _invalidLooseTileLocation)
        {
            // TODO: Notify player
            return false;
        }

        PlayRockPushSound();

        switch (_currentLooseTileLocation)
        {
            case LooseTileLocation.BottomLeft:
                DoPushVerticalUp(1, LooseTileLocation.TopLeft, onPushDone);
                break;

            case LooseTileLocation.BottomMiddle:
                DoPushVerticalUp(3, LooseTileLocation.TopMiddle, onPushDone);
                break;

            case LooseTileLocation.BottomRight:
                DoPushVerticalUp(5, LooseTileLocation.TopRight, onPushDone);
                break;

            case LooseTileLocation.RightBottom:
                DoPushHorizontalLeft(1, LooseTileLocation.LeftBottom, onPushDone);
                break;

            case LooseTileLocation.RightMiddle:
                DoPushHorizontalLeft(3, LooseTileLocation.LeftMiddle, onPushDone);
                break;

            case LooseTileLocation.RightTop:
                DoPushHorizontalLeft(5, LooseTileLocation.LeftTop, onPushDone);
                break;

            case LooseTileLocation.TopRight:
                DoPushVerticalDown(5, LooseTileLocation.BottomRight, onPushDone);
                break;

            case LooseTileLocation.TopMiddle:
                DoPushVerticalDown(3, LooseTileLocation.BottomMiddle, onPushDone);
                break;

            case LooseTileLocation.TopLeft:
                DoPushVerticalDown(1, LooseTileLocation.BottomLeft, onPushDone);
                break;

            case LooseTileLocation.LeftTop:
                DoPushHorizontalRight(5, LooseTileLocation.RightTop, onPushDone);
                break;

            case LooseTileLocation.LeftMiddle:
                DoPushHorizontalRight(3, LooseTileLocation.RightMiddle, onPushDone);
                break;

            case LooseTileLocation.LeftBottom:
                DoPushHorizontalRight(1, LooseTileLocation.RightBottom, onPushDone);
                break;
        }

        return true;
    }

    private void DoPushVerticalUp(int column, LooseTileLocation newLooseTileLocation, System.Action onPushDone)
    {
        GameRules.StartPushing();
        var direction = new Vector3(0, 0, TileWidth);
        _currentLooseTileLocation = _invalidLooseTileLocation = newLooseTileLocation;

        StartCoroutine(GameplayStatics.MoveToPosition(_looseTile.transform, _looseTile.transform.position + direction, 1, onPushDone));

        for (int y = 0; y < 7; y++)
        {
            foreach(var player in _players)
            {
                if(player)
                    if (GameObject.ReferenceEquals(player.GetComponent<MovementHandler>().GetCurrentTile(), _tiles[column, y]))
                        StartCoroutine(GameplayStatics.MoveToPosition(player.transform, player.transform.position + direction, 1));
            }

            if (y == 6)
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[column, y].transform, _tiles[column, y].transform.position + direction, 1, onPushDone));
            }
            else
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[column, y].transform, _tiles[column, y].transform.position + direction, 1));
            }

            var temp = _tiles[column, y];
            _tiles[column, y] = _looseTile;
            _looseTile = temp;
        }
    }

    private void DoPushVerticalDown(int column, LooseTileLocation newLooseTileLocation, System.Action onPushDone)
    {
        GameRules.StartPushing();
        var direction = new Vector3(0, 0, -1 * TileWidth);
        _currentLooseTileLocation = _invalidLooseTileLocation = newLooseTileLocation;

        StartCoroutine(GameplayStatics.MoveToPosition(_looseTile.transform, _looseTile.transform.position + direction, 1));

        for (int y = 6; y >= 0; y--)
        {
            foreach (var player in _players)
            {
                if (player)
                    if (GameObject.ReferenceEquals(player.GetComponent<MovementHandler>().GetCurrentTile(), _tiles[column, y]))
                        StartCoroutine(GameplayStatics.MoveToPosition(player.transform, player.transform.position + direction, 1));
            }

            if (y == 0)
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[column, y].transform, _tiles[column, y].transform.position + direction, 1, onPushDone));
            }
            else
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[column, y].transform, _tiles[column, y].transform.position + direction, 1));
            }

            var temp = _tiles[column, y];
            _tiles[column, y] = _looseTile;
            _looseTile = temp;
        }
    }

    private void DoPushHorizontalRight(int row, LooseTileLocation newLooseTileLocation, System.Action onPushDone)
    {
        GameRules.StartPushing();
        var direction = new Vector3(TileWidth, 0, 0);
        _currentLooseTileLocation = _invalidLooseTileLocation = newLooseTileLocation;

        StartCoroutine(GameplayStatics.MoveToPosition(_looseTile.transform, _looseTile.transform.position + direction, 1, onPushDone));

        for (int x = 0; x < 7; x++)
        {
            foreach (var player in _players)
            {
                if (player)
                    if (GameObject.ReferenceEquals(player.GetComponent<MovementHandler>().GetCurrentTile(), _tiles[x, row]))
                        StartCoroutine(GameplayStatics.MoveToPosition(player.transform, player.transform.position + direction, 1));
            }

            if (x == 6)
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[x, row].transform, _tiles[x, row].transform.position + direction, 1, onPushDone));
            }
            else
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[x, row].transform, _tiles[x, row].transform.position + direction, 1));
            }

            var temp = _tiles[x, row];
            _tiles[x, row] = _looseTile;
            _looseTile = temp;
        }
    }

    private void DoPushHorizontalLeft(int row, LooseTileLocation newLooseTileLocation, System.Action onPushDone)
    {
        GameRules.StartPushing();
        var direction = new Vector3(-1 * TileWidth, 0, 0);
        _currentLooseTileLocation = _invalidLooseTileLocation = newLooseTileLocation;

        StartCoroutine(GameplayStatics.MoveToPosition(_looseTile.transform, _looseTile.transform.position + direction, 1));

        for (int x = 6; x >= 0; x--)
        {
            foreach (var player in _players)
            {
                if (player)
                    if (GameObject.ReferenceEquals(player.GetComponent<MovementHandler>().GetCurrentTile(), _tiles[x, row]))
                        StartCoroutine(GameplayStatics.MoveToPosition(player.transform, player.transform.position + direction, 1));
            }

            if (x == 0)
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[x, row].transform, _tiles[x, row].transform.position + direction, 1, onPushDone));
            }
            else
            {
                StartCoroutine(GameplayStatics.MoveToPosition(_tiles[x, row].transform, _tiles[x, row].transform.position + direction, 1));
            }

            var temp = _tiles[x, row];
            _tiles[x, row] = _looseTile;
            _looseTile = temp;
        }
    }

    private void PossessPlayer(GameObject player)
    {
        if (player)
        {
            IngameMenu.SetActive(false);
            _border.SetActive(true);
            GameRules.GoIntoPossessionMode();
            GameCamera.GetComponent<AudioListener>().enabled = false;
            player.GetComponent<MovementHandler>().Activate();
            player.GetComponent<MouseHandler>().Activate();
            _looseTile.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void UnpossessPlayer(GameObject player)
    {
        if (player)
        {
            IngameMenu.SetActive(true);
            _border.SetActive(false);
            GameRules.LeavePossessionMode();
            player.GetComponent<MovementHandler>().Deactivate();
            player.GetComponent<MouseHandler>().Deactivate();
            _looseTile.GetComponent<MeshRenderer>().enabled = true;
            GameCamera.GetComponent<AudioListener>().enabled = true;
        }
    }

    private void PlayRockMoveSound()
    {
        if (_random.Next(0, 2) == 0)
            AudioSource.PlayClipAtPoint(RockMoveSoundA, GameCamera.transform.position, AudioListener.volume);
        else
            AudioSource.PlayClipAtPoint(RockMoveSoundB, GameCamera.transform.position, AudioListener.volume);
    }

    private void PlayRockPushSound()
    {
        AudioSource.PlayClipAtPoint(RockPushSound, GameCamera.transform.position, AudioListener.volume);
    }

    private void PlayRockRotateSound()
    {
        AudioSource.PlayClipAtPoint(RockRotateSound, GameCamera.transform.position, AudioListener.volume);
    }

    private void CheckAndRepositionPlayerIfOutOfBounds(GameObject player)
    {
        if (player)
        {
            if (player.transform.position.x > TileWidth * 6 + (TileWidth / 2))
            {
                player.GetComponent<MovementHandler>().SetNewPosition(new Vector3(0, player.transform.position.y, GameplayStatics.RoundToNearest10(player.transform.position.z)));
            }

            if (player.transform.position.x < 0 - (TileWidth / 2))
            {
                player.GetComponent<MovementHandler>().SetNewPosition(new Vector3(TileWidth * 6, player.transform.position.y, GameplayStatics.RoundToNearest10(player.transform.position.z)));
            }

            if (player.transform.position.z > TileWidth * 6 + (TileWidth / 2))
            {
                player.GetComponent<MovementHandler>().SetNewPosition(player.transform.position = new Vector3(GameplayStatics.RoundToNearest10(player.transform.position.x), player.transform.position.y, 0));
            }

            if (player.transform.position.z < 0 - (TileWidth / 2))
            {
                player.GetComponent<MovementHandler>().SetNewPosition(player.transform.position = new Vector3(GameplayStatics.RoundToNearest10(player.transform.position.x), player.transform.position.y, TileWidth * 6));
            }
        }
    }
}
