using ProjectManager.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DAL.Component
{
    public class UserComponent : IUser
    {

        public void AddUser(DAL.User objUser)
        {
            try
            {
                using (ProjectManagerEntities context = new ProjectManagerEntities())
                {
                    var usr = new User()
                    {
                        First_Name = objUser.First_Name,
                        Last_Name = objUser.Last_Name,
                        Employee_ID = objUser.Employee_ID,
                        Project_ID = objUser.Project_ID,
                        Task_ID = objUser.Task_ID
                    };
                    context.Users.Add(usr);
                    context.SaveChanges();
                }                                 
            }
            catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public void DeleteUser(int iUserID)
        {
            try
            {
                using (ProjectManagerEntities context = new ProjectManagerEntities())
                {
                    User user = GetUsersByID(iUserID);
                    if(user != null)
                    {
                        User usrDelete = new User { User_ID = iUserID };
                        context.Entry(usrDelete).State = EntityState.Deleted;
                        context.SaveChanges();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public List<DAL.User> GetUsers()
        {
            using (ProjectManagerEntities context = new ProjectManagerEntities())
            {
                var user = (from p in context.Users                            
                                select new
                                {                                    
                                    First_Name = p.First_Name,
                                    Last_Name = p.Last_Name,
                                    Employee_ID = p.Employee_ID,
                                    Project_ID = p.Project_ID,
                                    Task_ID = p.Task_ID,
                                    User_ID = p.User_ID
                                }).ToList()
                                .Select(x => new User()
                                {
                                    First_Name = x.First_Name,
                                    Last_Name = x.Last_Name,
                                    Employee_ID = x.Employee_ID,
                                    Project_ID = x.Project_ID,
                                    Task_ID = x.Task_ID,
                                    User_ID = x.User_ID
                                });
                return user.ToList();
            }                
        }

        public DAL.User GetUsersByID(int iUserID)
        {
            using (ProjectManagerEntities context = new ProjectManagerEntities())
            {
                var user = (from p in context.Users
                            where p.User_ID == iUserID
                            select new
                            {
                                First_Name = p.First_Name,
                                Last_Name = p.Last_Name,
                                Employee_ID = p.Employee_ID,
                                Project_ID = p.Project_ID,
                                Task_ID = p.Task_ID,
                                User_ID = p.User_ID
                            }).ToList()
                                .Select(x => new User()
                                {
                                    First_Name = x.First_Name,
                                    Last_Name = x.Last_Name,
                                    Employee_ID = x.Employee_ID,
                                    Project_ID = x.Project_ID,
                                    Task_ID = x.Task_ID,
                                    User_ID = x.User_ID
                                });
                return user.FirstOrDefault();
            }                
        }

        public void UpdateUser(DAL.User objUser)
        {
            try
            {
                using (ProjectManagerEntities context = new ProjectManagerEntities())
                {
                    var result = context.Users.SingleOrDefault(b => b.User_ID == objUser.User_ID);
                    if (result != null)
                    {
                        result.First_Name = objUser.First_Name;
                        result.Last_Name = objUser.Last_Name;
                        result.Employee_ID = objUser.Employee_ID;
                        result.Project_ID = objUser.Project_ID;
                        result.Task_ID = objUser.Task_ID;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
