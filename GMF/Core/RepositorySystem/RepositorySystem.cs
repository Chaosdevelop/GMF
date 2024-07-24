using System.Linq;
using System.Threading.Tasks;

namespace GMF.Data
{
    public interface IData
    {
        int Id { get; set; }
    }

    public interface IDataRepository<T> where T : IData
    {
        T Singleton { get; }
        T GetById(int id);
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        Task<IQueryable<T>> GetAllAsync();
    }


    public static class Data<T> where T : IData
    {
        static IDataRepository<T> Repository => Services.GetService<IDataRepository<T>>();
        
        public static T Singleton => Repository.Singleton;
        public static T GetById(int id) => Repository.GetById(id);
        public static IQueryable<T> GetAll() => Repository.GetAll();
        public static Task<T> GetByIdAsync(int id) => Repository.GetByIdAsync(id);
        public static Task<IQueryable<T>> GetAllAsync() => Repository.GetAllAsync();
    }

}