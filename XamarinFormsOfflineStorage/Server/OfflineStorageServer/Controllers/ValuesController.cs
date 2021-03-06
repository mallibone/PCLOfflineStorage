﻿using System.Collections.Generic;
using System.Web.Http;
using OfflineStorageServer.Models;

namespace OfflineStorageServer.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<TheValueObject> Get()
        {
            var valueObjects = new List<TheValueObject>
            {
                new TheValueObject {Name = "Noser", ImageUri = "http://www.noser.com/de/themes/noser_home_de/images/logo.jpg", CompanyUri = "http://www.noser.com"},
                new TheValueObject {Name = "Xamarin", ImageUri = "https://xamarin.com/content/images/pages/branding/assets/xamarin-logo.png", CompanyUri = "http://www.xamarin.com"},
                new TheValueObject {Name = "Microsoft", ImageUri = "http://blogs.microsoft.com/wp-content/uploads/2012/08/8867.Microsoft_5F00_Logo_2D00_for_2D00_screen.jpg", CompanyUri = "http://www.microsoft.com"},
            };

            return valueObjects;
        }
    }
}
