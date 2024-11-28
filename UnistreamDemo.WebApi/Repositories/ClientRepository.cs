namespace UnistreamDemo.WebApi.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    public class ClientRepository: IClientRepository
    {
        //non-readoly for Storage Emulation
        private IList<Client> _clients = new List<Client>();
        private static readonly object _locker = new object();

        /*
        public ClientRepository()
        {
            _clients.Add(new Client() { Id = new Guid("66775B21-2B0E-4569-BE90-05A603000000"), Balance = 0 });
            _clients.Add(new Client() { Id = new Guid("66775B21-2B0E-4569-BE90-05A603000001"), Balance = 10 });
            _clients.Add(new Client() { Id = new Guid("66775B21-2B0E-4569-BE90-05A603000002"), Balance = 20 });
            _clients.Add(new Client() { Id = new Guid("66775B21-2B0E-4569-BE90-05A603000003"), Balance = 30 });
        }
        */

        public ClientRepository(IList<Client> clients)
        {
            _clients = clients;
        }

        public Client Get(Guid clientId)
        {
            lock (_locker)
            {
                return _clients.FirstOrDefault(c => c.Id == clientId);
            }
        }

        public decimal? GetBalance(Guid clientId)
        {
            lock (_locker)
            {
                return Get(clientId)?.Balance;
            }
        }

        public bool UpdateBalance(Guid clientId, decimal balance)
        {
            lock (_locker)
            {
                if (_clients.Count > 0)
                {
                    var client = _clients.FirstOrDefault(c => c.Id == clientId);
                    if (client != null) client.Balance = balance;
                    return true;
                }
            }
            return false;
        }
    }
}