using ASPUnit20.Models.Services;
using ASPUnit20.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ASPUnit20.Models.Services
{
    internal class PartySourceFactory
    {
        public IPartySource Create()
        {
            return new ScraperPartySource();
        }
    }
}
