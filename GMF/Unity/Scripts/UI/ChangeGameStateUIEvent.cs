using GMF;
using GMF.UI;
using GMF.Utility;
using UnityEngine;

public class ChangeGameStateUIEvent : IEventUI
{
	[SerializeReference]
	[TypeSelector]
	IGameState gotoState;
	public void Send()
	{
		if (gotoState == null)
		{
			throw new System.InvalidOperationException("The 'onInitializedState' must be initialized before sending the event.");
		}
		Services.GetService<IGameStateManager>().ChangeState(gotoState);
	}
}
