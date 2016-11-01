
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Xml;

namespace NiHaoCookie
{


    public class lol : SecurityTokenHandler
    {
        public override Type TokenType
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public override bool CanReadToken(string tokenString)
        {
            return true;
        }

        public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
        {
            return null;
        }

        public override SecurityToken ReadToken(string tokenString)
        {
            return null;
        }

        public override SecurityToken ReadToken(XmlReader reader)
        {
            return null;
        }

        public override SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters)
        {
            throw new NotImplementedException();
        }

        public override void WriteToken(XmlWriter writer, SecurityToken token)
        {
            throw new NotImplementedException();
        }


        //
        // Summary:
        //     Serializes to string a token of the type handled by this instance.
        // Parameters:
        //   token: A token of type TokenType.
        // Returns: The serialized token.
        public override string WriteToken(SecurityToken token)
        {
            return "";
        }
        

    }


    // https://gist.github.com/pmhsfelix/4151369
    public class MyTokenHandler : Microsoft.IdentityModel.Tokens.ISecurityTokenValidator
    {
        private int m_MaximumTokenByteSize;

        public MyTokenHandler()
        { }

        bool ISecurityTokenValidator.CanValidateToken
        {
            get
            {
                // throw new NotImplementedException();
                return true;
            }
        }



        int ISecurityTokenValidator.MaximumTokenSizeInBytes
        {
            get
            {
                return this.m_MaximumTokenByteSize;
            }

            set
            {
                this.m_MaximumTokenByteSize = value;
            }
        }

        bool ISecurityTokenValidator.CanReadToken(string securityToken)
        {
            System.Console.WriteLine(securityToken);
            return true;
        }

        ClaimsPrincipal ISecurityTokenValidator.ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            // validatedToken = new JwtSecurityToken(securityToken);
            try
            {
                
                tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
                validatedToken = new JwtSecurityToken("jwtEncodedString");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }



            ClaimsPrincipal principal = null;
            // SecurityToken validToken = null;

            validatedToken = null;


            System.Collections.Generic.List<System.Security.Claims.Claim> ls =
                new System.Collections.Generic.List<System.Security.Claims.Claim>();

            ls.Add(
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.Name, "IcanHazUsr_éèêëïàáâäåãæóòôöõõúùûüñçø_ÉÈÊËÏÀÁÂÄÅÃÆÓÒÔÖÕÕÚÙÛÜÑÇØ 你好，世界 Привет\tмир"
                , System.Security.Claims.ClaimValueTypes.String
                )
            );

            // 

            System.Security.Claims.ClaimsIdentity id = new System.Security.Claims.ClaimsIdentity("authenticationType");
            id.AddClaims(ls);

            principal = new System.Security.Claims.ClaimsPrincipal(id);

            return principal;
            throw new NotImplementedException();
        }


    }


}
