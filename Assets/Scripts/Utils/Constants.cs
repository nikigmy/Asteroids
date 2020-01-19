namespace Utils
{
    public static class Constants
    {
        public class Tags
        {
            public const string player = "Player";
            public const string boundary = "Boundary";
            public const string projectile = "Projectile";
            public const string asteroid = "Asteroid";
            public const string enemy = "Enemy";
            public const string pickup = "Pickup";
        }

        public class ResourcePaths
        {
            public const string uiHearthPrefabPath = "Prefabs/UI/Heart";
            public const string asteroidPrefabPath = "Prefabs/Gameplay/Asteroid";
            public const string bulletPrefabPath = "Prefabs/Gameplay/Bullet";
            public const string flyingSaucerPrefabPath = "Prefabs/Gameplay/FlyingSaucer";
            public const string shipPrefabPath = "Prefabs/Gameplay/Ship";
            public const string shieldPickup = "Prefabs/Gameplay/Pickups/ShieldPickup";
            public const string speedBoostPickup = "Prefabs/Gameplay/Pickups/SpeedBoostPickup";
            public const string homingAmmoPickup = "Prefabs/Gameplay/Pickups/HomingAmmoPickup";

            public const string levelsFolder = "Levels";
        }

        public class PoolKeys
        {
            public const string heart = "Heart";
            public const string shieldPickup = "ShieldPickup";
            public const string speedBoostPickup = "SpeedBoostPickup";
            public const string homingAmmoPickup = "HomingAmmoPickup";
        }

        public class SaveKeys
        {
            public const string highScore = "HighScore";
            public const string musicVolume = "MusicVolume";
            public const string sfxVolume = "SfxVolume";
        }

        public class AudioKeys
        {
            public const string lazerBlast = "LazerBlast";
            public const string explosion = "Explosion";
            public const string pickup = "Pickup";
            public const string menuMusic = "MenuMusic";
            public const string gameMusic = "GameMusic";
            public const string gameOver = "GameOver";
            public const string gameComplete = "GameComplete";
        }
    }
}