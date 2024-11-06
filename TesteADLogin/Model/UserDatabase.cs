using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteADLogin.Model
{
    public class UserDatabase
    {
        public int Id { get; set; }
        public string AzureAdId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
