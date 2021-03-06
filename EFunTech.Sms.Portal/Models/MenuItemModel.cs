﻿using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    //public class MenuItemModel
    //{
    //    public int Id { get; set; }

    //    public int Level { get; set; }

    //    public int? ParentId { get; set; }

    //    public string Name { get; set; }

    //    public string MapRouteUrl { get; set; }

    //    public WebAuthorizationModel WebAuthorization { get; set; }

    //}

    public class MenuItemModel
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public string MapRouteUrl { get; set; }

        //public WebAuthorizationModel WebAuthorization { get; set; }
        public string ControllerName { get; set; }
        public string Roles { get; set; }
        public string ActionName { get; set; }
    }
}
