namespace UnistreamDemo.WebApi.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Threading;

    public class ClientRepository: IClientRepository
    {
        //non-readoly for Storage Emulation

        private ConcurrentBag<Client> _clients = new ConcurrentBag<Client>();
        
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
            //_clients = clients;
            _clients = new ConcurrentBag<Client>(clients);
        }

        public async Task<Client> GetAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            return _clients.FirstOrDefault(c => c.Id == clientId);
        }

        public async Task<decimal?> GetBalanceOrDefaultAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            var client = await GetAsync(clientId, cancellationToken);
            return client?.Balance;
        }

        public async Task<bool> UpdateBalanceAsync(Guid clientId, decimal balance, CancellationToken cancellationToken = default)
        {
            if (_clients.Count > 0)
            {
                var client = _clients.FirstOrDefault(c => c.Id == clientId);
                if (client != null) client.Balance = balance;
                return true;
            }

            return false;
        }
    }
}