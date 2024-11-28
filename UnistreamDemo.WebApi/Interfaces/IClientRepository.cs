namespace UnistreamDemo.WebApi.Interfaces
{
    using Models;
    using System;

    public interface IClientRepository
    {
        Client Get(Guid clientId);
        public decimal? GetBalance(Guid clientId);
        public bool UpdateBalance(Guid clientId, decimal balance);
    }
}
