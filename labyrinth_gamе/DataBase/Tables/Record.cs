using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labyrinth_gamе.DataBase.Tables
{
    public class Record
    {
        public int RecordId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public User User { get; set; }

        public int Level { get; set; }
        public string Time { get; set; }

    }
}
