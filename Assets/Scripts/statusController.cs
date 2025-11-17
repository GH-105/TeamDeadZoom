using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class statusController : MonoBehaviour
{
    [Serializable]
    public class RuntimeEffect
    {
        public DamageEffects def;
        public float magnitude;
        public int stacks = 1;
        public float timeLeft;
        public float tickTimer;
        public GameObject source;
        public float baseHitDamage;
    }

    private IStatusDamageReceiver recv;

    private void Awake()
    {
        recv = GetComponent<IStatusDamageReceiver>();
    }

    public IStatusDamageReceiver Receiver => recv;

    readonly List<RuntimeEffect> active = new();
    private readonly List<DamageEffects> aggregateKeys = new();
    private readonly List<RuntimeEffect> aggregateBucket = new();
    readonly Dictionary<DamageEffects, RuntimeEffect> singletons = new();
    readonly Dictionary<DamageEffects, float> aggregateTimers = new();

    public void ApplyEffect(DamageEffects def, in DamageContext context, float magnitude)
    {
        switch (def.Policy)
        {
            case DamageEffects.StackPolicy.IndependentStacks:
                {
                    var instance = NewInstance(def, context, magnitude);
                    active.Add(instance);
                    RegisterAggregateTimers(def);
                    def.OnApplied(this, instance);
                    break;
                }

            case DamageEffects.StackPolicy.ReplaceIfStronger:
                {
                    if (singletons.TryGetValue(def, out var existing))
                    {
                        if (magnitude > existing.magnitude)
                        {
                            existing.magnitude = magnitude;
                            existing.timeLeft = def.Duration;
                            existing.baseHitDamage = context.baseHitDamage;
                            existing.source = context.source;
                        }
                    }
                    else
                    {
                        var instance = NewInstance(def, context, magnitude);
                        active.Add(instance);
                        singletons[def] = instance;
                        RegisterAggregateTimers(def);
                        def.OnApplied(this, instance);
                    }
                    break;
                }

            case DamageEffects.StackPolicy.StackAndRefresh:
                {
                    if (singletons.TryGetValue(def, out var rt))
                    {
                        rt.stacks = Mathf.Min(rt.stacks + 1, def.MaxStacks);
                        rt.magnitude = (def.Combine == DamageEffects.CombineMode.Add)
                            ? (rt.magnitude + magnitude)
                            : Mathf.Max(rt.magnitude, magnitude);
                        rt.timeLeft = def.Duration;
                    }
                    else
                    {
                        var instance = NewInstance(def, context, magnitude);
                        active.Add(instance);
                        singletons[def] = instance;
                        RegisterAggregateTimers(def);
                        def.OnApplied(this, instance);
                    }
                    break;
                }
        }
    }

    static RuntimeEffect NewInstance(DamageEffects def, in DamageContext context, float magnitude) => new RuntimeEffect
    {
        def = def,
        magnitude = magnitude,
        stacks = 1,
        timeLeft = def.Duration,
        tickTimer = def.TickInterval,
        source = context.source,
        baseHitDamage = context.baseHitDamage
    };

    public void ClearAllEffects()
    {
        active.Clear();
        singletons.Clear();
        aggregateTimers.Clear();
    }

    private void OnDisable()
    {
        ClearAllEffects();
    }

    private void OnDestroy()
    {
        ClearAllEffects();
    }

    private void Update()
    {
        if (recv == null || !isActiveAndEnabled) return;

        float time = Time.deltaTime;

        for (int i = active.Count - 1; i >= 0; i--)
        {
            var rt = active[i];
            rt.timeLeft -= time;

            if (!rt.def.AggregateTicks)
                rt.tickTimer -= time;

            if (rt.timeLeft <= 0f)
            {
                rt.def.OnExpired(this, rt);
                if (singletons.TryGetValue(rt.def, out var only) && only == rt)
                    singletons.Remove(rt.def);
                active.RemoveAt(i);

                RemoveAggregateTimerIfUnused(rt.def);
            }
        }

        for (int i = 0; i < active.Count; i++)
        {
            var rt = active[i];
            if (rt.def.AggregateTicks) continue;
            if (rt.tickTimer <= 0f)
            {
                rt.tickTimer += Mathf.Max(0.001f, rt.def.TickInterval);
                rt.def.OnTick(this, rt);
            }
        }

        if (aggregateTimers.Count > 0)
        {
            aggregateKeys.Clear();
            foreach (var k in aggregateTimers.Keys)
                aggregateKeys.Add(k);

            for (int i = 0; i < aggregateKeys.Count;i++)
            {
                var def = aggregateKeys[i];

                float t = aggregateTimers[def] - time;

                while (t <= 0f)
                {
                    // TEMP
                    t += Mathf.Max(0.0001f, def.TickInterval);

                    aggregateBucket.Clear();
                    for (int j = 0; j < active.Count; j++)
                    {
                        var rt = active[j];
                        if (rt.def == def)
                            aggregateBucket.Add(rt);
                    }
                    if (aggregateBucket.Count > 0)
                        def.OnAggregateTick(this, aggregateBucket);
                }

                aggregateTimers[def] = t;
            }

            
        }
    }

    void RegisterAggregateTimers(DamageEffects def)
    {
        if (def.AggregateTicks && !aggregateTimers.ContainsKey(def))
            aggregateTimers[def] = def.TickInterval;
    }

    void RemoveAggregateTimerIfUnused(DamageEffects def)
    {
        if (!def.AggregateTicks) return;

        for (int i = 0; i < active.Count; i++)
        {
            if (active[i].def == def)
                return;   
        }
        aggregateTimers.Remove(def);
    }
}
