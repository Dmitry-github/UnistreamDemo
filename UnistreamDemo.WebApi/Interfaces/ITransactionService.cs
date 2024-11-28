namespace UnistreamDemo.WebApi.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Queries;
    using Responses;
    using Models;

    public interface ITransactionService
    {
        Task<(bool, string)> TransactionIsValidAsync(TransactionQuery transactionQuery, TransactionType transactionType);
        Task<TransactionResponse> CommitCreditAsync(TransactionQuery transactionQuery);
        Task<TransactionResponse> CommitDebitAsync(TransactionQuery transactionQuery);
        Task<(RevertResponse, string)> RevertAsync(Guid transactionId);
        Task<BalanceResponse> GetClientBalanceAsync(Guid clientId);
    }
}
