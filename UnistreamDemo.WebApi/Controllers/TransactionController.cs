namespace UnistreamDemo.WebApi.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Interfaces;
    using Models;
    using Queries;
    using FluentValidation;

    [Route("api/[action]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreditAsync(TransactionQuery request, [FromServices] IValidator<TransactionQuery> validator) 
        {
            var requestValidationResult = await RequestValidationAsync(request, validator);
            if (requestValidationResult != null)
                return requestValidationResult;

            var (validTransaction, text) = await _transactionService.TransactionIsValidAsync(request, TransactionType.Credit);
            if (!validTransaction)
            {
                return BadRequest(CreateProblemDetails("Unsuccessful transaction attempt",
                    $"Invalid Transaction for client {request.ClientId} {text}"));
            }

            var response = await _transactionService.CommitCreditAsync(request);
            return Ok(response);

        }

        [HttpPost]
        public async Task<IActionResult> DebitAsync(TransactionQuery request, [FromServices] IValidator<TransactionQuery> validator)
        {
            var requestValidationResult = await RequestValidationAsync(request, validator);
            if (requestValidationResult != null)
                return requestValidationResult;

            var(validTransaction, text) = await _transactionService.TransactionIsValidAsync(request, TransactionType.Debit);

            if (!validTransaction)
            {
                return BadRequest(CreateProblemDetails("Unsuccessful transaction attempt",
                    $"Invalid Transaction for client {request.ClientId} {text}"));
            }

            var response = await _transactionService.CommitDebitAsync(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RevertAsync(Guid id)
        {
            var(revertResult, text) = await _transactionService.RevertAsync(id);

            if (revertResult != null && string.IsNullOrEmpty(text))
                return Ok(revertResult);
            else
            {
                return BadRequest(CreateProblemDetails("Unsuccessful transaction revert",
                    $"Invalid Transaction {id} revert: {text}"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> BalanceAsync(Guid id)
        {

            var result = await _transactionService.GetClientBalanceAsync(id);

            return result == null
                ? BadRequest(CreateProblemDetails("Unsuccessful query", $"Client {id} not found"))
                : Ok(result);

        }

        private async Task<IActionResult> RequestValidationAsync(TransactionQuery request, IValidator<TransactionQuery> validator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                return BadRequest(CreateProblemDetails("Validation error(s):", errorMessages));
            }
            return null;
        }

        //TODO: Possible use Nuget Hellang.Middleware.ProblemDetails or ProblemDetailsFactory.CreateProblemDetails()
        private ProblemDetails CreateProblemDetails(string type, string detail, int status = (int)HttpStatusCode.BadRequest)
        {
            return new ProblemDetails()
            {
                Type = type,
                Title = "Error",
                Status = status,
                Detail = $"{type}: {detail}",
                Instance = Request.Path
            };
        }
    }
}
