using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfinfData.Settings
{
        public class OdooConfiguracion
        {
            public string Url { get; set; } = "https://frutasborja.alfinfdoo.com";
            public string Database { get; set; } = "frescitrus";
            public string Username { get; set; } = "admin";
            public string Password { get; set; } = "20@lfinF20";
            public int TimeoutSeconds { get; set; } = 30;
        }
    
}
