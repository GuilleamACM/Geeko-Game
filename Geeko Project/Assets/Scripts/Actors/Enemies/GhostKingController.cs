﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public enum GhostKingMoves
{
    HandAttack,
    JumpAttack,
    BreathAttack,
    Heal,
    SwordAttack,
}

public class GhostKingController : EnemyController
{

    [Header("Ghost King Properties")]
    public Animator ghostKingAnimator;
    public BossState bossState;
    public GameObject particle;
    public GameObject healParticle;
    
    [Header("Camera Shake")]
    public float duration;
    public float strength;
    public int vibration;
    public float randomness;
    public bool fadeOut;

    [Header("Hand Attack")]
    public float timeHandAttack;
    public float distanceInvokeFromtheCenter;
    public GameObject[] prefabs;



    [Header("Sword Attack")] 
    public Transform swordPosition;
    public float timeSwordAttack;
    public int numberOfShotsSA;
    public float amplitudeSA;
    public float timeBetweenShotsSA;
    public float bulletSpeedSA;

    [Header("Sword Attack in Rage")] 
    public float timeDisappeared;

    public float delayTime;
    

    [Header("Jump Attack")] 
    public Transform jumpPosition;
    public int howManyJumps;
    public float timeJumpAttack;
    public int numberOfShotsRS;
    public float amplitudeDegressRS;
    public float minimumSpeedRS;
    public float maximumSpeedRS;

    [Header("Breath Attack")] 
    public Transform breathPosition;
    public float timeBreathAttack;
    public int numberOfShotsBA;
    public float timeBetweenShotsBA;
    public float bulletSpeedBA;
    public float brakingTime;

    [Header("Heal")] 
    public float timeHeal;
    public float healFrame;
    public float healValue;


    private Collider2D _collider2D;
    private GameObject _explosionObject;
    private float _rage;
    private GhostKingMoves _currMove;
    private bool _attacking;
    private Vector3 _roomCenter;
    private WeaponComponent _weaponComponent;
    private WeaponComponent _weaponHoming;
    
    private float _timeHandAttack;
    private bool _attackingHand;
    private bool _invokeAllowed;

    private float _timeSwordAttack;
    private bool _attackingSword;
    private bool _runningSword;
    private bool _disappeared;
    private bool _teleportAttack;
    private bool _delay;
    private float _delayTime;

    private float _timeJumpAttack;
    private bool _attackingJump;
    private bool _jumpIt;
    private bool _jumpAllowed;
    private int _howManyJumps;

    private float _timeBreathAttack;
    private bool _attackingBreath;

    private float _timeHeal;
    private bool _healing;
    private bool _recoverLife;
    private int _healCte=0;
    
    
    public override void Start()
    {
        base.Start();
        _roomCenter = FindObjectOfType<DungeonManager>().GetActualRoom().transform.position;
        var wcs = GetComponents<WeaponComponent>();
        _weaponComponent = wcs[0];
        _weaponHoming = wcs[1];
        _collider2D = GetComponent<Collider2D>();
    }

    public override void Idle()
    {
        base.Idle();

        if (!GetWaiting() && _attacking)
        {
            SetWaiting(true);
            StartCoroutine(WaitingToAttack(idleTime));
        }
        ghostKingAnimator.SetBool("isIdle",true);
    }
    
    public IEnumerator WaitingToAttack(float sec)
    {
        yield return new WaitForSeconds(sec);
        
        print("waited: "+sec+", ghostKing: "+currState);

        JumpOrNot();

        SetWaiting(false);
        setIdle(false);
    }
    
    private void JumpOrNot()
    {
        if (!_jumpIt)
        {
            _jumpIt = true;
            _currMove = GhostKingMoves.JumpAttack;
        }
        else
        {
            _attacking = false;
            _jumpIt = false;
        }
    }
    
    public GhostKingMoves ChooseAttack() //modified
    {
        float _sword = 0;
        float _hand = 0;
        float _breath = 0;
        float _heal = 0;
        
        var random = Random.Range(0, 100);
        
        switch (bossState)
        {
            case BossState.Normal:
                _sword = 0;
                _hand = 0;
                _breath = 0;
                _heal = 0;
                break;
            case BossState.Enrage:
                _sword = 0;
                _hand = 0;
                _breath = 0;
                _heal = 0;
                break;
            case BossState.Rage:
                 _sword = 0;
                 _hand = 0;
                 _breath = 0;
                _heal = 0;
                break;
        }

        if (random < _sword)
        {
            return GhostKingMoves.SwordAttack;
        }
        else if(random < _hand)
        {
            return GhostKingMoves.HandAttack;
        }else if (random < _breath)
        {
            return GhostKingMoves.BreathAttack;
        }
        else
        {
            return GhostKingMoves.Heal;
        }

    }
    
    public override void Attack()
    {
        if (stateHasChanged)
        {
            StopMovement(); 
        }
        
        if (!_attacking)
        {
            _currMove = ChooseAttack();
            _attacking = true;
        }
        else
        {
            switch (_currMove)
            {
                case GhostKingMoves.HandAttack:
                    HandAttack();
                    break;
                case GhostKingMoves.JumpAttack:
                    JumpAttack();
                    break;
                case GhostKingMoves.SwordAttack:
                    SwordAttack();
                    break;
                case GhostKingMoves.BreathAttack:
                    BreathAttack();
                    break;
                case GhostKingMoves.Heal:
                    Heal();
                    break;
            }
        }
    }
    
    private void ShootPattern()
    {
        switch (_currMove)
        {
            case GhostKingMoves.HandAttack:
                print("ERROR!!");
                break;
            case GhostKingMoves.JumpAttack:
                RandomSpeedAllRange();
                break;
            case GhostKingMoves.SwordAttack:
                NineWaySpread();
                break;
            case GhostKingMoves.BreathAttack:
                HomingBullets();
                break;
            case GhostKingMoves.Heal:
                print("ERROR!!");
                break;
        }
    }

    private void NineWaySpread()
    {
        var dir = PlayerDirection(swordPosition.position);

        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        
        _explosionObject = new GameObject();
        _explosionObject.transform.position = swordPosition.position;
        _weaponComponent.firePoint = _explosionObject.transform;
        _weaponComponent.SpreadNineWay(dir,numberOfShotsSA,amplitudeSA,timeBetweenShotsSA,bulletSpeedSA);
    }

    private void HomingBullets()
    {
        var dir = PlayerDirection(breathPosition.position);

        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        
        _explosionObject = new GameObject();
        _explosionObject.transform.position = breathPosition.position;
        _weaponHoming.firePoint = _explosionObject.transform;
        _weaponHoming.FourDiagonals(dir,numberOfShotsBA,timeBetweenShotsBA,bulletSpeedBA);
        _weaponHoming.SetHomingDirectional(GetPlayer(), brakingTime);

    }

    private void RandomSpeedAllRange()
    {
        if (_explosionObject)
        {
            Destroy(_explosionObject);
        }
        _explosionObject = new GameObject();
        _explosionObject.transform.position = jumpPosition.position;
        _weaponComponent.firePoint = _explosionObject.transform;
        _weaponComponent.RandomSpeedAndSpread(PlayerDirection(),numberOfShotsRS,amplitudeDegressRS,minimumSpeedRS,maximumSpeedRS);
        CameraShake(duration,strength,vibration,randomness,fadeOut);
        
    }

    public override bool flipStaticEnemy()
    {
        var flipChildren = base.flipStaticEnemy();
        return FlipChildrenIf(flipChildren);
    }
    
    private bool FlipChildrenIf(bool flipChildren)
    {
        if (flipChildren)
        {
            FlipChildren();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HandAttack()
    {
        if (!_invokeAllowed)
        {
            _invokeAllowed = MoveToCenterRoom(transform);
        }
        else
        {
            if (_timeHandAttack <= 0)
            {
                if (!_attackingHand)
                {
                    //animations things;
                    ghostKingAnimator.SetBool("isThrowing", true);
                    ghostKingAnimator.SetBool("isIdle", false);
                    
                    // thrown in the cyclop's throw frame 

                    InvokeMonsters();
                    
                    _attackingHand = true;
                    _timeHandAttack = timeHandAttack;
                }
                else
                {
                    ghostKingAnimator.SetBool("isThrowing", false);
                    ghostKingAnimator.SetBool("isIdle", true);
                    _attackingHand = false;
                    _invokeAllowed = false;
                    _recoverLife = true;
                    _currMove = GhostKingMoves.Heal;
                    setIdle(true);
                }
            }
            else
            {
                _timeHandAttack -= Time.deltaTime;
            }
        }

    }

    private void InvokeMonsters() //eu sei que poderia tá melhor
    { 
        var prefabChoosed1 = ChoosePrefab();
        var position = new Vector3(_roomCenter.x+distanceInvokeFromtheCenter,_roomCenter.y,_roomCenter.z);
        Instantiate(prefabChoosed1, position, Quaternion.identity);
        
        var prefabChoosed2 = ChoosePrefab();
        var position2 = new Vector3(_roomCenter.x-distanceInvokeFromtheCenter,_roomCenter.y,_roomCenter.z);
        Instantiate(prefabChoosed2, position2, Quaternion.identity);
    }

    private GameObject ChoosePrefab()
    {
        var size = prefabs.Length;
        var random = Random.Range(0,size);
        return prefabs[random];
    }


    public void CameraShake(float duration, float strength, int vibration, float randomness, bool fadeOut)
    {
        Camera.main.DOShakePosition(duration, strength, vibration, randomness, fadeOut);
    }
    
    private void SwordAttack()
    {
        if (_runningSword || (bossState != BossState.Rage))
        {
            SimpleSwordAttack();
        }
        else
        {
            TeleportSwordAttack();
        }
    }

    private void TeleportSwordAttack()
    {
        if (!_disappeared)
        {
            _disappeared = true;
            StartCoroutine(WaitingToAppearBehindPlayer(timeDisappeared));
        }
        else
        {
            DelayToAttack();
        }

        if (_teleportAttack)
        {
            SimpleSwordAttack();
        }
    }

    private void DelayToAttack()
    {
        if (!_delay && !_teleportAttack)
        {
            _delay = true;
            StartCoroutine(WaitDelay(delayTime));
        }
    }

    private IEnumerator WaitDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _delay = false;
        _teleportAttack = true;
    }

    private IEnumerator WaitingToAppearBehindPlayer(float timeDisappeared)
    {
        print("disappering");
        //Disappear animation things here;

        yield return new WaitForSeconds(timeDisappeared);

        MoveToBehindPlayer();
        
        //appear animation things
        
    }

    public void Disappear()
    {
        _collider2D.enabled = false;
        GetSprite().enabled = false;
        
        //shadow.SetActive(false);
    }

    public void Appear()
    {
        _collider2D.enabled = true;
        GetSprite().enabled = true;

        //shadow.SetActive(true);
    }
    
    private void MoveToBehindPlayer()
    {
        var p = GetPlayer();
        var playerPosition = p.TransformPoint(p.GetComponent<Collider2D>().offset);
        transform.position = playerPosition;
    }

    private void SimpleSwordAttack()
    {
        if (_timeSwordAttack <= 0)
        {
            if (!_attackingSword)
            {
                ghostKingAnimator.SetBool("isStomping", true);
                ghostKingAnimator.SetBool("isIdle", false);

                ShootPattern(); // call in the right frame later;
                
                _runningSword = true;
                _attackingSword = true;
                _timeSwordAttack = timeSwordAttack;
            }
            else
            {
                ghostKingAnimator.SetBool("isIdle", true);
                ghostKingAnimator.SetBool("isStomping", false);
                _attackingSword = false;
                _runningSword = false;
                _teleportAttack = false;
                setIdle(true);
            }
        }
        else
        {
            _timeSwordAttack -= Time.deltaTime;
        }
    }

    private void Heal()
    {
        if (_timeHeal <= 0)
        {
            if (!_healing)
            {
                ghostKingAnimator.SetBool("isStomping",true);
                ghostKingAnimator.SetBool("isIdle",false);
                
                healParticle.SetActive(true);
                
                _healing = true;
                _timeHeal = timeHeal;
            }
            else
            {
                ghostKingAnimator.SetBool("isIdle",true);
                ghostKingAnimator.SetBool("isStomping",false);
                _healing = false;
                //_stompRangeAllowed = false;
                setIdle(true);
                _recoverLife = false;
                healParticle.SetActive(true);
            }
        }
        else
        {
            HealPerSecond();
            _timeHeal -= Time.deltaTime;
        }
    }

    private void HealPerSecond()
    {
        if(((timeHeal-_timeHeal)/healFrame) > _healCte)
        {
            _healCte++;
            
            GetStatusComponent().Heal(healValue);

        }
    }

    private void JumpAttack()
    {
        
        if (!_jumpAllowed)
        {
            _jumpAllowed = MoveToCenterRoom(jumpPosition);
        }
        else
        {
            if (_timeJumpAttack <= 0)
            {
                if (!_attackingJump)
                {
                    ghostKingAnimator.SetBool("isStomping",true);
                    ghostKingAnimator.SetBool("isIdle",false);
                
                    ShootPattern(); // is now being called in the frame of the hittingfloor animation
                
                    _attackingJump = true;
                    _timeJumpAttack = timeJumpAttack;
                }
                else
                {
                    _howManyJumps++;

                    ResetJump();

                    RepeatOrNotJump();
                    
                }
            }
            else
            {
                _timeJumpAttack -= Time.deltaTime;
            }
        }
    }

    private void RepeatOrNotJump()
    {
        if (_howManyJumps >= howManyJumps)
        {
            print("ended jumping");
            
        }
    }

    public void StopJump()
    {
        _howManyJumps = 0;
        setIdle(true);
    }

    private void ResetJump()
    {
        _attackingJump = false;
        _jumpAllowed = false;
        ghostKingAnimator.SetBool("isIdle",true);
        ghostKingAnimator.SetBool("isStomping",false);
    }


    private bool MoveToCenterRoom(Transform transform)
    {
        //int layer = LayerMask.GetMask("Default");
        //Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius, layer);
        
        if (transform.position != _roomCenter)
        {
            var dir = DirectionNormalized(transform.position, _roomCenter);
            MoveEnemy(dir, speed);
            return false;
        }
        else
        {
            StopMovement();
            return true;
        }
    }
    
    private void BreathAttack() //maybe looping breathing 
    {
        if (_timeBreathAttack <= 0)
        {
            if (!_attackingBreath)
            {
                //animations things;
                ghostKingAnimator.SetTrigger("GuardTheEye");

                ShootPattern(); //call in the right frame later!
                
                _attackingBreath = true;
                _timeBreathAttack = timeBreathAttack;
            }
            else
            {
                _attackingBreath = false;
                setIdle(true);
            }
        }
        else
        {
            _timeBreathAttack -= Time.deltaTime;
        }
    }
    
    public void OnFlip()
    {
        FlipChildren();
    }
    
    public void IdlingAfterAttack()
    {
        ghostKingAnimator.SetBool("isIdle",true);
        ghostKingAnimator.SetBool("isStomping",false);
        ghostKingAnimator.SetBool("isThrowing",false);
    }
    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir, speed);
        ghostKingAnimator.SetBool("isMoving",true);
        ghostKingAnimator.SetBool("isIdle", false);
        
    }
    
    public override void StopMovement()
    {
        base.StopMovement();
        ghostKingAnimator.SetBool("isMoving",false);
        ghostKingAnimator.SetBool("isIdle",true);
        
        flipStaticEnemy();
    }
    
    private void FlipChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.localPosition = new Vector3(-child.localPosition.x, child.localPosition.y, child.localPosition.z);
        }

    }
    
    public void UpdateRage()
    {
        var previousMinotaurState = bossState;
        var aux = (GetCurrentHealth() / getMaximumHealth()) * 100;
        _rage = 100 - aux;
        
        if (_rage >= 70)
        {
            bossState = BossState.Rage;
            
        }else if (_rage >= 35)
        {
            bossState = BossState.Enrage;
        }

        if (previousMinotaurState != bossState)
        {
            Debug.Log("Rage in ("+_rage+") Updated to: "+bossState+" mode, with life(%): "+aux);
           
            if (bossState == BossState.Rage)
            {
                speed = speed + 0.25f;
                
                idleTime = idleTime - 0.5f;

                // var weapon = GetComponent<WeaponComponent>();
                // weapon.speed = weapon.speed + 1f;

                particle.SetActive(true);
            }else if (bossState == BossState.Enrage)
            {
                speed = speed + 0.25f;
                idleTime = idleTime - 0.25f;
                
                // var weapon = GetComponent<WeaponComponent>();
                //  weapon.speed = weapon.speed + 2f;
                
            }
            
            
        }
        
    }
    
    
    
    
}