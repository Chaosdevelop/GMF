using GMF.Data;
using GMF.Utility;
using UnityEngine;

public abstract class DataObject : ScriptableObject, IData
{
	[field: SerializeField]
	[field: ReadOnly]
	public int Id { get; set; }
}
