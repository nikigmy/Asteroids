using UnityEngine;

public enum ProjectileMode
{
    Friendly,
    Enemy
}

public enum ProjectileType
{
    Normal,
    Homing
}

/// <summary>
/// Base behaviour for projectiles
/// </summary>

public abstract class Projectile : MonoBehaviour
{
    public ProjectileMode Mode
    {
        get => mMode;
        protected set => mMode = value;
    }

    public ProjectileType Type
    {
        get => mType;
        protected set => mType = value;
    }

    /// <summary>
    /// Initialise the projectile
    /// </summary>
    /// <param name="mode">Mode of the projectile</param>
    /// <param name="type">Type of the projectile</param>
    public virtual void Init(ProjectileMode mode, ProjectileType type)
    {
        Mode = mode;
        Type = type;
    }

    private ProjectileMode mMode;
    private ProjectileType mType;
}