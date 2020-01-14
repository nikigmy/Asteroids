using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CollisionArgs = ValueArgs<System.Tuple<UnityEngine.Collider2D, UnityEngine.Collider2D>>;

/// <summary>
/// Responsible for reporting back using events on collisions.
/// </summary>
public class CollisionReporter
    : MonoBehaviour
{
    protected  event EventHandler<CollisionArgs> TriggerEnter;
    protected  event EventHandler<CollisionArgs> TriggerExit;

    private void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D data)
    {
        this.Raise(TriggerEnter, new CollisionArgs(Tuple.Create(GetComponent<Collider2D>(), data)));
    }

    void OnTriggerExit2D(Collider2D data)
    {
        this.Raise(TriggerExit, new CollisionArgs(Tuple.Create(GetComponent<Collider2D>(), data)));
    }

    public CollisionReporter CallOnEnter(EventHandler<CollisionArgs> handler)
    {
        TriggerEnter += handler;
        return this;
    }

    public CollisionReporter CallOnExit(EventHandler<CollisionArgs> handler)
    {
        TriggerExit += handler;
        return this;
    }

    public CollisionReporter UnsubscribeOnEnter(EventHandler<CollisionArgs> handler)
    {
        TriggerEnter -= handler;
        return this;
    }

    public CollisionReporter UnsubscribeOnExit(EventHandler<CollisionArgs> handler)
    {
        TriggerExit -= handler;
        return this;
    }  
}
