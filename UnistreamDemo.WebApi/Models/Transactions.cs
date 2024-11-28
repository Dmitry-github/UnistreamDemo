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
    }
}