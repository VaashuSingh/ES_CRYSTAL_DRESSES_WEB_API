using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES_CrystalDresses_WEB.Repository
{
    public interface IRepository
    {
        dynamic ValidateUser(string UName, string Pass, string CompCode, int FY);
    }
}
