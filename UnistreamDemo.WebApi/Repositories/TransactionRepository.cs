namespace UnistreamDemo.WebApi.Repositories
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class TransactionRepository: ITransactionRepository
    {
        //non-readoly for Storage Emulation

        private IList<Transaction> _transactions = new List<Transaction>();
        private static readonly object _locker = new object();

        public TransactionRepository() { }
        
        public Transaction GetTransaction(Guid transactionId)
        {
            //return !_transactions.IsEmpty && _transactions.TryPeek(out var transaction) ? transaction : null;
            lock (_locker)
            {
                return _transactions.FirstOrDefault(t => t.Id == transactionId);
            }
        }

        public bool AddTransaction(Transaction transaction)
        {
            if (transaction != null)
            {
                lock (_locker)
                {
                    _transactions.Add(transaction);
                    return true;
                }
            }
            return false;
        }

        public Transaction DeleteTransaction(Guid transactionId)
        {
            lock (_locker)
            {
                if (_transactions.Count > 0)
                {
                    var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
                    _transactions.Remove(transaction); 
                    return transaction;
                }
            }

            return null;
        }

        public DateTime? RevertTransaction(Guid transactionId)
        {
            lock (_locker)
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
            }

            return null;
        }
    }
}
