using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TRPGGame.Repository
{
    public interface IRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetDataAsync();
    }
}
