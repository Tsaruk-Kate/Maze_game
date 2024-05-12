using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labyrinth_gamе.DataBase.Tables
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Key { get; set; }

        public static User CurrentUser { get; set; }
    }
}
