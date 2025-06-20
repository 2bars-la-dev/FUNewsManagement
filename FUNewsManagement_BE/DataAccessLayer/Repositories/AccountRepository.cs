using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class AccountRepository
    {
        private readonly FunewsManagementContext _context;

        public AccountRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<SystemAccount?> GetByIdAsync(short accountId)
        {
            return await _context.SystemAccounts.FindAsync(accountId);
        }

        public async Task<IEnumerable<SystemAccount>> GetAllAsync()
        {
            return await _context.SystemAccounts.ToListAsync();
        }

        //Get by email
        public async Task<SystemAccount?> GetByEmailAsync(string email)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email);
        }

        public async Task AddAsync(SystemAccount account)
        {
            await _context.SystemAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SystemAccount account)
        {
            _context.SystemAccounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(short accountId)
        {
            var account = await _context.SystemAccounts.FindAsync(accountId);
            if (account == null)
            {
                return false;
            }

            _context.SystemAccounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(short accountId)
        {
            return await _context.SystemAccounts.AnyAsync(a => a.AccountId == accountId);
        }
    }
}
