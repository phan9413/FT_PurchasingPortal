using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRWebApi.Models
{
    public class UserLogin
    {
        public Guid Oid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StoredPassword { get; set; }
        public int Company { get; set; }
    }
}
