namespace UnistreamDemo.WebApi.Responses
{
    using System;

    public class BalanceResponse
    {
        public DateTime BalanceDateTime { get; set; }
        public decimal? ClientBalance { get; set; }
    }
}
