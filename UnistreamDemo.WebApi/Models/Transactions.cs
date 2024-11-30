namespace UnistreamDemo.WebApi.Models
{
    using Interfaces;
    using System;

    public enum TransactionType
    {
        /// <summary>
        /// списание средств у клиента
        /// </summary>
        Debit = -1,
        /// <summary>
        ///зачисление средств клиенту 
        /// </summary>
        Credit = 1
    }

    public class Transaction: ITransaction
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime? RevertDateTime { get; set; }

        public static int GetMultiplier(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Debit:
                    return -1;
                case TransactionType.Credit:
                    return 1;
                default:
                    throw new InvalidOperationException("Unknown transaction Type");
            }
        }
    }
}