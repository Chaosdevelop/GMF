using Microsoft.Extensions.DependencyInjection;
using R3;

namespace GMF
{
    public interface IGameStateManager
    {
        IGameState CurrentState { get; }
        void ChangeState(IGameState newState);
        Observable<IGameState> OnStateChanged { get; }
    }
    
    [ServiceDescriptor(ServiceLifetime.Singleton)]
    public class GameStateManager : IGameStateManager
    {
        //TODO: убрать R3
        private readonly ReactiveProperty<IGameState> currentState;
        
        public Observable<IGameState> OnStateChanged => currentState.AsObservable();
        
        public GameStateManager()
        {
            currentState = new ReactiveProperty<IGameState>();
        }
        
        public IGameState CurrentState => currentState.Value;
        
        public void ChangeState(IGameState newState)
        {
            currentState.Value?.Exit();
            newState?.Enter();
            currentState.Value = newState;
        }
    }
    
    public interface IGameState
    {
        void Enter();
        void Exit();
    }
}