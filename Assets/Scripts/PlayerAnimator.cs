using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement mov;
    private Animator anim;
    private SpriteRenderer spriteRend;

    private DemoManager demoManager;

    [Header("Movement Tilt")]
    public bool shouldTilt = false;
    [SerializeField] private float maxTilt;
    [SerializeField] [Range(0, 1)] private float tiltSpeed;

    [Header("Particle FX")]
    [SerializeField] private GameObject jumpFX;
    [SerializeField] private GameObject landFX;
    private ParticleSystem _jumpParticle;
    private ParticleSystem _landParticle;

    public bool startedJumping {  private get; set; }
    public bool justLanded { private get; set; }
    public bool running { private get; set; }
    public bool isInAir { private get; set; }
    public bool isFalling { private get; set; }
    public bool isDashing { private get; set; }
    public bool isWallSliding { private get; set; }

    private void Start()
    {
        mov = GetComponent<PlayerMovement>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        anim = spriteRend.GetComponent<Animator>();

        demoManager = FindObjectOfType<DemoManager>();

        _jumpParticle = jumpFX.GetComponent<ParticleSystem>();
        _landParticle = landFX.GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        #region Tilt
        if(shouldTilt)
        {
            float tiltProgress;

            int mult = -1;

            if (mov.IsWallSliding)
            {
                tiltProgress = 0.25f;
            }
            else
            {
                tiltProgress = Mathf.InverseLerp(-mov.Data.runMaxSpeed, mov.Data.runMaxSpeed, mov.RB.velocity.x);
                mult = (mov.IsFacingRight) ? 1 : -1;
            }
                
            float newRot = ((tiltProgress * maxTilt * 2) - maxTilt);
            float rot = Mathf.LerpAngle(spriteRend.transform.localRotation.eulerAngles.z * mult, newRot, tiltSpeed);
            spriteRend.transform.localRotation = Quaternion.Euler(0, 0, rot * mult);
        }
        #endregion

        CheckAnimationState();
    }

    //lets check animation state and play an animation per the state
    private void CheckAnimationState()
    {
        if (startedJumping)
        {
            isInAir = true;

            // Debug.Log("Jumped");
            anim.SetTrigger("Jump");
            anim.SetBool("Landed", false);
            GameObject obj = Instantiate(jumpFX, transform.position - (Vector3.up * transform.localScale.y / 2), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            startedJumping = false;
            return;
        }

        if (justLanded)
        {   
            isInAir = false;
         
            // Debug.Log("Landed");
            anim.SetBool("Landed", true);
            GameObject obj = Instantiate(landFX, transform.position - (Vector3.up * transform.localScale.y / 1.5f), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            justLanded = false;
            return;
        }

        //wanted to add particles but figured it will slow down performance
        if(running)
        {
            anim.SetFloat("Speed", 1f);
        }else{
            anim.SetFloat("Speed", 0f);
            
        }

        //we want to play this animation only if we are falling down and not jumping
        if(isFalling && !isInAir)
        {
            anim.SetBool("Falling", true);
        }else{
            anim.SetBool("Falling", false);
        }

        if(isDashing)
        {
            anim.SetTrigger("DashAttack");
        }

        if(isWallSliding)
        {
            anim.SetBool("WallSliding", true);
        }else{
            anim.SetBool("WallSliding", false);
        }
        
        //this was used for some sort of ground checking, it doesn't work well for this project
        // anim.SetFloat("Vel Y", mov.RB.velocity.y);
    }
}
