using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum WeaponShootType
{
    Manual,
    Automatic,
    Charge,
}

[System.Serializable]
public struct CrosshairData
{
    [Tooltip("The image that will be used for this weapon's crosshair")]
    public Sprite CrosshairSprite;

    [Tooltip("The size of the crosshair image")]
    public int CrosshairSize;

    [Tooltip("The color of the crosshair image")]
    public Color CrosshairColor;
}


[RequireComponent(typeof(AudioSource))]
public class WeaponControler_GMTK : MonoBehaviour
{
    #region VARS

    #region weapons settings
    [Header("Information")]
    [Tooltip("The name that will be displayed in the UI for this weapon")]
    public string WeaponName;

    [Tooltip("The image that will be displayed in the UI for this weapon")]
    public Sprite WeaponIcon;

    [Tooltip("Default data for the crosshair")]
    public CrosshairData CrosshairDataDefault;

    [Tooltip("Data for the crosshair when targeting an enemy")]
    public CrosshairData CrosshairDataTargetInSight;

    [Header("Internal References")]
    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject WeaponRoot;

    [Tooltip("Tip of the weapon, where the projectiles are shot")]
    public Transform WeaponMuzzle;

    [Header("Shoot Parameters")]
    [Tooltip("The type of weapon wil affect how it shoots")]
    public WeaponShootType ShootType;

    // [Tooltip("The projectile prefab")] 
    // public ProjectileBase ProjectilePrefab;

    [Tooltip("Minimum duration between two shots")]
    public float DelayBetweenShots = 0.5f;

    [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
    public float BulletSpreadAngle = 0f;

    [Tooltip("Amount of bullets per shot")]
    public int BulletsPerShot = 1;

    [Tooltip("Force that will push back the weapon after each shot")] [Range(0f, 2f)]
    public float RecoilForce = 1;

    [Tooltip("Maximum amount of ammo in the gun")]
    public int MaxAmmo = 8;
    #endregion

    #region Graphic feedback
    [Header("Audio & Visual")]
    [Tooltip("Optional weapon animator for OnShoot animations")]
    public Animator WeaponAnimator;

    [Tooltip("Prefab of the muzzle flash")]
    public GameObject MuzzleFlashPrefab;

    [Tooltip("Unparent the muzzle flash instance on spawn")]
    public bool UnparentMuzzleFlash;
    #endregion

    #region Audio feedback
    [Tooltip("sound played when shooting")]
    public AudioClip ShootSfx;

    [Tooltip("Sound played when changing to this weapon")]
    public AudioClip ChangeWeaponSfx;

    [Tooltip("Continuous Shooting Sound")] public bool UseContinuousShootSound = false;
    public AudioClip ContinuousShootStartSfx;
    public AudioClip ContinuousShootLoopSfx;
    public AudioClip ContinuousShootEndSfx;
    AudioSource m_ContinuousShootAudioSource = null;
    bool m_WantsToShoot = false;
    #endregion

    #region variables not exposed
    //public UnityAction OnShoot;
    //public event Action OnShootProcessed;
    int m_CurrentAmmo;
    float m_LastTimeShot = Mathf.NegativeInfinity;
    public float LastChargeTriggerTimestamp { get; private set; }
    Vector3 m_LastMuzzlePosition;

    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public float CurrentAmmoRatio { get; private set; }
    public bool IsWeaponActive { get; private set; }
    public bool IsCooling { get; private set; }
    public float CurrentCharge { get; private set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }
    #endregion

    #region getters
    public int GetCurrentAmmo() => Mathf.FloorToInt(m_CurrentAmmo);
    #endregion

    AudioSource m_ShootAudioSource;

    const string k_AnimAttackParameter = "Attack";

    private Queue<Rigidbody> m_PhysicalAmmoPool;
    #endregion

    #region METHODS
    private void Awake()
    {
        m_CurrentAmmo = MaxAmmo;
        m_LastMuzzlePosition = WeaponMuzzle.position;

        m_ShootAudioSource = GetComponent<AudioSource>();

        if (UseContinuousShootSound)
        {
            m_ContinuousShootAudioSource = gameObject.AddComponent<AudioSource>();
            m_ContinuousShootAudioSource.playOnAwake = false;
            m_ContinuousShootAudioSource.clip = ContinuousShootLoopSfx;
            m_ContinuousShootAudioSource.loop = true;
        }
    }

    void ShootShell()
    {
        Rigidbody nextShell = m_PhysicalAmmoPool.Dequeue();

        nextShell.gameObject.SetActive(true);
        nextShell.transform.SetParent(null);
        nextShell.collisionDetectionMode = CollisionDetectionMode.Continuous;

        m_PhysicalAmmoPool.Enqueue(nextShell);
    }

    //void PlaySFX(AudioClip sfx) => AudioUtility.CreateSFX(sfx, transform.position, AudioUtility.AudioGroups.WeaponShoot, 0.0f);

    void Update()
    {
        UpdateAmmo();
        UpdateContinuousShootSound();

        if (Time.deltaTime > 0)
        {
            MuzzleWorldVelocity = (WeaponMuzzle.position - m_LastMuzzlePosition) / Time.deltaTime;
            m_LastMuzzlePosition = WeaponMuzzle.position;
        }
    }

    void UpdateAmmo()
    {
        if (MaxAmmo == Mathf.Infinity)
        {
            CurrentAmmoRatio = 1f;
        }
        else
        {
            CurrentAmmoRatio = m_CurrentAmmo / MaxAmmo;
        }
    }

    void UpdateContinuousShootSound()
    {
        if (UseContinuousShootSound)
        {
            if (m_WantsToShoot && m_CurrentAmmo >= 1f)
            {
                if (!m_ContinuousShootAudioSource.isPlaying)
                {
                    m_ShootAudioSource.PlayOneShot(ShootSfx);
                    m_ShootAudioSource.PlayOneShot(ContinuousShootStartSfx);
                    m_ContinuousShootAudioSource.Play();
                }
            }
            else if (m_ContinuousShootAudioSource.isPlaying)
            {
                m_ShootAudioSource.PlayOneShot(ContinuousShootEndSfx);
                m_ContinuousShootAudioSource.Stop();
            }
        }
    }

    public void ShowWeapon(bool show)
    {
        WeaponRoot.SetActive(show);

        if (show && ChangeWeaponSfx)
        {
            m_ShootAudioSource.PlayOneShot(ChangeWeaponSfx);
        }

        IsWeaponActive = show;
    }

    public void UseAmmo(float amount)
    {
        m_CurrentAmmo = (int)Mathf.Clamp(m_CurrentAmmo - amount, 0f, MaxAmmo);
        m_LastTimeShot = Time.time;
    }

    public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
    {
        m_WantsToShoot = inputDown || inputHeld;
        switch (ShootType)
        {
            case WeaponShootType.Manual:
                if (inputDown)
                {
                    return TryShoot();
                }

                return false;

            case WeaponShootType.Automatic:
                if (inputHeld)
                {
                    return TryShoot();
                }

                return false;

            default:
                return false;
        }
    }

    bool TryShoot()
    {
        if (m_CurrentAmmo >= 1
            && m_LastTimeShot + DelayBetweenShots < Time.time)
        {
            HandleShoot();
            m_CurrentAmmo -= 1;

            return true;
        }
        return false;
    }

    void HandleShoot()
    {
        int bulletsPerShotFinal = ShootType == WeaponShootType.Charge
            ? Mathf.CeilToInt(CurrentCharge * BulletsPerShot)
            : BulletsPerShot;

        // spawn all bullets with random direction
        for (int i = 0; i < bulletsPerShotFinal; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
            //ProjectileBase newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position,
            //    Quaternion.LookRotation(shotDirection));
            //newProjectile.Shoot(this);
        }

        // muzzle flash
        if (MuzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position,
                WeaponMuzzle.rotation, WeaponMuzzle.transform);
            // Unparent the muzzleFlashInstance
            if (UnparentMuzzleFlash)
            {
                muzzleFlashInstance.transform.SetParent(null);
            }

            Destroy(muzzleFlashInstance, 2f);
        }

        m_LastTimeShot = Time.time;

        // play shoot SFX
        if (ShootSfx && !UseContinuousShootSound)
        {
            m_ShootAudioSource.PlayOneShot(ShootSfx);
        }

        // Trigger attack animation if there is any
        if (WeaponAnimator)
        {
            WeaponAnimator.SetTrigger(k_AnimAttackParameter);
        }

        //OnShoot?.Invoke();
        //OnShootProcessed?.Invoke();
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = BulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);

        return spreadWorldDirection;
    }
    #endregion
}
