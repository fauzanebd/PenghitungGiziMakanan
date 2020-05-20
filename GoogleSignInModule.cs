using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PenghitungGiziMakanan
{
    public class GoogleSignInModule
    {
        public static Dictionary<string, string> Hasil = new Dictionary<string, string>();

        private const string clientID = "397787604976-k43c1ib10hcsimt6pefp5dcsk82evl22.apps.googleusercontent.com";
        private const string clientSecret = "TP-QUAco2n1G-HLH9tSNYG_1";
        private const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";


        //Buat bikin random port lokal
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public static async Task executeLoginModule()
        {
            // Generates state and PKCE values.
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format
                ("{0}?response_type=code&" +
                "scope=openid%20profile%20email%20https://www.googleapis.com/auth/user.gender.read https://www.googleapis.com/auth/user.birthday.read&" +
                "redirect_uri={1}&" +
                "client_id={2}&" +
                "state={3}&" +
                "code_challenge={4}&" +
                "code_challenge_method={5}",
                authorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                state,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                return;
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                return;
            }

            // Starts the code exchange at the Token Endpoint.
            performCodeExchange(code, code_verifier, redirectURI);
            await Task.Delay(4000);
        }

        public static async void performCodeExchange(string code, string code_verifier, string redirectURI)
        {

            // builds the  request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody = string.Format
                ("code={0}&" +
                "redirect_uri={1}&" +
                "client_id={2}&" +
                "code_verifier={3}&" +
                "client_secret={4}&" +
                "scope=https://www.googleapis.com/auth/user.birthday.read https://www.googleapis.com/auth/user.gender.read&" +
                "grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                code_verifier,
                clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    userinfoCall(access_token);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        MessageBox.Show("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            MessageBox.Show(responseText);
                        }
                    }

                }
            }
        }

        public static async void userinfoCall(string access_token)
        {
            // builds the  request
            string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";
            string userID;
            string userName;
            string userPicture;
            string userNickName;
            string userBirthDay;
            string userGender = "";

            // sends the request
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // gets the response
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                // reads response body
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();

                // converts to dictionary
                Dictionary<string, string> userInfoDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(userinfoResponseText);
                userName = userInfoDecoded["name"];
                userPicture = userInfoDecoded["picture"];
                userNickName = userInfoDecoded["given_name"];
                userID = userInfoDecoded["sub"];
            }

            string userBGReqURI =
                "https://people.googleapis.com/v1/people/me?personFields=genders%2Cbirthdays&key=AIzaSyDTxrqbkUXfKHVU99uAq5kblGBoZNjDBc0";
            HttpWebRequest userBGReq = (HttpWebRequest)WebRequest.Create(userBGReqURI);
            userBGReq.Method = "GET";
            userBGReq.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userBGReq.ContentType = "application/x-www-form-urlencoded";
            userBGReq.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            WebResponse userBGResp = await userBGReq.GetResponseAsync();
            using (StreamReader userBGRespReader = new StreamReader(userBGResp.GetResponseStream()))
            {
                string userBGRespText = await userBGRespReader.ReadToEndAsync();
                JsonObject obj = (JsonObject)SimpleJson.DeserializeObject(userBGRespText);
                JsonArray genderObj = (JsonArray)obj["genders"];
                JsonArray birthdayObj = (JsonArray)obj["birthdays"];
                JsonObject genderInfo = (JsonObject)genderObj[0];
                JsonObject birthdayInfo = (JsonObject)birthdayObj[birthdayObj.Count - 1];
                JsonObject birthdayInfoChild = (JsonObject)birthdayInfo["date"];
                string year = birthdayInfoChild["year"].ToString();
                string month = birthdayInfoChild["month"].ToString();
                string day = birthdayInfoChild["day"].ToString();
                userBirthDay = string.Format("{0}/{1}/{2}", day, month, year);
                string genderEN = (string)genderInfo["value"];
                if (genderEN == "male")
                    userGender = "Pria";
                else if (genderEN == "female")
                    userGender = "Wanita";
            }
            Dictionary<string, string> Result = new Dictionary<string, string>();
            Result.Add("Name", userName);
            Result.Add("NickName", userNickName);
            Result.Add("JenisKelamin", userGender);
            Result.Add("BirthDate", userBirthDay);
            Result.Add("Picture", userPicture);
            Result.Add("ID", userID);

            Hasil = Result;
            
        }
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}

