using BusinessLogicLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var acc = await _accountService.GetByIdAsync(id);
            return acc is null ? NotFound() : Ok(acc);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SystemAccount account)
        {
            await _accountService.AddAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.AccountId }, account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(short id, [FromBody] SystemAccount account)
        {
            var success = await _accountService.UpdateAsync(id, account);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var success = await _accountService.DeleteAsync(id);
            return success ? NoContent() : BadRequest("Account is linked to existing articles and cannot be deleted.");
        }
    }
}
