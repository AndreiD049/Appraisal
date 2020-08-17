using AppRaisal.Data;
using System.Threading.Tasks;
using AppRaisal.Data.Enitities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AppRaisal.Models
{
    public class GenericModel
    {
        private ApplicationContext DbContext
        {
            get
            {
                return new ApplicationContext();
            }
        }

        public GenericModel()
        { }

        public async Task<User> GetCurrentUser()
        {
            string username = GetCurrentUserName();
            return await (from u in DbContext.Users where u.Username == username select u).FirstOrDefaultAsync();
        }

        public string GetCurrentUserName()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        public async Task<User> CheckCreateUser()
        {
            User user = new User
            {
                Username = GetCurrentUserName()
            };
            if (await GetCurrentUser() == null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // Create the user
                    await db.Users.AddAsync(user);
                    db.SaveChanges();
                }
            }
            return user;
        }

    }
}
