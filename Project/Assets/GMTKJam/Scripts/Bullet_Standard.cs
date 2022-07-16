using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Standard : Bullet
{
    #region VARS
    [Header("General")]
    [Tooltip("Radius of this projectile's collision detection")]
    public float Radius = 0.01f;

    [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
    public Transform Root;

    [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
    public Transform Tip;

    [Tooltip("LifeTime of the projectile")]
    public float MaxLifeTime = 5f;

    [Tooltip("VFX prefab to spawn upon impact")]
    public GameObject ImpactVfx;

    [Tooltip("LifeTime of the VFX before being destroyed")]
    public float ImpactVfxLifetime = 5f;

    [Tooltip("Offset along the hit normal where the VFX will be spawned")]
    public float ImpactVfxSpawnOffset = 0.1f;

    [Tooltip("Clip to play on impact")]
    public AudioClip ImpactSfxClip;

    [Tooltip("Layers this projectile can collide with")]
    public LayerMask HittableLayers = -1;

    [Header("Movement")]
    [Tooltip("Speed of the projectile")]
    public float Speed = 20f;

    [Tooltip("Downward acceleration from gravity")]
    public float GravityDownAcceleration = 0f;

    [Tooltip(
        "Distance over which the projectile will correct its course to fit the intended trajectory (used to drift projectiles towards center of screen in First Person view). At values under 0, there is no correction")]
    public float TrajectoryCorrectionDistance = -1;

    [Tooltip("Determines if the projectile inherits the velocity that the weapon's muzzle had when firing")]
    public bool InheritWeaponVelocity = false;

    [Header("Damage")]
    [Tooltip("Damage of the projectile")]
    public float Damage = 40f;

    //[Tooltip("Area of damage. Keep empty if you don<t want area damage")]
    //public DamageArea AreaOfDamage;

    [Header("Debug")]
    [Tooltip("Color of the projectile radius debug view")]
    public Color RadiusColor = Color.cyan * 0.2f;

    //ProjectileBase m_ProjectileBase;
    Vector3 m_LastRootPosition;
    Vector3 m_Velocity;
    bool m_HasTrajectoryOverride;
    float m_ShootTime;
    Vector3 m_TrajectoryCorrectionVector;
    Vector3 m_ConsumedTrajectoryCorrectionVector;
    List<Collider> m_IgnoredColliders;
    #endregion
}
