namespace UnistreamDemo.WebApi.Queries
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using FluentValidation;

    public class TransactionQuery : ITransaction
    {
        public Guid Id { get; set; }
        
        public Guid ClientId { get; set; }

        //public DateTime DateTime => DateTime.Now;
        public DateTime DateTime { get; set; }

        public decimal Amount { get; set; }

        //public static bool IsValid<T>(T obj, out ICollection<ValidationResult> results) where T : class
        //{
        //    var validationContext = new ValidationContext(obj);
        //    results = new List<ValidationResult>();
        //    return Validator.TryValidateObject(obj, validationContext, results, true);
        //}
    }

    public class TransactionQueryValidator : AbstractValidator<TransactionQuery>
    {
        public TransactionQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Transaction Id required");
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("Client Id required");
            RuleFor(x => x.Amount).Must(v => v > 0).WithMessage("Transaction Amount must be more then 0");
            
            RuleFor(x => x.DateTime).NotEmpty().WithMessage("Transaction DateTime should not be empty");
            RuleFor(x => x.DateTime).Must(d => d < DateTime.Now).WithMessage("Transaction DateTime should not be in future time");
        }
    }
}
