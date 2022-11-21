using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAuth.Data.Models
{
    public class UserVerification : User
    {
        public long UserVerificationId { get; set; }

        public bool EmailVerified { get; set; }

        public bool PhoneVerified { get; set; }
    }
}
