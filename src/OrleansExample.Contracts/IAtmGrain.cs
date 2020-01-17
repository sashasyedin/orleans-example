using Orleans;
using System;
using System.Threading.Tasks;

namespace OrleansExample.Contracts
{
    public interface IAtmGrain : IGrainWithGuidKey
    {
        [Transaction(TransactionOption.Create)]
        Task Transfer(Guid fromAccount, Guid toAccount, uint amountToTransfer);

        Task<string> GetAtmName();

        Task SetAtmName(string name);
    }
}
