using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DragonService.Controllers
{
    public class SettingsController : ApiController
    {
        //[HttpGet]
        //public List<KeyValuePair<string,int>> GetSettings()
        //{
        //    return 
        //}

        [HttpGet]
        public void SetSetting(string settingKey, string settingValue)
        {

        }
    }
}
