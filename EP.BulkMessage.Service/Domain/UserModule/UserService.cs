using EP.BulkMessage.Service.Entity;
using EP.BulkMessage.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Domain.UserModule
{
    public class UserService
    {

        public EPUser GetUser(string username)
        {
            MainUnitOfWork unitOfWork = new MainUnitOfWork();
            return unitOfWork.Session.QueryOver<EPUser>().Where(p => p.Username == username).SingleOrDefault();
        }

        public bool UpdateUser(EPUser user)
        {

            MainUnitOfWork unitOfWork = new MainUnitOfWork();
            unitOfWork.Begin();
            unitOfWork.Session.Update(user);
            unitOfWork.Commit();
            return true;

        }

        public bool UpdateUserPassword(string username, string password)
        {

            MainUnitOfWork unitOfWork = new MainUnitOfWork();
            var user = unitOfWork.Session.QueryOver<EPUser>().Where(p => p.Username == username).SingleOrDefault();
            user.Password = password;
            unitOfWork.Begin();
            unitOfWork.Session.Update(user);
            unitOfWork.Commit();
            return true;

        }

        public EPUser RegisterUser(EPUser user)
        {

            MainUnitOfWork unitOfWork = new MainUnitOfWork();
            unitOfWork.Begin();
            user.Id = (int)unitOfWork.Session.Save(user);
            unitOfWork.Commit();
            return user;

        }




    }
}