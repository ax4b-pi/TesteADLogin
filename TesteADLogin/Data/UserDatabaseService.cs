using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteADLogin.Model;
using Microsoft.EntityFrameworkCore;

namespace TesteADLogin.Data
{
    public class UserDatabaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserDatabaseService> _logger;

        public UserDatabaseService(ApplicationDbContext context, ILogger<UserDatabaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDatabase> GetUserByAzureIdAsync(string azureId)
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.AzureAdId == azureId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with Azure ID: {AzureId}", azureId);
                throw;
            }
        }
    }
}
