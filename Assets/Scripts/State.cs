
namespace Patterns
{
    public class State
    {
        protected FSM m_fsm;
        public State(FSM fsm)
        {
            m_fsm = fsm;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}