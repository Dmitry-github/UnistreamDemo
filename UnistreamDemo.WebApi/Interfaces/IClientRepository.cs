namespace UnistreamDemo.WebApi.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;
    using System.Threading;

    public interface IClientRepository
    {
        Task<Client> GetAsync(Guid clientId, CancellationToken cancellationToken = default);
        Task<decimal?> GetBalanceOrDefaultAsync(Guid clientId, CancellationToken cancellationToken = default);
        Task<bool> UpdateBalanceAsync(Guid clientId, decimal balance, CancellationToken cancellationToken = default);
    }
}
