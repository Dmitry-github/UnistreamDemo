using System;

namespace UnistreamDemo.WebApi.Interfaces
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IClientRepository
    {
        //IEnumerable<Client> GetAll();
        
        Client Get(Guid clientId);
        public decimal? GetBalance(Guid clientId);
        public bool UpdateBalance(Guid clientId, decimal balance);
        
        //Task<Client> AddAsync(Client item);
        //Task RemoveAsync(int id);
        //Task<bool> UpdateAsync(Client item);
    }
}
