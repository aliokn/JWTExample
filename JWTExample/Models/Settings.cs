using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTExample.Models
{
    public class Settings
    {
        public string Token { get;set;}
        public string Issuer { get;set;}
        public double ExpireDate { get;set;}
    }
}
