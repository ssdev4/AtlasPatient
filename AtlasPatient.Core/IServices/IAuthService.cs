using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasPatient.Core.IServices
{
    public interface IAuthService
    {
        Task<string> GetAuthTokenAsync();
    }
}
