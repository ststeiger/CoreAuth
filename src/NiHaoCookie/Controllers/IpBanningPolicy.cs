
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;


namespace NiHaoCookie
{


    public class IpBanningPolicy : AuthorizationPolicy
    {
        public IpBanningPolicy(IEnumerable<IAuthorizationRequirement> requirements, IEnumerable<string> authenticationSchemes) : base(requirements, authenticationSchemes)
        {

        }
    }



}