using System;

namespace XamarinPR.Models
{
    public class UserLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StoredPassword { get; set; }
        public int Company { get; set; }
        public int Employee { get; set; }
        public int WhsCode { get; set; }

    }
}
