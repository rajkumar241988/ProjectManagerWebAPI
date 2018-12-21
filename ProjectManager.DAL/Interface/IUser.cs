using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DAL.Interface
{
    public interface IUser
    {
        List<User> GetUsers();
        User GetUsersByID(int iUserID);
        void AddUser(User objUser);
        void UpdateUser(User objUser);
        void DeleteUser(int iUserID);
    }
}
