using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ASPUnit20.Models
{
    public interface IPartySource
    {
        string GetParty(string memberId);
    }
}