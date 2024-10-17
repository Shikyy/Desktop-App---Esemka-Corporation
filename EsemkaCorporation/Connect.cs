using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsemkaCorporation
{
    internal class Connect
    {
        public SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-9DMUOMU;Initial Catalog=EsemkaCorporation;Integrated Security=True;Encrypt=False;");
            return con;
        }
    }
}
