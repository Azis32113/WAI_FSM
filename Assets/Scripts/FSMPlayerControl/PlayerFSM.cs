using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;


public class PlayerFSM : FSM
{
    public PlayerFSM() : base()
    {
    }

    public void Add(PlayerFSMState state)
    {
        m_states.Add((int)state.ID, state);
    }

    public PlayerFSMState GetState(PlayerFSMStateType key)
    {
        return (PlayerFSMState)GetState((int)key);
    }

    public void SetCurrentState(PlayerFSMStateType stateKey)
    {
        State state = m_states[(int)stateKey];
        if (state != null)
        {
            SetCurrentState(state);
        }
    }
}
