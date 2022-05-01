using JWTExample.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTExample.Helpers.Interfaces
{
     public interface ITokenHelper
    {
        public string CreateUserToken(UserDTO request);

        public UserDTO DecodeUserToken();
    }
}
