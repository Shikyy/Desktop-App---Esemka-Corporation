using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsemkaCorporation
{
    internal class LoggedInEmployee
    {
        private static LoggedEmployee _loggedEmployee;
       
        public static LoggedEmployee GetLoggedEmployee()
        {
            return _loggedEmployee;
        }

        public static void SetLoggedEmployee(LoggedEmployee loggedEmployee)
        {
            _loggedEmployee = loggedEmployee;
        }
    }
}
