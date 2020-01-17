using System;

namespace OrleansExample.API.Models
{
    public class TransferMoneyRequest
    {
        public Guid From { get; set; }
        public Guid To { get; set; }
        public uint Amount { get; set; }
    }
}
