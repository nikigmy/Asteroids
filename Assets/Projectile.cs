using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Defines.ProjectileMode mode;

    public Defines.ProjectileMode Mode
    {
        get { return mode;}
        protected set { mode = value; }
    }
    public virtual void Init(Defines.ProjectileMode mode)
    {
        this.Mode = mode;
    }
}
