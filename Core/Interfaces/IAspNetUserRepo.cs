using Core.Models;

namespace Core.Interfaces
{
    public interface IAspNetUserRepo
    {
        Task<ResponseData> PostAspNetUserUser(AspNetUser applicationUser);
        Task<ResponseData> PutAspNetUserUser(AspNetUser applicationUser);
        Task<ResponseData> GetUser(string userId);
        Task<ResponseData> GetUsers(PaginatorData paginatorData);
    }
}
