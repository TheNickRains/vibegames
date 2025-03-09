using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all networking functionality for the game using Photon PUN.
/// Manages connections, rooms, and player synchronization.
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Settings")]
    [SerializeField] private string gameVersion = "1.0";
    [SerializeField] private int maxPlayersPerRoom = 16;
    [SerializeField] private string defaultRoomName = "VibeMod_";
    [SerializeField] private string defaultPlayerName = "Player";
    
    [Header("UI References")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button randomJoinButton;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Dropdown gameModeDropdown;
    
    // Private variables
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> roomListItems = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();
    private bool hasJoinedLobby = false;
    
    // Singleton pattern
    public static NetworkManager Instance { get; private set; }
    
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
        
        // Initialize UI
        if (playerNameInput != null)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "");
            if (string.IsNullOrEmpty(savedName))
            {
                savedName = defaultPlayerName + Random.Range(1000, 9999);
                PlayerPrefs.SetString("PlayerName", savedName);
            }
            
            playerNameInput.text = savedName;
        }
        
        // Generate a random room name
        if (roomNameInput != null)
        {
            roomNameInput.text = defaultRoomName + Random.Range(1000, 9999);
        }
    }
    
    private void Start()
    {
        ShowLoginPanel();
        
        // Configure Photon settings
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
    }
    
    #endregion
    
    #region UI Management
    
    /// <summary>
    /// Show the login panel
    /// </summary>
    private void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        errorPanel.SetActive(false);
    }
    
    /// <summary>
    /// Show the lobby panel
    /// </summary>
    private void ShowLobbyPanel()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        errorPanel.SetActive(false);
    }
    
    /// <summary>
    /// Show the room panel
    /// </summary>
    private void ShowRoomPanel()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        errorPanel.SetActive(false);
    }
    
    /// <summary>
    /// Show the error panel with a message
    /// </summary>
    private void ShowErrorPanel(string message)
    {
        errorPanel.SetActive(true);
        errorText.text = message;
    }
    
    /// <summary>
    /// Update the list of available rooms
    /// </summary>
    private void UpdateRoomList()
    {
        // Clear the current list items
        foreach (GameObject item in roomListItems.Values)
        {
            Destroy(item);
        }
        roomListItems.Clear();
        
        // Create new list items
        foreach (RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            roomItem.transform.localScale = Vector3.one;
            
            // Set room info on list item
            RoomListItem listItem = roomItem.GetComponent<RoomListItem>();
            if (listItem != null)
            {
                listItem.Setup(room.Name, room.PlayerCount, room.MaxPlayers);
            }
            
            roomListItems[room.Name] = roomItem;
        }
        
        // Enable/disable buttons based on room list
        joinRoomButton.interactable = roomListItems.Count > 0;
        randomJoinButton.interactable = roomListItems.Count > 0;
    }
    
    /// <summary>
    /// Update the list of players in the room
    /// </summary>
    private void UpdatePlayerList()
    {
        // Clear the current list items
        foreach (GameObject item in playerListItems.Values)
        {
            Destroy(item);
        }
        playerListItems.Clear();
        
        // Create new list items for players in room
        if (PhotonNetwork.CurrentRoom != null)
        {
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);
                playerItem.transform.localScale = Vector3.one;
                
                // Set player info on list item
                PlayerListItem listItem = playerItem.GetComponent<PlayerListItem>();
                if (listItem != null)
                {
                    bool isLocalPlayer = player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
                    listItem.Setup(player.NickName, player.ActorNumber, isLocalPlayer);
                }
                
                playerListItems[player.ActorNumber] = playerItem;
            }
        }
        
        // Show start button only for master client
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Connect to Photon network with player name
    /// </summary>
    public void Connect()
    {
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            // Save player name
            string playerName = playerNameInput.text;
            PlayerPrefs.SetString("PlayerName", playerName);
            
            // Set player name in Photon
            PhotonNetwork.NickName = playerName;
            
            // Connect to Photon
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
            else
            {
                if (hasJoinedLobby)
                {
                    ShowLobbyPanel();
                }
                else
                {
                    PhotonNetwork.JoinLobby();
                }
            }
        }
        else
        {
            ShowErrorPanel("Please enter a valid player name.");
        }
    }
    
    /// <summary>
    /// Create a new room with the specified name
    /// </summary>
    public void CreateRoom()
    {
        if (roomNameInput != null && !string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = (byte)maxPlayersPerRoom,
                IsVisible = true,
                IsOpen = true,
                PublishUserId = true
            };
            
            PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
        }
        else
        {
            ShowErrorPanel("Please enter a valid room name.");
        }
    }
    
    /// <summary>
    /// Join a specific room by name
    /// </summary>
    public void JoinRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }
    
    /// <summary>
    /// Join a random available room
    /// </summary>
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    
    /// <summary>
    /// Leave the current room
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    /// <summary>
    /// Start the game with the current players
    /// </summary>
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Set the game mode based on dropdown selection
            if (gameModeDropdown != null)
            {
                int selectedMode = gameModeDropdown.value;
                
                ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable
                {
                    {"GameMode", selectedMode}
                };
                
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
            }
            
            // Start the game
            PhotonNetwork.LoadLevel("Game");
        }
    }
    
    #endregion
    
    #region Photon Callbacks
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");
        ShowLoginPanel();
        hasJoinedLobby = false;
    }
    
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        hasJoinedLobby = true;
        cachedRoomList.Clear();
        ShowLobbyPanel();
    }
    
    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        hasJoinedLobby = false;
        cachedRoomList.Clear();
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                cachedRoomList[room.Name] = room;
            }
        }
        
        UpdateRoomList();
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        ShowRoomPanel();
        UpdatePlayerList();
    }
    
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        ShowLobbyPanel();
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room: " + newPlayer.NickName);
        UpdatePlayerList();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player Left Room: " + otherPlayer.NickName);
        UpdatePlayerList();
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("New Master Client: " + newMasterClient.NickName);
        UpdatePlayerList();
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Create Room Failed: {message} ({returnCode})");
        ShowErrorPanel($"Failed to create room: {message}");
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message} ({returnCode})");
        ShowErrorPanel($"Failed to join room: {message}");
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Random Room Failed: {message} ({returnCode})");
        ShowErrorPanel($"Failed to join a random room: {message}");
    }
    
    #endregion
}

/// <summary>
/// UI component for displaying room information in the room list
/// </summary>
public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button joinButton;
    
    private string roomName;
    
    public void Setup(string name, int playerCount, int maxPlayers)
    {
        roomName = name;
        
        if (roomNameText != null)
        {
            roomNameText.text = name;
        }
        
        if (playerCountText != null)
        {
            playerCountText.text = $"{playerCount}/{maxPlayers}";
        }
        
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinButtonClicked);
        }
    }
    
    private void OnJoinButtonClicked()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.JoinRoom(roomName);
        }
    }
    
    private void OnDestroy()
    {
        if (joinButton != null)
        {
            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
        }
    }
}

/// <summary>
/// UI component for displaying player information in the player list
/// </summary>
public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject masterClientIndicator;
    
    public void Setup(string name, int actorNumber, bool isLocalPlayer)
    {
        if (playerNameText != null)
        {
            playerNameText.text = name + (isLocalPlayer ? " (You)" : "");
        }
        
        if (masterClientIndicator != null)
        {
            masterClientIndicator.SetActive(PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).IsMasterClient);
        }
    }
} 