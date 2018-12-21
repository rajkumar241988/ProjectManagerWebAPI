using ProjectManager.BusinessEntities;
using System.Collections.Generic;

namespace ProjectManager.BL
{
    public interface IUserServices
    {
        UserEntity GetUserById(int userId);
        IEnumerable<UserEntity> GetAllUsers();
        int CreateUsers(UserEntity userEntity);
        bool UpdateUser(int userId, UserEntity userEntity);
        bool DeleteUser(int userId);
    }
}
