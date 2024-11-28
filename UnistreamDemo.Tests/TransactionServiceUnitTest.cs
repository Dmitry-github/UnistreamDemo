namespace UnistreamDemo.Tests
{
    using Moq;
    using WebApi.Interfaces;
    using WebApi.Services;

    public class TransactionServiceUnitTest
    {
        private readonly Mock<IClientRepository> _clientRepository;
        private readonly Mock<ITransactionRepository> _transactionRepository;

        public TransactionServiceUnitTest()
        {
            _clientRepository = new Mock<IClientRepository>();
            _transactionRepository = new Mock<ITransactionRepository>();
        }

        [Fact]
        public void GetZeroBalanceTest()
        {
            var zeroBalanceClientId = new Guid("66775B21-2B0E-4569-BE90-05A603000000");

            _clientRepository.Setup(cr => cr.GetBalance(zeroBalanceClientId)).Returns(0);

            var transactionService = new TransactionService(_clientRepository.Object, _transactionRepository.Object);

            Assert.NotNull(transactionService);

            var balanceResult = transactionService.GetClientBalanceAsync(zeroBalanceClientId).Result;

            Assert.NotNull(balanceResult);
            Assert.Equal( 0, balanceResult.ClientBalance);
        }

        [Fact]
        public void RevertNonExistingTransactionTest()
        {
            var transactionId = new Guid("00000000-0000-0000-0000-000000000001");

            var transactionService = new TransactionService(_clientRepository.Object, _transactionRepository.Object);

            Assert.NotNull(transactionService);

            var (revertResponse, text ) = transactionService.RevertAsync(transactionId).Result;

            Assert.NotEqual(string.Empty, text);
            Assert.Null(revertResponse);
        }
    }
}