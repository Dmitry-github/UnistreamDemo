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
            var currentBalance = await  _clientRepository.GetBalanceOrDefaultAsync(transactionQuery.ClientId);
            
            if (currentBalance == null) return (false, "(client id)");

            var newBalance = (decimal)currentBalance +
                             Transaction.GetMultiplier(transactionType) * transactionQuery.Amount;

            return (newBalance > 0, "(balance)");
        }

        public async Task<TransactionResponse> CommitCreditAsync(TransactionQuery transactionQuery)
        {
            return await CommitTransaction(transactionQuery, TransactionType.Credit);
        }

        public async Task<TransactionResponse> CommitDebitAsync(TransactionQuery transactionQuery)
        {
            return await CommitTransaction(transactionQuery, TransactionType.Debit);
        }

        public async Task<(RevertResponse, string)> RevertAsync(Guid transactionId)
        {
            return await RevertTransaction(transactionId);
        }

        public async Task<BalanceResponse> GetClientBalanceAsync(Guid clientId)
        {

            var balance = await _clientRepository.GetBalanceOrDefaultAsync(clientId);

            var balanceResponse = balance == null
                ? null
                : new BalanceResponse()
                {
                    ClientBalance = balance,
                    BalanceDateTime = DateTime.Now
                };

            return balanceResponse;
        }

        private async Task<TransactionResponse> CommitTransaction(TransactionQuery transactionQuery, TransactionType transactionType)
        {
            TransactionResponse transactionResponse;
            var existingTransaction = await _transactionRepository.GetTransactionAsync(transactionQuery.Id);
            
            if (existingTransaction != null)
            {
                var clientBalance =  await _clientRepository.GetBalanceOrDefaultAsync(existingTransaction.ClientId) ?? Decimal.MinValue;
                
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

                var currentBalance = await _clientRepository.GetBalanceOrDefaultAsync(transactionQuery.ClientId) ?? Decimal.MinValue;
                
                if (currentBalance == decimal.MinValue) return null;

                currentBalance += Transaction.GetMultiplier(transactionType) * transactionQuery.Amount;

                //TODO: check both success
                await _transactionRepository.AddTransactionAsync(newTransaction);
                await _clientRepository.UpdateBalanceAsync(transactionQuery.ClientId, currentBalance);

                transactionResponse = new TransactionResponse()
                {
                    ClientBalance = currentBalance,
                    InsertDateTime = newTransaction.DateTime
                };
            }

            return transactionResponse;
        }

        private async Task<(RevertResponse, string)> RevertTransaction(Guid transactionId)
        {
            var existingTransaction = await _transactionRepository.GetTransactionAsync(transactionId);

            if (existingTransaction?.ClientId != null)
            {
                var currentBalance = await _clientRepository.GetBalanceOrDefaultAsync(existingTransaction.ClientId) ?? Decimal.MinValue;

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

                var newBalance = currentBalance + 
                                 -1 * Transaction.GetMultiplier(existingTransaction.Type) * existingTransaction.Amount;

                if (newBalance < 0) return (null, "No balance for revert");

                var revertDateTime =await _transactionRepository.RevertTransactionAsync(transactionId);  //reverting  

                if (revertDateTime != null) //just reverted 
                {
                    await _clientRepository.UpdateBalanceAsync(existingTransaction.ClientId, newBalance);

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
