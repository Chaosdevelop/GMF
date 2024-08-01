using System.Collections.Generic;
using System.Linq;
using GMF;
using GMF.Saving;
using GMF.Utility;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public interface IStartupInstallable
{
	public abstract ServiceDescriptor GetServiceDescriptor();
}

public abstract class InstallableMonoBehaviour : MonoBehaviour, IStartupInstallable
{
	public abstract ServiceDescriptor GetServiceDescriptor();
}

public abstract class InstallableScriptableObject : ScriptableObject, IStartupInstallable
{
	public abstract ServiceDescriptor GetServiceDescriptor();
}

public interface IGameStarter
{
	void Initialize(IStartupInstallable[] startupInstallables, IGameState onInitializedState);
}

public class GameStarter : IGameStarter
{
	public async void Initialize(IStartupInstallable[] startupInstallables, IGameState onInitializedState)
	{
		Services.Initialize(startupInstallables?.Select(arg => arg.GetServiceDescriptor()));
		Services.GetService<IGameStateManager>().ChangeState(new InitializationState());
		Services.GetService<ISaveLoadManager>().Initialize();
		await SaveSystem.TryAutoLoadAsync();
		Services.GetService<IGameStateManager>().ChangeState(onInitializedState);
	}
}

public class UnityGameStarter : MonoBehaviour
{
	[SerializeField]
	InstallableScriptableObject[] installableScriptableObjects;
	[SerializeField]
	InstallableMonoBehaviour[] installableMonoBehaviours;

	[SerializeReference]
	[TypeSelector]
	IGameState onInitializedState;
	private async void Awake()
	{
		Debug.Log($"UnityGameStarter initialization start");
		var startupInstallables = new List<IStartupInstallable>(installableScriptableObjects);
		startupInstallables.AddRange(installableMonoBehaviours);

		Services.Initialize(startupInstallables.Select(arg => arg.GetServiceDescriptor()));
		Services.GetService<IGameStateManager>().ChangeState(new InitializationState());
		Services.GetService<ISaveLoadManager>().Initialize();
		await SaveSystem.TryAutoLoadAsync();
		Services.GetService<IGameStateManager>().ChangeState(onInitializedState);

		Debug.Log($"UnityGameStarter initialization end");
	}


}
