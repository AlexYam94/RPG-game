namespace RPG.Control
{
    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}