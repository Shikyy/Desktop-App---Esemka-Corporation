using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsemkaCorporation
{
    internal class LoggedEmployee
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public DateTime hireDate { get; set; }
        public string position {  get; set; }
        public string jobLevel { get; set; }
        public string department { get; set; }
        public int supervisorId { get; set; }

        public LoggedEmployee(int id, string name, string email, string phoneNumber, DateTime hireDate, string position, string jobLevel, string department, int supervisorId)
        {
            this.id = id;
            this.name = name;
            this.email = email;
            this.phoneNumber = phoneNumber;
            this.hireDate = hireDate;
            this.position = position;
            this.jobLevel = jobLevel;
            this.department = department;
            this.supervisorId = supervisorId;
        }
    }
}
