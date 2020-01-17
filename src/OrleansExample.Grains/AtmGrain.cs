using Orleans;
using Orleans.Runtime;
using OrleansExample.Contracts;
using System;
using System.Threading.Tasks;

namespace OrleansExample.Grains
{
    public class AtmGrain : Grain, IAtmGrain
    {
        private readonly IPersistentState<AtmState> _atmState;

        public AtmGrain(
            [PersistentState("atmState", "Storage2")] IPersistentState<AtmState> atmState)
        {
            _atmState = atmState ?? throw new ArgumentNullException(nameof(atmState));
        }

        Task IAtmGrain.Transfer(Guid fromAccount, Guid toAccount, uint amountToTransfer)
        {
            var from = GrainFactory.GetGrain<IAccountGrain>(fromAccount);
            var to = GrainFactory.GetGrain<IAccountGrain>(toAccount);

            return Task.WhenAll(
                from.Withdraw(amountToTransfer),
                to.Deposit(amountToTransfer));
        }

        async Task<string> IAtmGrain.GetAtmName()
        {
            await _atmState.ReadStateAsync();
            return _atmState.State.Name;
        }

        async Task IAtmGrain.SetAtmName(string name)
        {
            _atmState.State.Name = name;
            await _atmState.WriteStateAsync();
        }
    }

    [Serializable]
    public class AtmState
    {
        public string Name { get; set; }
    }
}
