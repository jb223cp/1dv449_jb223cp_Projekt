﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoMap.Models.Entities
{
    public class User
    {
        public string id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
    }
}