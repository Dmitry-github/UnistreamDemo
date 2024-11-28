namespace UnistreamDemo.WebApi.Interfaces
{
    using Interfaces;
    using Models;
    using System;

    public interface ITransactionRepository
    {
        Transaction GetTransaction(Guid transactionId);
        bool AddTransaction(Transaction transaction);
        Transaction DeleteTransaction(Guid transactionId);
        DateTime? RevertTransaction(Guid transactionId);
    }
}