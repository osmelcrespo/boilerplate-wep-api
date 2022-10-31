using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Rest
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetUsersController : ControllerBase
    {
        private readonly IAspNetUserService _aspNetUserService;

        public AspNetUsersController(IAspNetUserService aspNetUserService)
        {
            _aspNetUserService = aspNetUserService;
        }

        [HttpPut("{id}")]
        public async Task<ResponseData> PutAspNetUserUser(string id, AspNetUser applicationUser)
        {
            var responseData = new ResponseData();

            try
            {
                return await _aspNetUserService.PutAspNetUserUser(applicationUser);
            }
            catch (Exception e)
            {
                responseData.Error = true;
                responseData.ErrorValue = 2;
                responseData.Description = e.Message;
                responseData.Data = e;
            }

            return responseData;
        }

        [Route("GetUsers")]
        [HttpGet]
        public async Task<ResponseData> GetUsers([FromHeader] PaginatorData paginatorData)
        {
            return await _aspNetUserService.GetUsers(paginatorData);
        }

        [HttpGet("{id}")]
        public async Task<ResponseData> GetUser(string id)
        {
            return await _aspNetUserService.GetUser(id);
        }
    }
}
