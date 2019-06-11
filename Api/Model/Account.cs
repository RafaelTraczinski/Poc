using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Model
{
    public class Account
    {

        public bool Authenticated { get; set; }

        public bool TwoFactorIsEnabled { get; set; }

        public bool UserHasTwoFactorCode { get; set; }

        public string Token { get; set; }

        public string message { get; set; }
    }
}
