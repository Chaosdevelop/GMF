using System;
using Microsoft.Extensions.DependencyInjection;

namespace GMF
{
	public interface IGameStateManager
	{
		IGameState CurrentState { get; }
		void ChangeState(IGameState newState);
		System.Action<IGameState> OnStateChanged { get; }

	}

	[ServiceDescriptor(ServiceLifetime.Singleton)]
	public class GameStateManager : IGameStateManager
	{

		IGameState currentState;
		public IGameState CurrentState => currentState;
		public Action<IGameState> OnStateChanged { get; set; }

		public GameStateManager()
		{

		}

		public void ChangeState(IGameState newState)
		{
			currentState?.Exit();
			OnStateChanged?.Invoke(newState);
			newState?.Enter();
			currentState = newState;
		}


	}

	public interface IGameState
	{
		void Enter();
		void Exit();
	}

	public class InitializationState : IGameState
	{
		public void Enter() => Console.WriteLine("Entering Initialization State");
		public void Exit() => Console.WriteLine("Exiting Initialization State");
	}

}