using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DamageEffects", menuName = "Scriptable Objects/DamageEffects")]
public abstract class DamageEffects : ScriptableObject
{
    public enum StackPolicy { IndependentStacks, StackAndRefresh, ReplaceIfStronger }
    public enum CombineMode { Add, Max }
    
    [Header("Common")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private StackPolicy stackPolicy = StackPolicy.IndependentStacks;
    [SerializeField] private CombineMode combineMode = CombineMode.Add;
    [SerializeField] private int maxStacks = 99;
    [SerializeField] private bool aggregateTicks = false;

    public float Duration => duration;
    public float TickInterval => tickInterval;
    public StackPolicy Policy => stackPolicy;
    public CombineMode Combine => combineMode;
    public int MaxStacks => maxStacks;
    public bool AggregateTicks => aggregateTicks;

    public virtual void OnApplied(statusController target, statusController.RuntimeEffect rt) { }
    public virtual void OnTick(statusController target, statusController.RuntimeEffect rt) { }
    public virtual void OnExpired(statusController target, statusController.RuntimeEffect rt) { }

    public virtual void OnAggregateTick(statusController target, List<statusController.RuntimeEffect> instance) { }
}
