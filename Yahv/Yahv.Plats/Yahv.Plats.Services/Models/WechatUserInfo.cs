﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Plats.Services.Models
{
    public class WechatUserInfo
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public int sex { get; set; }
        public string language { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string[] privilege { get; set; }
        public string unionid { get; set; }
    }
}
