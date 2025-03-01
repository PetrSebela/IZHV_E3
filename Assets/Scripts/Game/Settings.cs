using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// ReSharper disable InvalidXmlDocComment

/// <summary>
/// Container for settings used in the game.
/// </summary>
public class Settings : MonoBehaviour
{
    // References to GameObjects within the scene.
    [ Header("Game Objects") ]

    /// <summary>
    /// Reference to the main game manager.
    /// </summary>
    public GameObject gameManager;
    
    /// <summary>
    /// Prefab used for each player.
    /// </summary>
    public GameObject playerPrefab = null;
    
    /// <summary>
    /// List of all available players.
    /// </summary>
    public List<GameObject> players = new List<GameObject>();

    /// <summary>
    /// Main camera used for viewing the scene.
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// List of all available UIs for players.
    /// </summary>
    public List<GameObject> playerUIList;

    /// <summary>
    /// Root of the developer UI.
    /// </summary>
    public GameObject devUI;

    /// <summary>
    /// Root of the Virtual GamePad UI.
    /// </summary>
    public GameObject gamepadUI;
    
    [Header("Control Settings")]
    /// <summary>
    /// Attenuation for the smooth zoom.
    /// </summary>
    public float zoomAttune = 0.6f;
    
    /// <summary>
    /// Cutoff value for the smooth zoom.
    /// </summary>
    public float zoomCutoff = 0.1f;
    
    // Game Settings
    [Header("Game Settings")] 
    
    /// <summary>
    /// Use the Entity-Component-System?.
    /// </summary>
    [HideInInspector]
    public bool useECS = false;

    /// <summary>
    /// Use true ECS physics (true) or a radius-based simplification (false)?
    /// </summary>
    [HideInInspector]
    public bool useECSPhysics = false;

    /// <summary>
    /// Radius used for the collisions in our simplified physics.
    /// </summary>
    [HideInInspector]
    public float ecsSimplePhysicsRadius = 1.5f;
    
    // World Settings
    [ Header("World Settings") ]
    
    /// <summary>
    /// Mask for the objects in the ground layer.
    /// </summary>
    public LayerMask groundLayer;

    /// <summary>
    /// Currently used GameManager component.
    /// </summary>
    [CanBeNull] 
    private GameManager mGameManager;
    
    /// <summary>
    /// Singleton instance of the Settings.
    /// </summary>
    private static Settings sInstance;
    
    [SerializeField] public PlayerInputManager PlayerInputManager;

    /// <summary>
    /// Getter for the singleton Settings object.
    /// </summary>
    public static Settings Instance
    { get { return sInstance; } }
    
    /*
     * Task #3: Implement the local multiplayer
     * Useful functions and variables:
     *  - Events from the Player Input Manager: Joined Event and Left Event
     *  - Registering a new player: AddPlayer(gameObject)
     *  - Removing a registered player: RemovePlayer(gameObject)
     * Add the Player Input Manager to the Settings object, handle the new player registration,
     *   and enable joining of second player using the alternative input scheme.
     * Hint: You could use the "Invoke Unity Events" notification behavior and
     *   create two functions here "OnPlayerJoined" and "OnPlayerLeft", which
     *   can call through to the AddPlayer and RemovePlayer respectively.
     *   Do not forget to set the Joining Behavior and the Player Prefab!
     */
    
    /// <summary>
    /// Called when the script instance is first loaded.
    /// </summary>
    private void Awake()
    {
        // Initialize the singleton instance, if no other exists.
        if (sInstance != null && sInstance != this)
        { Destroy(gameObject); }
        else
        { sInstance = this; }
        
        if (gameManager)
        { mGameManager = gameManager.GetComponent<GameManager>(); }

        PlayerInputManager.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.onPlayerLeft += OnPlayerLeft;
    }
    

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    { }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        AddPlayer(playerInput.gameObject);
        Debug.Log("Player joined");

    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log(playerInput.transform.name);
        RemovePlayer(playerInput.gameObject);
        Debug.Log("Player left");
    }

    /// <summary>
    /// Add a new player to the current player list.
    /// </summary>
    /// <param name="player">The new player.</param>
    /// <returns>Returns index of the player, starting at 0.</returns>
    public int AddPlayer(GameObject player)
    {
        var playerIndex = players.FindIndex(x => x == player);
        if (playerIndex < 0)
        { players.Add(player); playerIndex = players.Count - 1; }
        
        mGameManager?.UpdatePlayers();
        return playerIndex;
    }
    
    /// <summary>
    /// Get index of an already added player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>Returns index of the player, starting at 0. Returns -1
    ///          for players which were not added yet</returns>
    public int GetPlayerIndex(GameObject player)
    { return players.FindIndex(x => x == player); }
    
    /// <summary>
    /// Remove a player from the current player list.
    /// </summary>
    /// <param name="player">The player to be removed.</param>
    public void RemovePlayer(GameObject player)
    {
        players.RemoveAll(x => x == player);
        mGameManager?.UpdatePlayers();
    }

    /// <summary>
    /// Get UI for given player index.
    /// </summary>
    /// <param name="playerIdx">Index of the player.</param>
    /// <returns>Returns UI for specified player. If non exists, returns null.</returns>
    [CanBeNull]
    public GameObject PlayerUI(int playerIdx)
    {
        if (playerIdx >= playerUIList.Count)
        { return null; }

        return playerUIList[playerIdx];
    }
}
