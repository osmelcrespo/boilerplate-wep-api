using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class AspNetUserService : IAspNetUserService
    {
        private readonly IAspNetUserRepo _aspNetUserRepo;

        public AspNetUserService(IAspNetUserRepo aspNetUserRepo)
        {
            _aspNetUserRepo = aspNetUserRepo;
        }

        public async Task<ResponseData> PostAspNetUserUser(AspNetUser applicationUser)
        {
            var responseData = new ResponseData();

            responseData = await _aspNetUserRepo.PostAspNetUserUser(applicationUser);
            if (responseData.Error)
            {
                return responseData;
            }

            return responseData;
        }

        public async Task<ResponseData> PutAspNetUserUser(AspNetUser applicationUser)
        {
            return await _aspNetUserRepo.PutAspNetUserUser(applicationUser);
        }

        public async Task<ResponseData> GetUsers(PaginatorData paginatorData)
        {
            var userResponse = await _aspNetUserRepo.GetUsers(paginatorData);
            if (userResponse.Error)
            {
                return userResponse;
            }

            var aspNetUser = (List<AspNetUser>)userResponse.Data;
            var paginatorResult = Paginator<AspNetUser>.Create(aspNetUser, paginatorData);

            addFields(paginatorResult);

            userResponse.Data = paginatorResult;

            return userResponse;
        }

        public async Task<ResponseData> GetUser(string id)
        {
            return await _aspNetUserRepo.GetUser(id);
        }

        private void addFields(Paginator<AspNetUser> paginatorResult)
        {
            var _fieldData = new FieldData
            {
                Order = 1,
                Name = "id",
                Field = "id",
                Type = "text",
                Display = false,
                ColSize = 1
            };

            paginatorResult.fields = new List<FieldData>
            {
                _fieldData
            };

            _fieldData = new FieldData
            {
                Order = 2,
                Name = "Username",
                Field = "userName",
                Type = "text",
                Display = true,
                ColSize = 20,
                Sort = "asc"
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new FieldData
            {
                Order = 3,
                Name = "Email",
                Field = "email",
                Type = "text",
                Display = true,
                ColSize = 10
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new FieldData
            {
                Order = 4,
                Name = "Phone",
                Field = "phoneNumber",
                Type = "text",
                Display = true,
                ColSize = 10
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new FieldData
            {
                Order = 5,
                Name = "Name",
                Field = "name",
                Type = "text",
                Display = true,
                ColSize = 20
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new FieldData
            {
                Order = 6,
                Name = "Last Name",
                Field = "lastName",
                Type = "text",
                Display = true,
                ColSize = 20
            };

            paginatorResult.fields.Add(_fieldData);
        }
    }
}
