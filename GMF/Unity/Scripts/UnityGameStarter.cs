using System.Linq;
using GMF;
using GMF.Saving;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public abstract class InstallableScriptableObject : ScriptableObject
{
    public abstract ServiceDescriptor GetServiceDescriptor();
}



public class UnityGameStarter : MonoBehaviour
{
    [SerializeField]
    InstallableScriptableObject[] installableScriptableObjects;


    private async void Awake()
    {
        Services.Initialize(installableScriptableObjects?.Select(arg => arg.GetServiceDescriptor()));
        Services.GetService<IGameStateManager>().ChangeState(new InitializationState());
        Services.GetService<ISaveLoadManager>().Initialize();
        await SaveSystem.TryAutoLoadAsync();
        Services.GetService<IGameStateManager>().ChangeState(new MainPlayState());

        Debug.Log($"UnityGameStarter awaited");
    }

    /*    private void Awake()
		{
			Services.Initialize(installableScriptableObjects?.Select(arg => arg.GetServiceDescriptor()));
			Services.GetService<IGameStateManager>().ChangeState(new InitializationState());
			Services.GetService<ISaveLoadManager>().Initialize();

			//Task.Run(() => SaveSystem.TryAutoLoadAsync()).GetAwaiter().GetResult();

			SaveSystem.TryAutoLoadAsync()

			Debug.Log($"UnityGameStarter ends");
		}*/
}
