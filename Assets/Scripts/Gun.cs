using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using UnityEngine.UI;

public class Gun : Singleton<Gun>
{
    [Header("Aiming")]
    [SerializeField] private Transform AimDownSightPosition;
    [SerializeField] private Transform HoldPosition;
    [SerializeField] private Transform ShotPosition;
    [SerializeField] private float AimSpeed;
    [SerializeField] bool Aiming;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera PlayerCam;
    public float normalFov;
    public float aimFov;

    [Header("Shooting")]
    public int bulletForce;
    public float shakeIntensity;
    public float shakeTime;
    public float shootDelay = 1f;
    private float timeSinceLastShot;
    public GameObject bulletPrefab;

    [Header("Mags&Reload")]
    public KeyCode ReloadKey = KeyCode.R;
    public int magSize;
    public int numOfMags = 2;
    public int currentNumOfBulletInMag { get; set; }
    private int reloadableBullets;
    private int availableBullets;
    [SerializeField] bool Reloading;
    [Space(8f)]
    public Text AmmoInMag;
    public Text ReloadAmmo;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("VFX&SFX")]
    private GameObject model;
    private GameObject slide;

    [SerializeField] private GameObject spark;
    [SerializeField] private GameObject smoke;
    [SerializeField] private AudioSource shot;
 
    void Awake()
    {
        availableBullets = magSize * numOfMags;
        currentNumOfBulletInMag = magSize;
        reloadableBullets = availableBullets - currentNumOfBulletInMag;
        
    }
    void Start()
    {
        transform.position = HoldPosition.position;
        model = animator.gameObject;
        slide = model.transform.GetChild(11).gameObject;
    }
    // Update is called once per frame
    private void Update()
    {
        AmmoInMag.text = currentNumOfBulletInMag.ToString();
        ReloadAmmo.text = reloadableBullets.ToString();

        if (Input.GetMouseButton(1) && !Reloading)
        {
            transform.position = Vector3.Lerp(transform.position, AimDownSightPosition.position, AimSpeed * Time.deltaTime);
            PlayerCam.m_Lens.FieldOfView = Mathf.Lerp(PlayerCam.m_Lens.FieldOfView, aimFov, AimSpeed * Time.deltaTime);
            foreach (Transform child in model.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Gun");
            }
            foreach (Transform child in slide.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Gun");
            }
            Aiming = true;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, HoldPosition.position, AimSpeed * Time.deltaTime);
            PlayerCam.m_Lens.FieldOfView = Mathf.Lerp(PlayerCam.m_Lens.FieldOfView, normalFov, AimSpeed * Time.deltaTime);
            foreach (Transform child in model.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("HoldLayer");
            }
            foreach (Transform child in slide.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("HoldLayer");
            }
            Aiming = false;
        }
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && Aiming && timeSinceLastShot >= shootDelay)
        {
            if(currentNumOfBulletInMag != 0)
            {
                Shoot();
            }
            
        }
        if (Input.GetKeyDown(ReloadKey) && !Aiming && currentNumOfBulletInMag != magSize && !Reloading && reloadableBullets != 0)
        {
            animator.SetTrigger("Reload");
            Reloading = true;
        }
    }
    public void Shoot()
    {
        timeSinceLastShot = 0f;
        currentNumOfBulletInMag--;
        animator.SetTrigger("Shoot"); shot.Play(); //anim and sound
        StartCoroutine(ShakeCamera(shakeIntensity,shakeTime));
        GameObject firingBullet = Instantiate(bulletPrefab,ShotPosition.position,ShotPosition.rotation);
        Instantiate(spark, ShotPosition); Instantiate(smoke, ShotPosition);
        Rigidbody rb = firingBullet.GetComponent<Rigidbody>();
        rb.AddForce(ShotPosition.forward * bulletForce);
        
    }
    IEnumerator ShakeCamera(float ShakeIntense, float ShakeTime) 
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = PlayerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = ShakeIntense;
        yield return new WaitForSeconds(shakeTime);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
    }
    public void Reload()
    {
        while (currentNumOfBulletInMag != magSize)
        {
            reloadableBullets--;
            currentNumOfBulletInMag++;
            
        }
        if (reloadableBullets < 0)
        {
            currentNumOfBulletInMag -= (0 - reloadableBullets);
            reloadableBullets = 0;
        }
        Reloading = false;
    }
    
}
