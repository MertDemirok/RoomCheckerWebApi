using System;
using Microsoft.AspNetCore.Mvc;

namespace RoomCheckerApi.Controllers
{
    public class HelpController
    {
        public HelpController()
        {
        }

        public ObjectResult jsonResponseMessage(int id ,string Title,int ErrorCode){


           return new ObjectResult(new
            {
                ID = id,
                Title = Title,
                ErrorCode =  ErrorCode
            });
        }
    }
}
