/*using System.IdentityModel.Tokens.Jwt;

namespace HalloDocMVC.Models
{
    public static class CV
    {
        private static IHttpContextAccessor _httpContextAccessor;

        static CV()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public class DecodedToken
        {
            private List<string> audience1;
            private List<(string Type, string Value)> claims1;
            private DateTime validTo;
            private string signatureAlgorithm;
            private string rawData;
            private string subject;
            private DateTime validFrom;
            private string encodedHeader;
            private string encodedPayload;

            public DecodedToken(string keyId, string issuer, List<string> audience1, List<(string Type, string Value)> claims1, DateTime validTo, string signatureAlgorithm, string rawData, string subject, DateTime validFrom, string encodedHeader, string encodedPayload)
            {
                this.keyId = keyId;
                Issuer = issuer;
                this.audience1 = audience1;
                this.claims1 = claims1;
                this.validTo = validTo;
                this.signatureAlgorithm = signatureAlgorithm;
                this.rawData = rawData;
                this.subject = subject;
                this.validFrom = validFrom;
                this.encodedHeader = encodedHeader;
                this.encodedPayload = encodedPayload;
            }

            public string? keyId { get; set; }
            public string? Issuer { get; set; }
            public string? audience { get; set; }
            public string? claims { get; set; }
            public string? ValidTo { get; set; }
            public string? SignatureAlgorithm { get; set; }
            public string? RawData { get; set; }
            public string? Subject { get; set; }
            public string? ValidFrom { get; set; }
            public string? EncodedHeader { get; set; }
            public string? EncodedPayload { get; set; }
        }
        public static string? role()
        {
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(cookieValue);
            var tokenS = jsonToken as JwtSecurityToken;

            var d = ConvertJwtStringToJwtSecurityToken(cookieValue);
            var dd = DecodeJwt(d);
            var jti = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            return jti;
        }
        public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token;
        }

        public static DecodedToken DecodeJwt(JwtSecurityToken token)
        {
            var keyId = token.Header.Kid;
            var audience = token.Audiences.ToList();
            var claims = token.Claims.Select(claim => (claim.Type, claim.Value)).ToList();

            return new DecodedToken(
                keyId,
                token.Issuer,
                audience,
                claims,
                token.ValidTo,
                token.SignatureAlgorithm,
                token.RawData,
                token.Subject,
                token.ValidFrom,
                token.EncodedHeader,
                token.EncodedPayload
            );
        }
        public static string? UserName()
        {
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(cookieValue);
            var tokenS = jsonToken as JwtSecurityToken;
            var UserName = tokenS.Claims.First(claim => claim.Type == "Username").Value;
            return UserName;
        }

        public static string? UserID()
        {
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(cookieValue);
            var tokenS = jsonToken as JwtSecurityToken;
            var UserID = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            return UserID;
        }
        public static string? CurrentStatus()
        {
            string? Status = _httpContextAccessor.HttpContext.Request.Cookies["Status"];
            return Status;
        }

        public static string? Filter()
        {
            string? Filter = _httpContextAccessor.HttpContext.Request.Cookies["Filter"];
            return Filter;
        }
    }
}*/
/*GITHUB FILE*/