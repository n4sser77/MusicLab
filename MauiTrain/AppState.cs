using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiTrain
{
    public class AppState
    {
        public bool IsLoggedIn { get; set; } = false;
        public string Jwt { get; set; }

        public void StoreJwt(string jwt)
        {
            Jwt = jwt;
            IsLoggedIn = true;
        }
    }



}
