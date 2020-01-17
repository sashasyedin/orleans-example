using Orleans;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;
using OrleansExample.Contracts;
using OrleansExample.Grains.DataModels;
using System;
using System.Threading.Tasks;

namespace OrleansExample.Grains
{
    public class AccountGrain : Grain, IAccountGrain
    {
        private readonly ITransactionalState<BalanceState> _balanceState;
        private readonly IPersistentState<NameState> _nameState;

        public AccountGrain(
            [TransactionalState("balanceState", "Storage1")] ITransactionalState<BalanceState> balanceState,
            [PersistentState("nameState", "Storage1")] IPersistentState<NameState> nameState)
        {
            _balanceState = balanceState ?? throw new ArgumentNullException(nameof(balanceState));
            _nameState = nameState ?? throw new ArgumentNullException(nameof(nameState));
        }

        async Task IAccountGrain.CreateAccount(string name)
        {
            _nameState.State.Name = name;
            await _nameState.WriteStateAsync();
        }

        Task IAccountGrain.Deposit(uint amount)
        {
            return _balanceState.PerformUpdate(x => x.Balance += amount);
        }

        Task IAccountGrain.Withdraw(uint amount)
        {
            return _balanceState.PerformUpdate(x => x.Balance -= amount);
        }

        Task<uint> IAccountGrain.GetBalance()
        {
            return _balanceState.PerformRead(x => x.Balance);
        }
    }
}
