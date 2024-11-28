namespace UnistreamDemo.WebApi.Services
{
    using Interfaces;
    using Models;
    using Responses;
    using Queries;
    using System;
    using System.Threading.Tasks;

    public class TransactionService: ITransactionService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(IClientRepository clientRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<(bool, string)> TransactionIsValidAsync(TransactionQuery transactionQuery, TransactionType transactionType)
        {
            var currentBalance = await Task.Run(() => _clientRepository.GetBalance(transactionQuery.ClientId));
            
            if (currentBalance == null) return (false, "(client id)");

            var newBalance = (decimal) currentBalance + (int)transactionType * transactionQuery.Amount;

            return (newBalance > 0, "(balance)");
        }

        public async Task<TransactionResponse> CommitCreditAsync(TransactionQuery transactionQuery)
        {
            return await Task.Run(() => CommitTransaction(transactionQuery, TransactionType.Credit));
        }

        public async Task<TransactionResponse> CommitDebitAsync(TransactionQuery transactionQuery)
        {
            return await Task.Run(() => CommitTransaction(transactionQuery, TransactionType.Debit));
        }

        public async Task<(RevertResponse, string)> RevertAsync(Guid transactionId)
        {
            return await Task.Run(() => RevertTransaction(transactionId));
        }

        public async Task<BalanceResponse> GetClientBalanceAsync(Guid clientId)
        {

            var balance = await Task.Run(() => _clientRepository.GetBalance(clientId));

            var balanceResponse = balance == null
                ? null
                : new BalanceResponse()
                {
                    ClientBalance = balance,
                    BalanceDateTime = DateTime.Now
                };

            return balanceResponse;
        }

        private TransactionResponse CommitTransaction(TransactionQuery transactionQuery, TransactionType transactionType)
        {
            TransactionResponse transactionResponse;
            var existingTransaction = _transactionRepository.GetTransaction(transactionQuery.Id);
            
            if (existingTransaction != null)
            {
                var clientBalance = _clientRepository.GetBalance(existingTransaction.ClientId) ?? Decimal.MinValue;
                
                if (clientBalance == decimal.MinValue) return null;

                transactionResponse = new TransactionResponse()
                {
                    ClientBalance = clientBalance,
                    InsertDateTime = existingTransaction.DateTime
                };
                return transactionResponse;
            }
            else
            {
                var newTransaction = new Transaction()
                {
                    Id = transactionQuery.Id,
                    ClientId = transactionQuery.ClientId,
                    DateTime = transactionQuery.DateTime,
                    Amount = transactionQuery.Amount,
                    Type = transactionType
                };

                var currentBalance = _clientRepository.GetBalance(transactionQuery.ClientId) ?? Decimal.MinValue;
                
                if (currentBalance == decimal.MinValue) return null;

                currentBalance += (int)transactionType * transactionQuery.Amount;

                //TODO: check both success
                _transactionRepository.AddTransaction(newTransaction);
                _clientRepository.UpdateBalance(transactionQuery.ClientId, currentBalance);

                transactionResponse = new TransactionResponse()
                {
                    ClientBalance = currentBalance,
                    InsertDateTime = newTransaction.DateTime
                };
            }

            return transactionResponse;
        }

        private (RevertResponse, string) RevertTransaction(Guid transactionId)
        {
            var existingTransaction = _transactionRepository.GetTransaction(transactionId);

            if (existingTransaction?.ClientId != null)
            {
                var currentBalance = _clientRepository.GetBalance(existingTransaction.ClientId) ?? Decimal.MinValue;

                if (currentBalance == Decimal.MinValue)
                    return (null, $"no client {existingTransaction.ClientId} found");

                if (existingTransaction?.RevertDateTime != null)    //reverted previously. 
                {
                    return (new RevertResponse()
                        {
                            ClientBalance = currentBalance,
                            RevertDateTime = (DateTime)existingTransaction.RevertDateTime
                        }, string.Empty);
                }

                var newBalance = currentBalance + -1 * (int)existingTransaction.Type * existingTransaction.Amount;

                if (newBalance < 0) return (null, "No balance for revert");

                var revertDateTime = _transactionRepository.RevertTransaction(transactionId);  //reverting  

                if (revertDateTime != null) //just reverted 
                {
                    _clientRepository.UpdateBalance(existingTransaction.ClientId, newBalance);

                    return (new RevertResponse()
                        { 
                            ClientBalance = newBalance, 
                            RevertDateTime = (DateTime)revertDateTime
                        }, string.Empty);
                }
            }

            return (null, $"no transaction {transactionId} found");
        }
    }
}
