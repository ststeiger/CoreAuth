
using System.Collections.Generic;


// Dependency hell
// using Microsoft.AspNetCore.Http; // for WriteAsync 
using Microsoft.AspNetCore.Authorization;


namespace NiHaoCookie
{


    public class CustomAuthorizationPolicy : AuthorizationPolicy
    {


        public CustomAuthorizationPolicy(
             IEnumerable<IAuthorizationRequirement> requirements
            ,IEnumerable<string> authenticationSchemes) 
            : base(requirements, authenticationSchemes)
        {

        } // End Constructor 


    } // End Class CustomAuthorizationPolicy 


} // End Namespace 
