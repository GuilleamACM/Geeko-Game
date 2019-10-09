﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class OnCollisionData
{
    public OnHitBehavior m_HitBehavior;
    public ApplyDamageData m_ApplyDamage;
    public DestroySelfData m_DestroySelfData;
}

[Serializable]
public class ApplyDamageData
{
    public float m_Damage = 10.0f;
    public GameplayStatics.DamageType m_DamageType = GameplayStatics.DamageType.Normal;
    public List<string> m_ValidHitActors = SpellUtilities.invalid2;
}

[Serializable]
public class DestroySelfData
{
    public List<string> m_ValidHitActors = SpellUtilities.entities;
}

[System.Serializable]
public abstract class OnHitBehavior : ScriptableObject
{
    public abstract void OnTriggerHitEvent(Collider2D collision, GameObject src, OnCollisionData data);
    public abstract void OnCollisionHitEvent(Collision2D collision, GameObject src, OnCollisionData data);
}