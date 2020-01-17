using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansExample.API.Models;
using OrleansExample.Contracts;
using System;
using System.Threading.Tasks;

namespace OrleansExample.API.Controllers
{
    [Route("api/atm")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IClusterClient _client;

        public BankController(IClusterClient client)
        {
            _client = client;
        }

        [HttpGet("{id}")]
        public async Task<string> GetAtmName(Guid id)
        {
            var grain = _client.GetGrain<IAtmGrain>(id);
            return await grain.GetAtmName();
        }

        [HttpPut("{id}")]
        public async Task UpdateAtmName(Guid id, string name)
        {
            var grain = _client.GetGrain<IAtmGrain>(id);
            await grain.SetAtmName(name);
        }

        [HttpPost("account")]
        public async Task CreateAccount(Guid id, string name)
        {
            var grain = _client.GetGrain<IAccountGrain>(id);
            await grain.CreateAccount(name);
        }

        [HttpPost("transfer")]
        public async Task TransferMoney(Guid id, [FromBody]TransferMoneyRequest request)
        {
            var grain = _client.GetGrain<IAtmGrain>(id);
            await grain.Transfer(request.From, request.To, request.Amount);
        }
    }
}
