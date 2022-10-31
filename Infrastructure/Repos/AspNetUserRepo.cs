using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Repos
{
    public class AspNetUserRepo : IAspNetUserRepo
    {
        private readonly AppDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public AspNetUserRepo(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ResponseData> GetUser(string userId)
        {
            var responseData = new ResponseData();

            try
            {
                responseData.Data = await _context.AspNetUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            }
            catch (Exception e)
            {
                responseData.Error = true;
                responseData.ErrorValue = 2;
                responseData.Description = e.Message;
                return responseData;
            }

            return responseData;
        }

        public async Task<ResponseData> GetUsers(PaginatorData paginatorData)
        {
            var responseData = new ResponseData();

            var users = _context.AspNetUsers.AsQueryable();
            if (users != null)
            {
                if (!string.IsNullOrEmpty(paginatorData.filterDataSt))
                {
                    users = InsertFilters(users, paginatorData.filterDataSt);
                }

                try
                {
                    if (string.IsNullOrEmpty(paginatorData.orderField))
                    {
                        paginatorData.orderField = "Id";
                    }

                    responseData.Data = await users
                        .OrderBy(paginatorData.orderField + (paginatorData.descending ? " desc" : ""))
                        .ToListAsync();
                }
                catch (Exception e)
                {
                    responseData.Error = true;
                    responseData.ErrorValue = 2;
                    responseData.Description = e.Message;
                    responseData.Data = e;
                }
            }
            else
            {
                responseData.Error = true;
                responseData.ErrorValue = 2;
                responseData.Description = "Relation not found!";
                return responseData;
            }

            return responseData;
        }

        public async Task<ResponseData> PostAspNetUserUser(AspNetUser applicationUser)
        {
            var responseData = new ResponseData();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = applicationUser.Email,
                    Email = applicationUser.Email,
                    Name = applicationUser.Name,
                    LastName = applicationUser.LastName,
                };

                var result = await _userManager.CreateAsync(user, applicationUser.Password);
                if (result.Succeeded)
                {
                    var aspNetUser = new AspNetUser
                    {
                        Id = user.Id,
                        Name = user.Name,
                        LastName = user.LastName
                    };

                    responseData.Data = aspNetUser;
                    return responseData;
                }
                else
                {
                    responseData.Error = true;
                    responseData.Description = "Bad Request";
                    responseData.OthersValidations = result;
                    return responseData;
                }
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

        public async Task<ResponseData> PutAspNetUserUser(AspNetUser applicationUser)
        {
            var responseData = new ResponseData();

            try
            {
                var dbUser = await _context.AspNetUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == applicationUser.Id);
                if (dbUser == null)
                {
                    responseData.Error = true;
                    responseData.ErrorValue = 2;
                    responseData.Description = "User not found!";
                    return responseData;
                }

                if (dbUser.PasswordHash != applicationUser.Password)
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    var user = new ApplicationUser
                    {
                        UserName = applicationUser.Email,
                        Email = applicationUser.Email,
                        Name = applicationUser.Name,
                        LastName = applicationUser.LastName,
                    };

                    applicationUser.PasswordHash = passwordHasher.HashPassword(user, applicationUser.Password);
                }

                _context.Entry(applicationUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                responseData.Error = true;
                responseData.ErrorValue = 2;
                responseData.Description = e.Message;
                responseData.Data = e;
                return responseData;
            }

            responseData.Data = applicationUser;

            return responseData;
        }

        private IQueryable<AspNetUser> InsertFilters(IQueryable<AspNetUser> users, string filterDataSt)
        {
            var jFilterDataSt = JObject.Parse(filterDataSt);
            foreach (var item in jFilterDataSt)
            {
                switch (item.Key.ToLower())
                {
                    case "username":
                        users = users.Where(c => c.UserName.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "email":
                        users = users.Where(c => c.Email.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "phonenumber":
                        users = users.Where(c => c.PhoneNumber.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "name":
                        users = users.Where(c => c.Name.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "lastname":
                        users = users.Where(c => c.LastName.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                }
            }

            return users;
        }
    }
}
