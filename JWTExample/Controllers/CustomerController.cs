using JWTExample.Constants;
using JWTExample.DTOs;
using JWTExample.Helpers.Interfaces;
using JWTExample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JWTExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ITokenHelper _tokenHelper;
        public CustomerController(ITokenHelper tokenHelper, IHttpContextAccessor httpContextAccessor)
        {
            _tokenHelper = tokenHelper;
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserModel>> Login(UserDTO request)
        {
            var token = _tokenHelper.CreateUserToken(request);

            return Ok(token);
        }

        [HttpPost("GetData")]
        public async Task<ActionResult<UserModel>> GetData([FromHeader(Name = Headers.Token)] [Required] string token )
        {
           var user = _tokenHelper.DecodeUserToken();
            
            return Ok(user.Email);
        }
    }
}
