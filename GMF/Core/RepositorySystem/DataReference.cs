using BaseCore;
using UnityEngine;

namespace GMF.Data
{
    [System.Serializable]
    public class DataReference<T> where T : IData
    {
        T cached;

        [SerializeField]
        [ReadOnly]
        int id;
        
        public T Data
        {
            get
            {
                return cached ??= Data<T>.GetById(id);
            }
            private set
            {
                cached = value;
            }
        }
        
        public DataReference(T refdata)
        {
            Data = refdata;
            id = refdata.Id;
        }
    }
}