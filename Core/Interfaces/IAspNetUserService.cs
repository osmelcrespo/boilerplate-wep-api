using Core.Models;

namespace Core.Interfaces
{
    public interface IAspNetUserService
    {
        Task<ResponseData> PostAspNetUserUser(AspNetUser applicationUser);
        Task<ResponseData> PutAspNetUserUser(AspNetUser applicationUser);
        Task<ResponseData> GetUsers(PaginatorData paginatorData);
        Task<ResponseData> GetUser(string id);
    }
}
