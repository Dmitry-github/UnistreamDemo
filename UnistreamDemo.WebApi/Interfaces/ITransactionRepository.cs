namespace UnistreamDemo.WebApi.Interfaces
{
    using Models;
    using System;
    using System.Threading.Tasks;

    public interface ITransactionRepository
    {
        Task<Transaction> GetTransactionAsync(Guid transactionId);
        Task<bool> AddTransactionAsync(Transaction transaction);
        Task<Transaction> DeleteTransactionAsync(Guid transactionId);
        Task<DateTime?> RevertTransactionAsync(Guid transactionId);
    }
}