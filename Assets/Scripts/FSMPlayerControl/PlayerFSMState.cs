using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

public enum PlayerFSMStateType
{
    MOVEMENT = 0,
    CROUCH,
    ATTACK,
    RELOAD,
    TAKE_DAMAGE, // we won't use this as we do not have an animation for this state.
    DEAD,
}

public class PlayerFSMState : State {
    // For convenience we will keep the ID for a State.
    // This ID represents the key
    public PlayerFSMStateType ID { get { return _id; } }

    protected Player _player = null;
    protected PlayerFSMStateType _id;

    public PlayerFSMState(FSM fsm, Player player) : base(fsm)
    {
        _player = player;
    }

    // A convenience constructor with just Player
    public PlayerFSMState(Player player) : base(default)
    {
        _player = player;
        m_fsm = _player.playerFSM;
    }

    // The following are the normal methods from the State base class.
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}


public class PlayerFSMState_ATTACK : PlayerFSMState
{
    private float m_elaspedTime;

    public GameObject AttackGameObject { get; set; } = null;

    public PlayerFSMState_ATTACK(Player player) : base(player)
    {
        _id = PlayerFSMStateType.ATTACK;
    }

    private int _attackID = 0;
    private string _attackName;

    public int AttackId
    {
        get
        {
            return _attackID;
        }
        set
        {
            _attackID = value;
            _attackName = "Attack" + (_attackID + 1).ToString();
        }
    }

    public override void Enter()
    {
        //Debug.Log("PlayerFSMState_ATTACK");
        _player.playerAnimator.SetBool(_attackName, true);
        m_elaspedTime = 0.0f;
    }
    public override void Exit()
    {
        //Debug.Log("PlayerFSMState_ATTACK - Exit");
        _player.playerAnimator.SetBool(_attackName, false);
    }
    public override void Update()
    {
        //Debug.Log("Ammo count: " + _player.totalAmunitionCount + ", In Magazine: " + _player.bulletsInMagazine);
        if (_player.bulletsInMagazine == 0 && _player.totalAmunitionCount > 0)
        {
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.RELOAD);
            return;
        }

        if (_player.totalAmunitionCount == 0)
        {
            //Debug.Log("No ammo");
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.MOVEMENT);
            //_player.playerEffects.NoAmmo();
            return;
        }

        //_player.playerEffects.Aim();

        if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha3))
        {
            _player.playerAnimator.SetBool(_attackName, true);
            if (m_elaspedTime == 0.0f)
            {
                Fire();
            }

            m_elaspedTime += Time.deltaTime;
            if (m_elaspedTime > 1.0f / _player.roundsPerSecond)
            {
                m_elaspedTime = 0.0f;
            }
        }
        else
        {
            m_elaspedTime = 0.0f;
            _player.playerAnimator.SetBool(_attackName, false);
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.MOVEMENT);
        }
    }

    void Fire()
    {
        float secs = 1.0f / _player.roundsPerSecond;
        //_player.playerEffects.DelayedFire(secs);
        _player.bulletsInMagazine -= 1; ;
    }
}

public class PlayerFSMState_CROUCH : PlayerFSMState
{
    public PlayerFSMState_CROUCH(Player player) : base(player)
    {
        _id = PlayerFSMStateType.CROUCH;
    }

    public override void Enter()
    {
        _player.playerAnimator.SetBool("Crouch", true);
    }
    public override void Exit()
    {
        _player.playerAnimator.SetBool("Crouch", false);
    }
    public override void Update()
    {
        if (Input.GetButton("Crouch"))
        {
        }
        else
        {
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.MOVEMENT);
        }
    }
    public override void FixedUpdate() { }
}

public class PlayerFSMState_DEAD : PlayerFSMState
{
    public PlayerFSMState_DEAD(Player player) : base(player)
    {
        _id = PlayerFSMStateType.DEAD;
    }

    public override void Enter()
    {
        Debug.Log("Player dead");
        _player.playerAnimator.SetTrigger("Die");
    }
    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }
}

public class PlayerFSMState_MOVEMENT : PlayerFSMState
{
    public PlayerFSMState_MOVEMENT(Player player) : base(player)
    {
        _id = PlayerFSMStateType.MOVEMENT;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        // call PlayerMovement's Move method.
        _player.playerMovement.Move();

        //_player.playerEffects.Aim();
        if (Input.GetButton("Fire1"))
        {
            // Fire 1 (Full Auto)
            PlayerFSMState_ATTACK attackState = (PlayerFSMState_ATTACK)_player.playerFSM.GetState(PlayerFSMStateType.ATTACK);
            attackState.AttackId = 0;
            _player.roundsPerSecond = 10;
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.ATTACK);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            // Fire 2 (Burst 3)
            PlayerFSMState_ATTACK attackState = (PlayerFSMState_ATTACK)_player.playerFSM.GetState(PlayerFSMStateType.ATTACK);
            attackState.AttackId = 1;
            _player.roundsPerSecond = 6;
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.ATTACK);
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            // Fire 3 (Single Shot)
            PlayerFSMState_ATTACK attackState = (PlayerFSMState_ATTACK)_player.playerFSM.GetState(PlayerFSMStateType.ATTACK);
            attackState.AttackId = 2;
            _player.roundsPerSecond = 2;
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.ATTACK);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerFSMState_RELOAD reloadState = (PlayerFSMState_RELOAD)_player.playerFSM.GetState(PlayerFSMStateType.RELOAD);
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.RELOAD);
        }

        if (Input.GetButton("Crouch")) 
        {
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.CROUCH);
        }

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

public class PlayerFSMState_RELOAD : PlayerFSMState
{
    public float ReloadTime = 3.0f;
    float dt = 0.0f;
    public int previousState;
    public PlayerFSMState_RELOAD(Player player) : base(player)
    {
        _id = PlayerFSMStateType.RELOAD;
    }

    public override void Enter()
    {
        //Debug.Log("PlayerFSMState_RELOAD");
        _player.playerAnimator.SetTrigger("Reload");
        dt = 0.0f;
    }
    public override void Exit()
    {
        if (_player.totalAmunitionCount > _player.maxAmunitionBeforeReload)
        {
            _player.bulletsInMagazine += _player.maxAmunitionBeforeReload;
            _player.totalAmunitionCount -= _player.bulletsInMagazine;
        }
        else if (_player.totalAmunitionCount > 0 && _player.totalAmunitionCount < _player.maxAmunitionBeforeReload)
        {
            _player.bulletsInMagazine += _player.totalAmunitionCount;
            _player.totalAmunitionCount = 0;
        }
        //Debug.Log("PlayerFSMState_RELOAD - Exit");
    }
    public override void Update()
    {
        dt += Time.deltaTime;
        //_player.playerAnimator.SetTrigger("Reload");
        //_player.playerEffects.Reload();
        if (dt >= ReloadTime)
        {
            //Debug.Log("Reload complete in " + dt);
            _player.playerFSM.SetCurrentState(PlayerFSMStateType.MOVEMENT);
        }
    }
    public override void FixedUpdate() { }
}

public class PlayerFSMState_TAKE_DAMAGE : PlayerFSMState
{
    public PlayerFSMState_TAKE_DAMAGE(Player player) : base(player)
    {
        _id = PlayerFSMStateType.TAKE_DAMAGE;
    }

    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }
}


