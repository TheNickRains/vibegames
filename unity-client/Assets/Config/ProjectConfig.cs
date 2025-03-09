using UnityEngine;

/// <summary>
/// Contains configuration settings for the entire project.
/// Lists all required packages and dependencies.
/// </summary>
public static class ProjectConfig
{
    // Project version
    public const string VERSION = "0.1.0";
    
    // Required Unity packages (dependencies)
    public static readonly string[] RequiredPackages = new string[]
    {
        "com.unity.render-pipelines.high-definition", // HDRP for realistic graphics
        "com.unity.netcode.gameobjects", // Unity Networking
        "com.unity.inputsystem", // New Input System
        "com.unity.cinemachine", // Camera system
        "com.unity.postprocessing", // Post-processing effects
        "com.unity.textmeshpro", // Text rendering
        "com.unity.animation.rigging", // Animation rigging
        "com.unity.ai.navigation", // Navigation system
        "com.unity.nuget.newtonsoft-json", // JSON parsing
    };
    
    // Third-party packages
    public static readonly string[] ThirdPartyPackages = new string[]
    {
        "Photon PUN 2", // Photon networking
        "DOTween", // Animation library
        "Amplify Shader Editor", // Shader creation (optional)
        "Ultimate FPS Counter", // Performance monitoring (development only)
    };
    
    // Graphics settings
    public static class Graphics
    {
        public const RenderPipelineType PipelineType = RenderPipelineType.HDRP;
        public const int TargetFrameRate = 60;
        public const bool UseVSync = true;
        public const bool UseAntiAliasing = true;
        public const bool UseAmbientOcclusion = true;
        public const bool UseReflections = true;
        
        public enum RenderPipelineType
        {
            BuiltIn,
            URP,
            HDRP
        }
    }
    
    // Physics settings
    public static class Physics
    {
        public const float Gravity = -9.81f;
        public const int SolverIterations = 6;
        public const int SolverVelocityIterations = 1;
        public const bool UseFixedTimestep = true;
        public const float FixedTimestep = 0.02f; // 50 Hz
    }
    
    // Network settings
    public static class Network
    {
        public const string PhotonAppID = "your-photon-app-id";
        public const string Region = "us";
        public const int SendRate = 30;
        public const int SerializationRate = 30;
        public const bool UseEncryption = true;
    }
    
    // Audio settings
    public static class Audio
    {
        public const int MaxChannels = 32;
        public const bool Use3DAudio = true;
        public const float MasterVolume = 1.0f;
        public const float MusicVolume = 0.8f;
        public const float SFXVolume = 1.0f;
        public const float VoiceVolume = 1.0f;
    }
    
    // Input settings
    public static class Input
    {
        public const float MouseSensitivity = 1.0f;
        public const bool InvertYAxis = false;
        public const bool UseGamepad = true;
        public const float GamepadDeadzone = 0.19f;
    }
    
    // Player settings
    public static class Player
    {
        public const float WalkSpeed = 5.0f;
        public const float RunSpeed = 10.0f;
        public const float JumpForce = 7.0f;
        public const float GravityMultiplier = 2.5f;
        public const float InteractDistance = 2.5f;
        public const float MaxGrabDistance = 5.0f;
        public const float MaxGrabMass = 50.0f;
    }
    
    // Game mode settings
    public static class GameModes
    {
        public static class HideAndSeek
        {
            public const float RoundTime = 300f; // 5 minutes
            public const float PrepTime = 30f; // 30 seconds
            public const int MinPlayers = 2;
            public const int MaxPlayers = 16;
        }
        
        public static class Infection
        {
            public const float RoundTime = 300f; // 5 minutes
            public const float PrepTime = 15f; // 15 seconds
            public const int MinPlayers = 3;
            public const int MaxPlayers = 16;
            public const float InfectionDistance = 2.0f;
        }
        
        public static class Sandbox
        {
            public const int MaxPlayers = 16;
            public const bool AllowPropSpawning = true;
            public const int MaxPropsPerPlayer = 10;
        }
    }
    
    // Map settings
    public static class Maps
    {
        public static readonly string[] AvailableMaps = new string[]
        {
            "Nuketown", // Based on Call of Duty map
            "Warehouse",
            "Terminal", // Based on Call of Duty map
            "Mansion",
            "Office",
            "Shipment", // Based on Call of Duty map
            "Sandbox",
        };
    }
    
    // Apply settings to Unity project
    public static void ApplySettings()
    {
        // Apply graphics settings
        QualitySettings.vSyncCount = Graphics.UseVSync ? 1 : 0;
        Application.targetFrameRate = Graphics.TargetFrameRate;
        
        // Apply physics settings
        UnityEngine.Physics.gravity = new Vector3(0, Physics.Gravity, 0);
        UnityEngine.Physics.defaultSolverIterations = Physics.SolverIterations;
        UnityEngine.Physics.defaultSolverVelocityIterations = Physics.SolverVelocityIterations;
        Time.fixedDeltaTime = Physics.FixedTimestep;
        
        Debug.Log($"Applied project settings for VibeMod v{VERSION}");
    }
} 