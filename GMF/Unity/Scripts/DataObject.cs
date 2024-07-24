using BaseCore;
using GMF.Data;
using UnityEngine;

public abstract class DataObject : ScriptableObject, IData
{
    [field: SerializeField]
    [field: ReadOnly]
    public int Id { get; set; }
}
