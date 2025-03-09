using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon;

/// <summary>
/// Main game manager that handles game flow, modes, and player management.
/// Supports Hide-n-Seek and Infection game modes on Call of Duty inspired maps.
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameMode
    {
        HideAndSeek,
        Infection,
        Sandbox
    }
    
    [Header("Game Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.Sandbox;
    [SerializeField] private float roundTime = 300f; // 5 minutes
    [SerializeField] private float prepTime = 30f; // 30 seconds
    [SerializeField] private int minPlayers = 2;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] hideSpawnPoints;
    [SerializeField] private Transform[] seekerSpawnPoints;
    [SerializeField] private GameObject playerPrefab;
    
    [Header("UI References")]
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject endgameUI;
    
    // Private variables
    private float currentRoundTime;
    private float currentPrepTime;
    private bool isRoundActive = false;
    private bool isPreparationPhase = false;
    private List<Player> hidingPlayers = new List<Player>();
    private List<Player> seekingPlayers = new List<Player>();
    private List<Player> infectedPlayers = new List<Player>();
    private Dictionary<Player, int> playerScores = new Dictionary<Player, int>();
    
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    
    // Public properties
    public GameMode CurrentGameMode => currentGameMode;
    public float RemainingTime => currentRoundTime;
    public bool IsRoundActive => isRoundActive;
    public bool IsPreparationPhase => isPreparationPhase;
    
    // Event codes for custom events
    private const byte PLAYER_FOUND_EVENT = 1;
    private const byte PLAYER_INFECTED_EVENT = 2;
    private const byte GAME_STATE_CHANGED_EVENT = 3;
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Register for Photon events
        PhotonNetwork.NetworkingClient.EventReceived += OnNetworkingEvent;
    }
    
    private void Start()
    {
        lobbyUI.SetActive(true);
        gameHUD.SetActive(false);
        endgameUI.SetActive(false);
        
        // Initialize the room if we're the master client
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
            {
                {"GameMode", (int)currentGameMode},
                {"IsActive", false},
                {"RoundTime", roundTime},
                {"PrepTime", prepTime}
            });
        }
    }
    
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (isPreparationPhase)
        {
            currentPrepTime -= Time.deltaTime;
            if (currentPrepTime <= 0)
            {
                StartRound();
            }
        }
        else if (isRoundActive)
        {
            currentRoundTime -= Time.deltaTime;
            if (currentRoundTime <= 0 || CheckGameEndCondition())
            {
                EndRound();
            }
        }
    }
    
    private void OnDestroy()
    {
        // Unregister from Photon events
        PhotonNetwork.NetworkingClient.EventReceived -= OnNetworkingEvent;
    }
    
    #endregion
    
    #region Game Flow
    
    /// <summary>
    /// Starts the preparation phase before the main round
    /// </summary>
    public void StartPreparationPhase()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        currentPrepTime = prepTime;
        isPreparationPhase = true;
        isRoundActive = false;
        
        // Assign roles based on the game mode
        AssignPlayerRoles();
        
        // Teleport players to their spawn positions
        TeleportPlayersToSpawns();
        
        // Update the game state for all clients
        UpdateGameState();
        
        // Raise event to notify all clients
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GAME_STATE_CHANGED_EVENT, new object[] { true, false, currentPrepTime }, raiseEventOptions, SendOptions.SendReliable);
    }
    
    /// <summary>
    /// Starts the main game round
    /// </summary>
    public void StartRound()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        currentRoundTime = roundTime;
        isPreparationPhase = false;
        isRoundActive = true;
        
        // Update the game state for all clients
        UpdateGameState();
        
        // Raise event to notify all clients
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GAME_STATE_CHANGED_EVENT, new object[] { false, true, currentRoundTime }, raiseEventOptions, SendOptions.SendReliable);
    }
    
    /// <summary>
    /// Ends the current round and shows the results
    /// </summary>
    public void EndRound()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        isRoundActive = false;
        isPreparationPhase = false;
        
        // Calculate final scores
        CalculateFinalScores();
        
        // Update the game state for all clients
        UpdateGameState();
        
        // Raise event to notify all clients
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GAME_STATE_CHANGED_EVENT, new object[] { false, false, 0f }, raiseEventOptions, SendOptions.SendReliable);
    }
    
    /// <summary>
    /// Checks if the game should end
    /// </summary>
    private bool CheckGameEndCondition()
    {
        switch (currentGameMode)
        {
            case GameMode.HideAndSeek:
                // End if all hiding players are found
                return hidingPlayers.Count == 0;
                
            case GameMode.Infection:
                // End if all players are infected or if only one non-infected player remains
                return infectedPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount ||
                       PhotonNetwork.CurrentRoom.PlayerCount - infectedPlayers.Count <= 1;
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Sets the game mode
    /// </summary>
    public void SetGameMode(GameMode mode)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        currentGameMode = mode;
        
        // Update the room properties
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
        {
            {"GameMode", (int)currentGameMode}
        });
    }
    
    #endregion
    
    #region Player Management
    
    /// <summary>
    /// Assigns roles to players based on the current game mode
    /// </summary>
    private void AssignPlayerRoles()
    {
        hidingPlayers.Clear();
        seekingPlayers.Clear();
        infectedPlayers.Clear();
        
        List<Player> allPlayers = PhotonNetwork.PlayerList.ToList();
        
        switch (currentGameMode)
        {
            case GameMode.HideAndSeek:
                // Randomly select one seeker
                int seekerIndex = Random.Range(0, allPlayers.Count);
                Player seeker = allPlayers[seekerIndex];
                seekingPlayers.Add(seeker);
                
                // Everyone else is a hider
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    if (i != seekerIndex)
                    {
                        hidingPlayers.Add(allPlayers[i]);
                    }
                }
                
                // Set player properties
                foreach (Player player in allPlayers)
                {
                    player.SetCustomProperties(new Hashtable
                    {
                        {"IsSeeker", seekingPlayers.Contains(player)}
                    });
                }
                break;
                
            case GameMode.Infection:
                // Randomly select one infected player
                int infectedIndex = Random.Range(0, allPlayers.Count);
                Player infected = allPlayers[infectedIndex];
                infectedPlayers.Add(infected);
                
                // Set player properties
                foreach (Player player in allPlayers)
                {
                    player.SetCustomProperties(new Hashtable
                    {
                        {"IsInfected", infectedPlayers.Contains(player)}
                    });
                }
                break;
                
            case GameMode.Sandbox:
                // No specific roles in sandbox mode
                break;
        }
    }
    
    /// <summary>
    /// Teleports players to their starting positions
    /// </summary>
    private void TeleportPlayersToSpawns()
    {
        // Get all player controllers in the scene
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        
        foreach (PlayerController player in players)
        {
            if (!player.photonView.IsMine) continue;
            
            Transform spawnPoint = null;
            
            switch (currentGameMode)
            {
                case GameMode.HideAndSeek:
                    bool isSeeker = (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSeeker"];
                    if (isSeeker)
                    {
                        spawnPoint = seekerSpawnPoints[Random.Range(0, seekerSpawnPoints.Length)];
                    }
                    else
                    {
                        spawnPoint = hideSpawnPoints[Random.Range(0, hideSpawnPoints.Length)];
                    }
                    break;
                    
                case GameMode.Infection:
                    bool isInfected = (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsInfected"];
                    if (isInfected)
                    {
                        spawnPoint = seekerSpawnPoints[Random.Range(0, seekerSpawnPoints.Length)];
                    }
                    else
                    {
                        spawnPoint = hideSpawnPoints[Random.Range(0, hideSpawnPoints.Length)];
                    }
                    break;
                    
                case GameMode.Sandbox:
                    spawnPoint = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
                    break;
            }
            
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
            }
        }
    }
    
    /// <summary>
    /// Reports a player as found in Hide and Seek mode
    /// </summary>
    public void ReportPlayerFound(Player foundPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (currentGameMode == GameMode.HideAndSeek && isRoundActive)
        {
            if (hidingPlayers.Contains(foundPlayer))
            {
                hidingPlayers.Remove(foundPlayer);
                
                // Add score to the seeker
                foreach (Player seeker in seekingPlayers)
                {
                    if (!playerScores.ContainsKey(seeker))
                    {
                        playerScores[seeker] = 0;
                    }
                    playerScores[seeker] += 10;
                }
                
                // Raise event to notify all clients
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PLAYER_FOUND_EVENT, foundPlayer, raiseEventOptions, SendOptions.SendReliable);
                
                // Check if the round should end
                if (CheckGameEndCondition())
                {
                    EndRound();
                }
            }
        }
    }
    
    /// <summary>
    /// Reports a player as infected in Infection mode
    /// </summary>
    public void ReportPlayerInfected(Player infectedPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (currentGameMode == GameMode.Infection && isRoundActive)
        {
            if (!infectedPlayers.Contains(infectedPlayer))
            {
                infectedPlayers.Add(infectedPlayer);
                
                // Update player properties
                infectedPlayer.SetCustomProperties(new Hashtable
                {
                    {"IsInfected", true}
                });
                
                // Raise event to notify all clients
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PLAYER_INFECTED_EVENT, infectedPlayer, raiseEventOptions, SendOptions.SendReliable);
                
                // Check if the round should end
                if (CheckGameEndCondition())
                {
                    EndRound();
                }
            }
        }
    }
    
    /// <summary>
    /// Calculates the final scores for all players
    /// </summary>
    private void CalculateFinalScores()
    {
        switch (currentGameMode)
        {
            case GameMode.HideAndSeek:
                // Add bonus points to hiders who weren't found
                foreach (Player hider in hidingPlayers)
                {
                    if (!playerScores.ContainsKey(hider))
                    {
                        playerScores[hider] = 0;
                    }
                    playerScores[hider] += 20;
                }
                break;
                
            case GameMode.Infection:
                // Add points to survivors
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (!infectedPlayers.Contains(player))
                    {
                        if (!playerScores.ContainsKey(player))
                        {
                            playerScores[player] = 0;
                        }
                        playerScores[player] += 20;
                    }
                }
                break;
        }
    }
    
    #endregion
    
    #region UI Management
    
    /// <summary>
    /// Updates the game state and UI for all clients
    /// </summary>
    private void UpdateGameState()
    {
        Hashtable roomProps = new Hashtable
        {
            {"IsActive", isRoundActive},
            {"IsPrepPhase", isPreparationPhase},
            {"RoundTime", currentRoundTime},
            {"PrepTime", currentPrepTime}
        };
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }
    
    /// <summary>
    /// Updates the UI based on the current game state
    /// </summary>
    public void UpdateUI()
    {
        lobbyUI.SetActive(!isRoundActive && !isPreparationPhase);
        gameHUD.SetActive(isRoundActive || isPreparationPhase);
        endgameUI.SetActive(!isRoundActive && !isPreparationPhase && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameOver") && (bool)PhotonNetwork.CurrentRoom.CustomProperties["GameOver"]);
    }
    
    #endregion
    
    #region Photon Callbacks
    
    private void OnNetworkingEvent(EventData photonEvent)
    {
        if (photonEvent.Code == GAME_STATE_CHANGED_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            isPreparationPhase = (bool)data[0];
            isRoundActive = (bool)data[1];
            
            if (isPreparationPhase)
            {
                currentPrepTime = (float)data[2];
            }
            else if (isRoundActive)
            {
                currentRoundTime = (float)data[2];
            }
            
            UpdateUI();
        }
        else if (photonEvent.Code == PLAYER_FOUND_EVENT)
        {
            Player foundPlayer = (Player)photonEvent.CustomData;
            if (hidingPlayers.Contains(foundPlayer))
            {
                hidingPlayers.Remove(foundPlayer);
            }
        }
        else if (photonEvent.Code == PLAYER_INFECTED_EVENT)
        {
            Player infectedPlayer = (Player)photonEvent.CustomData;
            if (!infectedPlayers.Contains(infectedPlayer))
            {
                infectedPlayers.Add(infectedPlayer);
            }
        }
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GameMode"))
        {
            currentGameMode = (GameMode)propertiesThatChanged["GameMode"];
        }
        
        if (propertiesThatChanged.ContainsKey("IsActive"))
        {
            isRoundActive = (bool)propertiesThatChanged["IsActive"];
        }
        
        if (propertiesThatChanged.ContainsKey("IsPrepPhase"))
        {
            isPreparationPhase = (bool)propertiesThatChanged["IsPrepPhase"];
        }
        
        if (propertiesThatChanged.ContainsKey("RoundTime"))
        {
            currentRoundTime = (float)propertiesThatChanged["RoundTime"];
        }
        
        if (propertiesThatChanged.ContainsKey("PrepTime"))
        {
            currentPrepTime = (float)propertiesThatChanged["PrepTime"];
        }
        
        UpdateUI();
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Spawn the player
            SpawnPlayer(newPlayer);
            
            // Update the new player about the current game state
            UpdateGameState();
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Remove player from lists
            if (hidingPlayers.Contains(otherPlayer))
            {
                hidingPlayers.Remove(otherPlayer);
            }
            
            if (seekingPlayers.Contains(otherPlayer))
            {
                seekingPlayers.Remove(otherPlayer);
            }
            
            if (infectedPlayers.Contains(otherPlayer))
            {
                infectedPlayers.Remove(otherPlayer);
            }
            
            // Check if the game should end
            if (isRoundActive && PhotonNetwork.CurrentRoom.PlayerCount < minPlayers)
            {
                EndRound();
            }
        }
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // If the master client leaves, the new master client continues the game
        UpdateGameState();
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Spawns a player into the game
    /// </summary>
    private void SpawnPlayer(Player player)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        // Find a random spawn point
        Transform spawnPoint = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
        
        // Create player data
        if (!playerScores.ContainsKey(player))
        {
            playerScores[player] = 0;
        }
        
        // If game is in progress, handle differently based on game mode
        if (isRoundActive)
        {
            switch (currentGameMode)
            {
                case GameMode.HideAndSeek:
                    // New players become seekers in active games
                    seekingPlayers.Add(player);
                    player.SetCustomProperties(new Hashtable
                    {
                        {"IsSeeker", true}
                    });
                    break;
                    
                case GameMode.Infection:
                    // New players become infected in active games
                    infectedPlayers.Add(player);
                    player.SetCustomProperties(new Hashtable
                    {
                        {"IsInfected", true}
                    });
                    break;
            }
        }
    }
    
    #endregion
} 