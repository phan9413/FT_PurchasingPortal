using System;

namespace XamarinGRN.Models
{
    public class UserLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Company Company { get; set; }
        public Employee Employee { get; set; }

    }
}
