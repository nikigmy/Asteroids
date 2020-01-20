using System;
using Game;
using ObjectGenerators;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Managers
{
    /// <summary>
    /// Main manager of the game
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get { return _instance; }
        }
        
        public Config Config;
        
        public uint CurrentLevelNumber => mCurrentLevelIndex + 1;

        public bool GamePaused { get; protected set; }

        public UIManager UIManager
        {
            get
            {
                return uiManager; 
            }
        }

        public InputManager InputManager
        {
            get
            {
                return inputManager; 
            }
        }

        public PoolManager PoolManager 
        {
            get
            {
                return poolManager; 
            }
        }

        public AudioManager AudioManager
        {
            get
            {
                return audioManager; 
            }
        }

        public LevelManager LevelManager
        {
            get
            {
                return levelManager; 
            }
        }

        private void Awake()
        {
            Debug.Assert(_instance == null);
            if (_instance == null)
            {
                _instance = this;
                
                Debug.Assert(uiManager != null);
                Debug.Assert(inputManager != null);
                Debug.Assert(poolManager != null);
                Debug.Assert(audioManager != null);
                Debug.Assert(levelManager != null);
            }
            else
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            mFsm = new FSM<GameFSMDescriptor>();
            mFsm.AddStateBinding(GameFSMDescriptor.State.Initialising, StartInitialisation,  null);
            mFsm.AddStateBinding(GameFSMDescriptor.State.MainMenu, OpenMainMenu,  CloseMainMenu);
            mFsm.AddStateBinding(GameFSMDescriptor.State.Game, StartGameState,  EndGame);
            mFsm.AddStateBinding(GameFSMDescriptor.State.GameOver, StartGameOver,  HideEndGameScreen);
            mFsm.AddStateBinding(GameFSMDescriptor.State.GameCompleted, StartGameComplete,  HideEndGameScreen);

            mFsm.Start(GameFSMDescriptor.State.Initialising);
        }

        #region Public

        public void LoadMainMenu()
        {
            mFsm.ExecuteAction(GameFSMDescriptor.Action.ViewMainMenu);
        }

        public void StartGame()
        {
            mFsm.ExecuteAction(GameFSMDescriptor.Action.StartGame);
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            GamePaused = true;
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
            GamePaused = false;
        }

        #endregion

        #region FSM Handlers
        private void StartInitialisation()
        {
            var levelsObjs = Resources.LoadAll(Constants.ResourcePaths.cLevelsFolder, typeof(LevelData));
            levels = new LevelData[levelsObjs.Length];
            for (int i = 0; i < levelsObjs.Length; i++)
            {
                levels[i] = (LevelData) levelsObjs[i];
            }

            CreatePoolManager();
            audioManager.Init(poolManager);
            uiManager.Init();

            inputManager.Init(Config.ControlScheme);
            inputManager.DisableVisuals();
            levelManager.Init(poolManager, audioManager, Config);
            levelManager.OnGameOver += OnGameOver;
            levelManager.OnLevelComplete += OnLevelCompleted;

            mFsm.ExecuteAction(GameFSMDescriptor.Action.ViewMainMenu);
        }
        
        private void OpenMainMenu()
        {
            uiManager.ShowMainMenu();

            mCurrentLevelIndex = 0;
            levelManager.LoadLevel(levels[mCurrentLevelIndex]);
        }

        private void CloseMainMenu()
        {
            uiManager.HideMainMenu();
        }

        private void StartGameState()
        {
            if (!mFsm.WasInState(GameFSMDescriptor.State.MainMenu))
            {
                mCurrentLevelIndex = 0;
                levelManager.LoadLevel(levels[mCurrentLevelIndex]);
            }

            levelManager.StartLevel(Config.playerData, Config.flyingSaucerData);

            uiManager.ShowGameUI(levelManager);
            inputManager.EnableVisuals();
        }

        private void EndGame()
        {
            inputManager.DisableVisuals();
            levelManager.Clear();
            uiManager.HideGameUI();
        }

        private void StartGameOver()
        {
            var newHighScore = UpdateHighScore(levelManager.Score);
            uiManager.ShowGameEndScreen(levelManager.Score, false, newHighScore);
        }
        
        private void StartGameComplete()
        {
            var newHighScore = UpdateHighScore(levelManager.Score);
            uiManager.ShowGameEndScreen(levelManager.Score, true, newHighScore);
        }
        
        private void HideEndGameScreen()
        {
            uiManager.HideGameEndScreen();
        }

        #endregion

        #region Event Handlers

        private void OnLevelCompleted(object sender, ValueArgs<(int health, int score)> args)
        {
            if (mCurrentLevelIndex + 1 < levels.Length)
            {
                mCurrentLevelIndex += 1;
                levelManager.Clear();
                levelManager.LoadLevel(levels[mCurrentLevelIndex], args.Value.health, args.Value.score);
                levelManager.StartLevel(Config.playerData, Config.flyingSaucerData);
            }
            else
            {
                mFsm.ExecuteAction(GameFSMDescriptor.Action.StartGameCompletedCeremony);
            }
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            mFsm.ExecuteAction(GameFSMDescriptor.Action.StartGameOverCeremony);
        }

        #endregion
        
        #region Private

        private bool UpdateHighScore(int newScore)
        {
            var highScore = PlayerPrefs.GetInt(Constants.SaveKeys.cHighScore, 0);

            var newHighScore = newScore > highScore;
            if (newHighScore) PlayerPrefs.SetInt(Constants.SaveKeys.cHighScore, newScore);

            return newHighScore;
        }
        
        private void CreatePoolManager()
        {
            poolManager.RegisterGenerator(
                new GenericObjectGenerator<PlayerController>(Constants.ResourcePaths.cShipPrefabPath));
            poolManager.PopulatePool<PlayerController>(1);
            poolManager.RegisterGenerator(
                new GenericObjectGenerator<FlyingSaucer>(Constants.ResourcePaths.cFlyingSaucerPrefabPath));

            poolManager.RegisterGenerator(Constants.PoolKeys.cHeart,
                new ObjectGenerator(Constants.ResourcePaths.cUiHearthPrefabPath));
            poolManager.PopulatePool<GameObject>(Constants.PoolKeys.cHeart, _instance.Config.playerStartHealth);

            poolManager.RegisterGenerator(Constants.PoolKeys.cShieldPickup,
                new GenericObjectGenerator<Pickup>(Constants.ResourcePaths.cShieldPickup));
            poolManager.RegisterGenerator(Constants.PoolKeys.cHomingAmmoPickup,
                new GenericObjectGenerator<Pickup>(Constants.ResourcePaths.cHomingAmmoPickup));
            poolManager.RegisterGenerator(Constants.PoolKeys.cSpeedBoostPickup,
                new GenericObjectGenerator<Pickup>(Constants.ResourcePaths.cSpeedBoostPickup));

            poolManager.RegisterGenerator(
                new GenericObjectGenerator<BulletController>(Constants.ResourcePaths.cBulletPrefabPath));
            poolManager.PopulatePool<BulletController>(_instance.Config.bulletPool);

            poolManager.RegisterGenerator(new AsteroidGenerator(Constants.ResourcePaths.cAsteroidPrefabPath));
            poolManager.PopulatePool<Asteroid>(_instance.Config.minAsteroidPool);

            poolManager.RegisterGenerator(new GenericObjectGenerator<AudioSource>());
            poolManager.PopulatePool<AudioSource>(_instance.Config.audioSourcePool);
        }
        
        #endregion
        
        private uint mCurrentLevelIndex;

        private FSM<GameFSMDescriptor> mFsm = new FSM<GameFSMDescriptor>();

        private LevelData[] levels;

        [SerializeField]
        private UIManager uiManager;

        [SerializeField]
        private InputManager inputManager;

        [SerializeField]
        private PoolManager poolManager;

        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private LevelManager levelManager;
    }
}