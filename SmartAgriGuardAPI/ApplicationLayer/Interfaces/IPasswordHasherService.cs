using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IPasswordHasherService
    {
        public void CreatePasswordHash(string password, string userName, out byte[] passwordHash);

        public bool VerifyPasswordHash(string password, string userName, byte[] storedHash);
    }
}
