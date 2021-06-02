using System;

namespace RPG.Control{
    public interface IStateMachine{
        void InitDefaultState(IState state);
        void SetState(IState state);
        void AddTransition(IState from, IState to, Func<bool> predicate);
        void AddAnyTransition(IState state, Func<bool> predicate);
    }
}