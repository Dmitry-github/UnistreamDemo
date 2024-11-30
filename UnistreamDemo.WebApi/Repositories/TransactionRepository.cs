namespace UnistreamDemo.WebApi.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TransactionRepository: ITransactionRepository
    {
        //non-readoly for Storage Emulation

        private ConcurrentBag<Transaction> _transactions = new ConcurrentBag<Transaction>();

        public TransactionRepository() { }
        
        public async Task<Transaction> GetTransactionAsync(Guid transactionId)
        {
            return  _transactions.FirstOrDefault(t => t.Id == transactionId);
        }

        public async Task<bool> AddTransactionAsync(Transaction transaction)
        {
            if (transaction == null) return false;
            _transactions.Add(transaction);
            return true;
        }

        public async Task<Transaction> DeleteTransactionAsync(Guid transactionId)
        {
            return !_transactions.IsEmpty && _transactions.TryTake(out var transaction) ? transaction : null;
        }

        public async Task<DateTime?> RevertTransactionAsync(Guid transactionId)
        {
            if (_transactions.Count > 0)
            {
                var transaction =
                    _transactions.FirstOrDefault(t => t.Id == transactionId && t.RevertDateTime == null);

                if (transaction != null)
                {
                    transaction.RevertDateTime = DateTime.Now;
                    return transaction.RevertDateTime;
                }
            }
            return null;
        }
    }
}
