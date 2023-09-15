using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

public class Player : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator playerAnimator;
    public PlayerFSM playerFSM = null;

    public int maxAmunitionBeforeReload = 40;
    public int totalAmunitionCount = 100;
    [HideInInspector]
    public int bulletsInMagazine = 40;
    public float roundsPerSecond = 10;

    // Start is called before the first frame update
    void Start()
    {

        playerFSM = new PlayerFSM();

        // create the FSM.
        playerFSM.Add(new PlayerFSMState_MOVEMENT(this));
        playerFSM.Add(new PlayerFSMState_ATTACK(this));
        playerFSM.Add(new PlayerFSMState_RELOAD(this));
        playerFSM.Add(new PlayerFSMState_TAKE_DAMAGE(this));
        playerFSM.Add(new PlayerFSMState_DEAD(this));
        playerFSM.Add(new PlayerFSMState_CROUCH(this));

        playerFSM.SetCurrentState(PlayerFSMStateType.MOVEMENT);
    }

    // Update is called once per frame
    void Update()
    {
        playerFSM.Update();
    }
}