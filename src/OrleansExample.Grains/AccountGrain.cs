using Orleans;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;
using OrleansExample.Contracts;
using System;
using System.Threading.Tasks;

namespace OrleansExample.Grains
{
    public class AccountGrain : Grain, IAccountGrain
    {
        private readonly ITransactionalState<AccountState1> _accountState1;
        private readonly IPersistentState<AccountState2> _accountState2;

        public AccountGrain(
            [TransactionalState("accountState1", "Storage1")] ITransactionalState<AccountState1> accountState1,
            [PersistentState("accountState2", "Storage1")] IPersistentState<AccountState2> accountState2)
        {
            _accountState1 = accountState1 ?? throw new ArgumentNullException(nameof(accountState1));
            _accountState2 = accountState2 ?? throw new ArgumentNullException(nameof(accountState2));
        }

        async Task IAccountGrain.CreateAccount(string name)
        {
            _accountState2.State.Name = name;
            await _accountState2.WriteStateAsync();
        }

        Task IAccountGrain.Deposit(uint amount)
        {
            return _accountState1.PerformUpdate(x => x.Balance += amount);
        }

        Task IAccountGrain.Withdraw(uint amount)
        {
            return _accountState1.PerformUpdate(x => x.Balance -= amount);
        }

        Task<uint> IAccountGrain.GetBalance()
        {
            return _accountState1.PerformRead(x => x.Balance);
        }
    }

    [Serializable]
    public class AccountState1
    {
        public uint Balance { get; set; }
    }

    [Serializable]
    public class AccountState2
    {
        public string Name { get; set; }
    }
}
