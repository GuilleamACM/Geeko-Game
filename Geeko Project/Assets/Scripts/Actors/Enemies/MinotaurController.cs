﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MinotaurState
{
    Normal,
    Stressed,
    Rage
}

public enum MinotaurAttack
{
    Poke,
    Axe,
    Spin,
    Dash,
    FloorHit,
}
public class MinotaurController : EnemyController
{
    [Header("Minotaur Properties")]
    public Animator minotaurAnimator;
    public Transform axeAttackPosition;
    public Transform pokePosition;
    public MinotaurState minotaurState = MinotaurState.Normal;
    public MinotaurAttack[] attacks;

    public LayerMask layerMask;
    public float axeAttackRange;
    public Vector2 pokeAttackRange;
    public float timeBtwAxeAttack;
    public float timeBtwPokeAttack;
    public float timeBtwFloorHitAttack;
    public float timeSpinning;
    
    private float _rage = 0;
    private bool _attacking=false;
    private MinotaurAttack _curAttack;
    private bool _attackingAxe;
    private bool _attackingPoke;
    private bool _attackingFloorHit;
    private float _timeBtwAxeAttack=0;
    private float _timeBtwPokeAttack=0;
    private float _timeBtwFloorHitAttack;
    private float _timeBtwSpinAttack;
    private bool _waitingIdle;
    private bool _floorHit;
    private bool _changeState;
    private bool _attackingSpin;
    


    /*
     *To-do after implement the minotaur states:
     * adjust minotaur children on flip and make it shoot not in opposite direction of the player
     */

    public override void CheckTransitions()
    {
        if (IsPlayerInRange(sightRange) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }
        
        if ((getIdle()))
        {
            currState = EnemyState.Idle;
        }else if (IsPlayerInAttackRange(attackRange) || _attacking)
        {
            currState = EnemyState.Attack;
        }
        
        if (GetCurrentHealth() <= 0)
        {
            currState = EnemyState.Die;
        }
        
    }

    public MinotaurAttack ChooseAttack()
    {
        int lottery = 0;
        int poke=0;
        int axe=0;
        int spin=0;
        int dash=0;
        
        /*
        switch (minotaurState)
        {
            case(MinotaurState.Normal):
                poke = 50; 
                axe = 100;
                break;
            case(MinotaurState.Stressed):
                poke = 25;
                axe = 50;
                spin = 100;
                break;
            case(MinotaurState.Rage):
                poke = 10;
                axe = 20;
                spin = 50;
                dash = 100;
                break;
        }
        */
        lottery = Random.Range(0, 100);

        if (lottery < poke)
        {
            return attacks[0];
        } 
        else if (lottery < axe)
        {
            return attacks[1];
        }
        /*
        else if (lottery < spin)
        {
            return attacks[2];
        }else if (lottery < dash)
        {
            return attacks[3];
        }*/
        Debug.Log("Bug in the lottery, number not in the range expected");
        return attacks[lottery];
    }
    
    public override void BasicAttacks() //minotaur Attacks
    {
        if (!_attacking)
        {
            _curAttack = ChooseAttack();
            _attacking = true;
        }
        else
        {
            switch (_curAttack)
            {
                case(MinotaurAttack.Poke):
                    Poke();
                    break;
                case(MinotaurAttack.Axe):
                    AxeAttack();
                    break;
                case(MinotaurAttack.FloorHit):
                    FloorHit();
                    break;
                case(MinotaurAttack.Spin):
                    Spin();
                    break;
                case(MinotaurAttack.Dash):
                    Dash();
                    break;
            }
        }
    }
    private void AxeAttack()
    {
        if (_timeBtwAxeAttack <= 0)
        {
            //attack
            if (!_attackingAxe)
            {
                minotaurAnimator.SetTrigger("isAttackingAxe");
                minotaurAnimator.SetBool("isIdle",false);
                
                HitBoxAxe();
                _attackingAxe = true;
                _timeBtwAxeAttack = timeBtwAxeAttack;
            }
            else
            {
                _attackingAxe = false;
                setIdle(true); //idling after attack
            }
        }
        else
        {
            _timeBtwAxeAttack -= Time.deltaTime;
        }
    }

    private void HitBoxAxe()
    {
        
        Collider2D hit = Physics2D.OverlapCircle(axeAttackPosition.position, axeAttackRange, layerMask);
        if (hit)
        {
            hit.gameObject.GetComponent<StatusComponent>().TakeDamage(10);
            Debug.Log("damaged by axe");
        }
        /*
        else
        {
            Debug.Log("got nothing in the axe");
        } */
    }

    private void HitBoxPoke()
    {
        Collider2D hit = Physics2D.OverlapBox(pokePosition.position, pokeAttackRange, 0,layerMask);
        if (hit)
        {
            hit.gameObject.GetComponent<StatusComponent>().TakeDamage(5);
            Debug.Log("damaged by poke");
        }/*
        else
        {
            Debug.Log("got nothing in the poke");
        }*/
    }
    
    private void Poke()
    {
        if (_timeBtwPokeAttack <= 0)
        {
            //attack
            if (!_attackingPoke)
            {
                minotaurAnimator.SetTrigger("isAttackingPoke");
                minotaurAnimator.SetBool("isIdle",false);
                
                HitBoxPoke();
                
                _attackingPoke = true;
                _timeBtwPokeAttack = timeBtwPokeAttack;
            }
            else
            {
                _attackingPoke = false;
                setIdle(true); //idling after attack
            }
        }
        else
        {
            _timeBtwPokeAttack -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(axeAttackPosition.position,axeAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pokePosition.position, pokeAttackRange);
    }

    private void Spin()
    {
        if (_timeBtwSpinAttack <= 0)
        {
            if (!_attackingSpin)
            {
                //animations things;
                minotaurAnimator.SetTrigger("isAttackingSpin");
                minotaurAnimator.SetBool("isIdle", false);
                
                //hitbox??
                
                ShootPattern();

                _attackingSpin = true;
                _timeBtwSpinAttack = timeSpinning;
            }
            else
            {
                _attackingSpin = false;
                setIdle(true);
            }
        }
        else
        {
            _timeBtwSpinAttack -= Time.deltaTime;
        }
    }

    private void FloorHit()
    {
        if (_timeBtwFloorHitAttack <= 0)
        {
            if (!_attackingFloorHit)
            {
                minotaurAnimator.SetBool("isHittingFloor",true);
                minotaurAnimator.SetBool("isIdle",false);
                
                ShootPattern();
                
                _attackingFloorHit = true;
                _timeBtwFloorHitAttack = timeBtwFloorHitAttack;
            }
            else
            {
                minotaurAnimator.SetBool("isIdle",true);
                _attackingFloorHit = false;
                setIdle(true);
            }
        }
        else
        {
            _timeBtwFloorHitAttack -= Time.deltaTime;
        }
    }

    private void ShootPattern()
    {
        projectile.SetActive(true);
        switch (_curAttack)
        {
            case MinotaurAttack.FloorHit:
                SpiralPattern(36,0,1);
                Invoke("DeactivateWeapon",0+0.1f);
                break;
            case MinotaurAttack.Spin:
                SpiralPattern(40,timeSpinning,2);
                Invoke("DeactivateWeapon",timeSpinning+0.1f);
                break;
            case MinotaurAttack.Dash:
                SpiralPattern(36,2,1);
                Invoke("DeactivateWeapon",2+0.1f);
                break;
        }
        
    }

    private void DeactivateWeapon()
    {
        projectile.SetActive(false);
        minotaurAnimator.SetBool("isHittingFloor",false);
    }

    private void SpiralPattern(int numberOfShotsPerLoop,float timeToSpiralOnce, int loops)
    {
        var weaponComponent = this.gameObject.GetComponent<WeaponComponent>();
       // weaponComponent.AttemptToShoot();
        var vec3 =  projectile.transform.position - pokePosition.position;
        var vec2 = new Vector2(vec3.x, vec3.y);
        weaponComponent.Spiral(vec2,numberOfShotsPerLoop,timeToSpiralOnce,loops, weaponComponent.speed);
        Debug.Log("Spiral");
    }


    public override void Idle()
    {
        base.Idle();
        if (!GetWaiting() && _attacking)
        {
            SetWaiting(true);
            StartCoroutine(WaitingToAttack(idleTime));
        }
        minotaurAnimator.SetBool("isIdle",true);

    }

    public IEnumerator WaitingToAttack(float sec)
    {
        yield return new WaitForSeconds(sec);
        
        if (!_floorHit)
        {
            _floorHit = true;
            _curAttack = MinotaurAttack.FloorHit;
        }
        else
        {
            _attacking = false;
            _floorHit = false;
        }
        
        SetWaiting(false);
        setIdle(false);
    }

    public void OnHit()
    {
        UpdateRage();
        minotaurAnimator.SetTrigger("isTakingDamage");
    }

    public void OnDeath()
    {
        minotaurAnimator.SetBool("isDead",true);
    }
    
    public void IdlingAfterAttack()
    {
       // Debug.Log("idling after attack: "+minotaurAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        minotaurAnimator.SetBool("isIdle",true);
    }

    public void OnFlip()
    {
        Debug.Log("Funcionou?");
    }

    public override void MoveEnemy(Vector3 dir, float speed)
    {
        base.MoveEnemy(dir, speed);
        minotaurAnimator.SetBool("isMoving",true);
        minotaurAnimator.SetBool("isIdle", false);
        
    }

    public override void StopMovement()
    {
        base.StopMovement();
        minotaurAnimator.SetBool("isMoving",false);
        minotaurAnimator.SetBool("isIdle",true);
    }

    public override void Attack()
    {
        if (stateHasChanged)
        {
            StopMovement();  // so the player cant be in moving animation, without shooting when enemystate = atack
        }

        base.Attack();

    }
    
    public void UpdateRage()
    {
        var previousMinotaurState = minotaurState;
        var aux = (GetCurrentHealth() / getMaximumHealth()) * 100;
        _rage = 100 - aux;
        
        if (_rage >= 70)
        {
            minotaurState = MinotaurState.Rage;
        }else if (_rage >= 35)
        {
            minotaurState = MinotaurState.Stressed;
        }
        /*
        else
        {
            minotaurState = MinotaurState.Normal;
        }*/

        if (previousMinotaurState != minotaurState)
        {
            Debug.Log("Rage in ("+_rage+") Updated to: "+minotaurState+" mode, with life(%): "+aux);
        }
        
    }
}
