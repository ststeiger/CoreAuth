
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Xml;
using Microsoft.AspNetCore.Authentication.JwtBearer;


// http://andrewlock.net/a-look-behind-the-jwt-bearer-authentication-middleware-in-asp-net-core/
// https://goblincoding.com/2016/07/03/issuing-and-authenticating-jwt-tokens-in-asp-net-core-webapi-part-i/

// https://github.com/aspnet/Security/tree/master/src/Microsoft.AspNetCore.Authentication.JwtBearer
// https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication.JwtBearer/Events/IJwtBearerEvents.cs
namespace NiHaoCookie
{


    // https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication.JwtBearer/Events/JwtBearerEvents.cs
    public class CustomBearerEvents : Microsoft.AspNetCore.Authentication.JwtBearer.IJwtBearerEvents
    {

        /// <summary>
        /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.FromResult(0);

        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.FromResult(0);

        /// <summary>
        /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
        /// </summary>
        public Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = context => Task.FromResult(0);


        /// <summary>
        /// Invoked before a challenge is sent back to the caller.
        /// </summary>
        public Func<JwtBearerChallengeContext, Task> OnChallenge { get; set; } = context => Task.FromResult(0);


        Task IJwtBearerEvents.AuthenticationFailed(AuthenticationFailedContext context)
        {
            return OnAuthenticationFailed(context);
        }

        Task IJwtBearerEvents.Challenge(JwtBearerChallengeContext context)
        {
            return OnChallenge(context);
        }

        Task IJwtBearerEvents.MessageReceived(MessageReceivedContext context)
        {
            return OnMessageReceived(context);
        }

        Task IJwtBearerEvents.TokenValidated(TokenValidatedContext context)
        {
            return OnTokenValidated(context);
        }
    }


    // https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication.JwtBearer/Events/IJwtBearerEvents.cs
    // http://codereview.stackexchange.com/questions/45974/web-api-2-authentication-with-jwt
    public class TokenMaker
    {

        class SecurityConstants
        {
            public static string TokenIssuer;
            public static string TokenAudience;
            public static int TokenLifetimeMinutes;
        }


        public static string IssueToken()
        {
            SecurityKey sSKey = null;

            var claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "userName"),
                new Claim(ClaimTypes.Role, "role")     //Not sure what this is for
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor desc = makeSecurityTokenDescriptor(sSKey, claimList);

            // JwtSecurityToken tok = tokenHandler.CreateJwtSecurityToken(desc);
            return tokenHandler.CreateEncodedJwt(desc);
        }

        
        public static ClaimsPrincipal ValidateJwtToken(string jwtToken)
        {
            SecurityKey sSKey = null;
            var tokenHandler = new JwtSecurityTokenHandler();

            // Parse JWT from the Base64UrlEncoded wire form 
            //(<Base64UrlEncoded header>.<Base64UrlEncoded body>.<signature>)
            JwtSecurityToken parsedJwt = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            TokenValidationParameters validationParams =
                new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidAudience = SecurityConstants.TokenAudience,
                    ValidIssuers = new List<string>() { SecurityConstants.TokenIssuer },
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = sSKey,
                    
                };

            SecurityToken secT;
            return tokenHandler.ValidateToken("token", validationParams, out secT);
        }


        private static SecurityTokenDescriptor makeSecurityTokenDescriptor(SecurityKey sSKey, List<Claim> claimList)
        {
            var now = DateTime.UtcNow;
            Claim[] claims = claimList.ToArray();
            return new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = SecurityConstants.TokenIssuer,
                Audience = SecurityConstants.TokenAudience,
                IssuedAt = System.DateTime.UtcNow,
                Expires = System.DateTime.UtcNow.AddMinutes(SecurityConstants.TokenLifetimeMinutes),
                NotBefore = System.DateTime.UtcNow.AddTicks(-1),

                SigningCredentials = new SigningCredentials(sSKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.EcdsaSha512Signature)
            };

            

        }

    }



    // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/master/src/Microsoft.IdentityModel.Tokens/CryptoProviderFactory.cs
    // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/master/test/Microsoft.IdentityModel.Tokens.Tests/CustomCryptoProviders.cs
    public class CustomCryptoProvider : ICryptoProvider
    {
        public SignatureProvider SignatureProvider { get; set; }

        public IList<string> AdditionalHashAlgorithms { get; private set; } = new List<string>();

        public System.Security.Cryptography.HashAlgorithm HashAlgorithm { get; set; }

        public bool IsSupportedResult { get; set; } = false;

        public bool CreateCalled { get; set; } = false;

        public bool IsSupportedAlgorithmCalled { get; set; } = false;

        public bool ReleaseCalled { get; set; } = false;

        public object Create(string algorithm, params object[] args)
        {
            CreateCalled = true;
            if (IsHashAlgorithm(algorithm))
                return HashAlgorithm;
            else
                return SignatureProvider;
        }

        public bool IsHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case SecurityAlgorithms.Sha256:
                case SecurityAlgorithms.Sha256Digest:
                case SecurityAlgorithms.Sha384:
                case SecurityAlgorithms.Sha384Digest:
                case SecurityAlgorithms.Sha512:
                case SecurityAlgorithms.Sha512Digest:
                    return true;
            }


            foreach (var alg in AdditionalHashAlgorithms)
                if (alg.Equals(algorithm, StringComparison.Ordinal))
                    return true;

            return false;

        }

        public bool IsSupportedAlgorithm(string algorithm, params object[] args)
        {
            IsSupportedAlgorithmCalled = true;
            return IsSupportedResult;
        }

        public void Release(object cryptoObject)
        {
            ReleaseCalled = true;
            if (cryptoObject as ICustomObject != null)
                return;

            var disposableObject = cryptoObject as IDisposable;
            if (disposableObject != null)
                disposableObject.Dispose();
        }
    }

    public interface ICustomObject { }

    public class CustomHashAlgorithm : System.Security.Cryptography.SHA256, ICustomObject
    {
        public bool DisposeCalled { get; set; } = false;

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            DisposeCalled = true;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            throw new NotImplementedException();
        }

        protected override byte[] HashFinal()
        {
            throw new NotImplementedException();
        }
    }






    // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/tree/master/src/Microsoft.IdentityModel.Tokens
    // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/master/src/Microsoft.IdentityModel.Tokens/SecurityToken.cs
    public class MySecurityToken : Microsoft.IdentityModel.Tokens.SecurityToken
    {

        private string m_ID;
        private string m_Issuer;
        private SecurityKey m_SecurityKey;
        private SecurityKey m_SigningKey;

        private System.DateTime m_validFrom;
        private System.DateTime m_ValidTo;

        public MySecurityToken()
        {
            m_validFrom = System.DateTime.Today;
            m_ValidTo = System.DateTime.Today.AddDays(1).AddTicks(-1);
        }

        public override string Id { get { return m_ID; } }
        public override string Issuer { get { return m_Issuer; } }
        public override SecurityKey SecurityKey { get { return m_SecurityKey; } }

        // Remarks:
        //     Microsoft.IdentityModel.Tokens.ISecurityTokenValidator.ValidateToken(...) can
        //     this value when a Microsoft.IdentityModel.Tokens.SecurityToken.SecurityKey is
        //     used to successfully validate a signature.
        public override SecurityKey SigningKey { get { return m_SigningKey; } set { } }

        public override DateTime ValidFrom { get { return m_validFrom; } }
        public override DateTime ValidTo { get { return System.DateTime.Today.AddDays(1).AddTicks(-1); } }
    }



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

