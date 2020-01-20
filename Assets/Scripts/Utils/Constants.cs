namespace Utils
{
    public static class Constants
    {
        public class Tags
        {
            public const string cPlayer = "Player";
            public const string cBoundary = "Boundary";
            public const string cProjectile = "Projectile";
            public const string cAsteroid = "Asteroid";
            public const string cEnemy = "Enemy";
            public const string cPickup = "Pickup";
        }

        /// <summary>
        /// Resource paths of prefabs
        /// </summary>
        public class ResourcePaths
        {
            public const string cUiHearthPrefabPath = "Prefabs/UI/Heart";
            public const string cAsteroidPrefabPath = "Prefabs/Gameplay/Asteroid";
            public const string cBulletPrefabPath = "Prefabs/Gameplay/Bullet";
            public const string cFlyingSaucerPrefabPath = "Prefabs/Gameplay/FlyingSaucer";
            public const string cShipPrefabPath = "Prefabs/Gameplay/Ship";
            public const string cShieldPickup = "Prefabs/Gameplay/Pickups/ShieldPickup";
            public const string cSpeedBoostPickup = "Prefabs/Gameplay/Pickups/SpeedBoostPickup";
            public const string cHomingAmmoPickup = "Prefabs/Gameplay/Pickups/HomingAmmoPickup";

            public const string cLevelsFolder = "Levels";
        }

        /// <summary>
        /// Keys for the pool manager
        /// </summary>
        public class PoolKeys
        {
            public const string cHeart = "Heart";
            public const string cShieldPickup = "ShieldPickup";
            public const string cSpeedBoostPickup = "SpeedBoostPickup";
            public const string cHomingAmmoPickup = "HomingAmmoPickup";
        }

        /// <summary>
        /// Keys for player prefs
        /// </summary>
        public class SaveKeys
        {
            public const string cHighScore = "HighScore";
            public const string cMusicVolume = "MusicVolume";
            public const string cSfxVolume = "SfxVolume";
        }

        /// <summary>
        /// Keys of audio dictionary
        /// </summary>
        public class AudioKeys
        {
            public const string cLazerBlast = "LazerBlast";
            public const string cExplosion = "Explosion";
            public const string cPickup = "Pickup";
            public const string cMenuMusic = "MenuMusic";
            public const string cGameMusic = "GameMusic";
            public const string cGameOver = "GameOver";
            public const string cGameComplete = "GameComplete";
        }
    }
}