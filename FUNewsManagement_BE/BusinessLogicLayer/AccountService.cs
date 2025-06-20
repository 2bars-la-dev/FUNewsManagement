using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepo;

        public AccountService(AccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        // Get all accounts asynchronously
        public async Task<IEnumerable<SystemAccount>> GetAllAsync()
        {
            return await _accountRepo.GetAllAsync();
        }

        // Get account by ID asynchronously
        public async Task<SystemAccount?> GetByIdAsync(short accountId)
        {
            return await _accountRepo.GetByIdAsync(accountId);
        }

        // Add a new account asynchronously
        public async Task AddAsync(SystemAccount account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            await _accountRepo.AddAsync(account);
        }

        // Update an existing account asynchronously
        public async Task<bool> UpdateAsync(short id, SystemAccount updated)
        {
            var existing = await _accountRepo.GetByIdAsync(id);
            if (existing == null) return false;

            // Update fields
            existing.AccountEmail = updated.AccountEmail;
            existing.AccountName = updated.AccountName;
            existing.AccountRole = updated.AccountRole;
            existing.AccountPassword = updated.AccountPassword;
            existing.NewsArticles = updated.NewsArticles;
            await _accountRepo.UpdateAsync(existing);
            return true;
        }

        // Delete an account asynchronously
        public async Task<bool> DeleteAsync(short id)
        {
            return await _accountRepo.DeleteAsync(id);
        }
    }
}
