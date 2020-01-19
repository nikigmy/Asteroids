using System;
using System.Linq;
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
    public class GameManager : MonoBehaviour
    {
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
                return mUIManager; 
            }
        }

        public InputManager InputManager
        {
            get
            {
                return mInputManager; 
            }
        }

        public PoolManager PoolManager 
        {
            get
            {
                return mPoolManager; 
            }
        }

        public AudioManager AudioManager
        {
            get
            {
                return mAudioManager; 
            }
        }

        public LevelManager LevelManager
        {
            get
            {
                return mLevelManager; 
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
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
            mFsm.AddStateBinding(GameFSMDescriptor.State.Initialising, StartInitialisation, null, null);
            mFsm.AddStateBinding(GameFSMDescriptor.State.MainMenu, OpenMainMenu, null, CloseMainMenu);
            mFsm.AddStateBinding(GameFSMDescriptor.State.Game, StartGameState, null, EndGame);
            mFsm.AddStateBinding(GameFSMDescriptor.State.GameOver, StartGameOver, null, HideEndGameScreen);
            mFsm.AddStateBinding(GameFSMDescriptor.State.GameCompleted, StartGameComplete, null, HideEndGameScreen);

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
            levels = Resources.LoadAll(Constants.ResourcePaths.levelsFolder, typeof(LevelData)).Select(x => x as LevelData)
                .ToArray();

            CreatePoolManager();
            mAudioManager.Init(mPoolManager);
            mUIManager.Init();

            mInputManager.Init(Config.ControlScheme);

            mLevelManager.Init(mPoolManager);
            mLevelManager.OnGameOver += OnGameOver;
            mLevelManager.OnLevelComplete += OnLevelCompleted;

            mFsm.ExecuteAction(GameFSMDescriptor.Action.ViewMainMenu);
        }
        
        private void OpenMainMenu()
        {
            mInputManager.DisableVisuals();

            mUIManager.ShowMainMenu();

            mCurrentLevelIndex = 0;
            mLevelManager.LoadLevel(levels[mCurrentLevelIndex]);
        }

        private void CloseMainMenu()
        {
            mUIManager.HideMainMenu();
        }

        private void StartGameState()
        {
            if (!mFsm.WasInState(GameFSMDescriptor.State.MainMenu))
            {
                mCurrentLevelIndex = 0;
                mLevelManager.LoadLevel(levels[mCurrentLevelIndex]);
            }

            mUIManager.ShowGameUI(mLevelManager);
            mInputManager.EnableVisuals();

            mLevelManager.StartLevel(Config.playerData, Config.flyingSaucerData);
        }

        private void EndGame()
        {
            mLevelManager.Clear();
            mUIManager.HideGameUI();
        }

        private void StartGameOver()
        {
            var newHighScore = UpdateHighScore(mLevelManager.Score);
            mUIManager.ShowGameEndScreen(mLevelManager.Score, true, newHighScore);
        }
        
        private void StartGameComplete()
        {
            var newHighScore = UpdateHighScore(mLevelManager.Score);
            mUIManager.ShowGameEndScreen(mLevelManager.Score, false, newHighScore);
        }
        
        private void HideEndGameScreen()
        {
            mUIManager.HideGameEndScreen();
        }

        #endregion

        #region Event Handlers

        private void OnLevelCompleted(object sender, ValueArgs<(int health, int score)> args)
        {
            if (mCurrentLevelIndex + 1 < levels.Length)
            {
                mCurrentLevelIndex += 1;
                mLevelManager.Clear();
                mLevelManager.LoadLevel(levels[mCurrentLevelIndex], args.Value.health, args.Value.score);
                mLevelManager.StartLevel(Config.playerData, Config.flyingSaucerData);
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
            var highScore = PlayerPrefs.GetInt(Constants.SaveKeys.highScore, 0);

            var newHighScore = newScore > highScore;
            if (newHighScore) PlayerPrefs.SetInt(Constants.SaveKeys.highScore, newScore);

            return newHighScore;
        }
        
        private void CreatePoolManager()
        {
            mPoolManager.RegisterGenerator(
                new GenericObjectGenerator<PlayerController>(Constants.ResourcePaths.shipPrefabPath));
            mPoolManager.RegisterGenerator(
                new GenericObjectGenerator<FlyingSaucer>(Constants.ResourcePaths.flyingSaucerPrefabPath));

            mPoolManager.RegisterGenerator(Constants.PoolKeys.heart,
                new ObjectGenerator(Constants.ResourcePaths.uiHearthPrefabPath));
            mPoolManager.PopulatePool<GameObject>(Constants.PoolKeys.heart, _instance.Config.playerStartHealth);

            mPoolManager.RegisterGenerator(Constants.PoolKeys.shieldPickup,
                new GenericObjectGenerator<Pickup>(Constants.ResourcePaths.shieldPickup));
            mPoolManager.RegisterGenerator(Constants.PoolKeys.homingAmmoPickup,
                new GenericObjectGenerator<Pickup>(Constants.ResourcePaths.homingAmmoPickup));
            mPoolManager.RegisterGenerator(Constants.PoolKeys.speedBoostPickup,
                new GenericObjectGenerator<Pickup>(Constants.ResourcePaths.speedBoostPickup));

            mPoolManager.RegisterGenerator(
                new GenericObjectGenerator<BulletController>(Constants.ResourcePaths.bulletPrefabPath));
            mPoolManager.PopulatePool<BulletController>(_instance.Config.bulletPool);

            mPoolManager.RegisterGenerator(new AsteroidGenerator(Constants.ResourcePaths.asteroidPrefabPath));
            mPoolManager.PopulatePool<Asteroid>(_instance.Config.minAsteroidPool);

            mPoolManager.RegisterGenerator(new GenericObjectGenerator<AudioSource>());
            mPoolManager.PopulatePool<AudioSource>(_instance.Config.audioSourcePool);
        }
        
        #endregion
        
        private uint mCurrentLevelIndex;

        private FSM<GameFSMDescriptor> mFsm = new FSM<GameFSMDescriptor>();

        private LevelData[] levels;

        [SerializeField]
        public UIManager mUIManager;

        [SerializeField]
        private InputManager mInputManager;

        [SerializeField]
        private PoolManager mPoolManager;

        [SerializeField]
        private AudioManager mAudioManager;

        [SerializeField]
        private LevelManager mLevelManager;

        private static GameManager _instance;
        
    }
}