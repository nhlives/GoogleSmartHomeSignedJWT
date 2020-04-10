using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

/*
 * In order to report status or to request sync for a Google Smarthome application, you need to create a signed JWT using
 * a certificate created on your Google Cloud Console.  Details are availabe using the below URL's
 * https://developers.google.com/assistant/smarthome/develop/report-state
 * https://developers.google.com/assistant/smarthome/develop/request-sync
 * 
 * Download a .p12 to use for the certicate private key and that needs to be in the project root directory
 * The "iss" is the email address of the IAM account.  This is available in the GCC.
 * You will need these two items to run the program: The IAM email address and the .p12 file
 * 
 * Note: the Crypto API only works on .Netcore 3.X. The Microsoft.IdentityModel.Tokens is a GUGet package
 * 
 * Finally, a command application is not too practical for production.  This application creates a token that expires in 60 minutes.
 * If you port this to a website, IIS must have "Load Profiles" set to "true" or the Crypto API will fail.
*/
namespace MakeJWT
{
    class Program
    {
        public static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            string token = MakeJwt();
            Console.WriteLine();
            Console.WriteLine("The Final JWT: " + token );
            Console.WriteLine("Submiting JWT for access token");
            Task<string> accessToken = PostUrl("https://oauth2.googleapis.com/token", token);
            if(accessToken.Result != null)
             {
                Console.WriteLine(accessToken.Result);
            }
            else { Console.WriteLine("We had an error contacting the Google OAuth server"); }
        }

        public static string MakeJwt()
        {
            string jwt = string.Empty;
            string iss = "xxxxx.iam.gserviceaccount.com";  //IAM account email address
            string scope = "https://www.googleapis.com/auth/homegraph";
            string aud = "https://oauth2.googleapis.com/token";
            string exp = EpochExpiration();
            string iat = EpochCurrentTime();
            string jwtHeader = "{\"alg\":\"RS256\",\"typ\":\"JWT\"}";

            Console.WriteLine("iss: " + iss);
            Console.WriteLine("scope: " + scope);
            Console.WriteLine("aud: " + aud);
            Console.WriteLine("exp: " + exp);
            Console.WriteLine("iat: " + iat);
            Console.WriteLine("JWT Header: " + jwtHeader);
          

            string claim = "{\"iss\": \"" + iss + "\",\"scope\": \"" + scope + "\",\"aud\": \"" + aud + "\",\"exp\": " +
                exp + ",\"iat\": " + iat + "}";

            Console.WriteLine(claim);

            var encodedHeader = Base64UrlEncoder.Encode(jwtHeader);
            Console.WriteLine("Encoded JWT Header: " + encodedHeader);
            var encodedClaim = Base64UrlEncoder.Encode(claim);

            string claimSet = encodedHeader + "." + encodedClaim;
            string sig = Sign(claimSet);
            jwt = claimSet + '.' + sig;
            return jwt;
        }

        public static string EpochExpiration()
        {
            DateTime epoch = DateTime.UnixEpoch;
            DateTime now = DateTime.UtcNow;
            DateTime expiration = now.AddMinutes(60);
            TimeSpan secondsSinceEpoch = expiration.Subtract(epoch);
            return secondsSinceEpoch.TotalSeconds.ToString().Substring(0, 10);
        }

        public static string EpochCurrentTime()
        {
            DateTime epoch = DateTime.UnixEpoch;
            DateTime now = DateTime.UtcNow;          
            TimeSpan secondsSinceEpoch = now.Subtract(epoch);
            return secondsSinceEpoch.TotalSeconds.ToString().Substring(0, 10);
        }

        public static string Sign( string toSHA256)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(toSHA256);
               
                var certificate = new X509Certificate2(@"xxxx.p12", "notasecret", X509KeyStorageFlags.Exportable); //uses the .pk file

                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(certificate.PrivateKey.ToXmlString(true));

                //Sign the data
                byte[] sig = key.SignData(data, CryptoConfig.MapNameToOID("SHA256"));

                string asciiString = Encoding.ASCII.GetString(sig, 0, sig.Length);

                return Base64UrlEncoder.Encode(sig);

            }
            catch { Exception e;  return null; }
        }

        public static async Task<string> PostUrl(string url, string jwt)
        {
  //          string query = "?grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion=" + jwt;

            var query = new Dictionary<string, string>();
            query.Add("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer");
            query.Add("assertion", jwt);
            var content = new FormUrlEncodedContent(query);

            HttpResponseMessage response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return responseString;
            else return null;
        }

    }
}
