﻿using Newtonsoft.Json;
using PhotoMap.Models.Entities;
using PhotoMap.Models.Responses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace PhotoMap.Models.Abstract
{
    public abstract class InstagramWebServiceBase : IInstagramWebService
    {
        // With this string starts every URL except Authentication URL
        internal static readonly string BaseUrl = "https://api.instagram.com/v1/";
        private string accessToken = string.Empty;
        // This is URL for authentication
        //internal static readonly string oAuthUrl= "https://api.instagram.com/oauth/authorize/?client_id=CLIENT-ID&redirect_uri=REDIRECT-URI&response_type=code

        public abstract List<User> GetUserImages(string code);
        public abstract UserInfo GetUser(string code);
        public abstract Tags GetTags(string code, string tag);
        public abstract List<InstagramPost> GetRecentImagesByTag(string code, string tag);
        public abstract List<InstagramPost> GetLocations(string code, string lat, string ltd);
        public abstract List<InstagramPost> GetImagesByLocationID(string code, string locationId);

        public string RawJson(string apiRequest, string code)
        {
            var rawJson = string.Empty;
            OAuthInstagramAccessToken accessTokenObj = new OAuthInstagramAccessToken();
            UserToken ut = GetUserTokens();

            if (ut != null)
            {
                accessToken = ut.AccessToken;
            }
            else
            {
                accessTokenObj = GetAccessToken(code);
                accessToken = accessTokenObj.AccessToken;

                //writes data in json file on server
                dynamic parsedJson = JsonConvert.DeserializeObject(rawJson);
                using (var writer = new StreamWriter(HttpContext.Current.Server.MapPath("~/App_Data/token.json")))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(parsedJson, Formatting.Indented));
                }
            }

            var url = BaseUrl + apiRequest + "?access_token=" + accessToken;
            var request = (HttpWebRequest)WebRequest.Create(url);

            using (var response = request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawJson = reader.ReadToEnd();
            }

            return rawJson;
        }
        public string RawJsonQuery(string apiRequest, string code)
        {
            var rawJson = string.Empty;
            OAuthInstagramAccessToken accessTokenObj = new OAuthInstagramAccessToken();
            //checks if access token expired or is not used

            UserToken ut = GetUserTokens();

            if (ut != null)
            {
                accessToken = ut.AccessToken;
            }
            else
            {
                accessTokenObj = GetAccessToken(code);
                accessToken = accessTokenObj.AccessToken;

                //writes data in json file on server
                dynamic parsedJson = JsonConvert.DeserializeObject(rawJson);
                using (var writer = new StreamWriter(HttpContext.Current.Server.MapPath("~/App_Data/token.json")))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(parsedJson, Formatting.Indented));
                }
            }
           
            var url = BaseUrl + apiRequest + "&access_token=" + accessToken;
            var request = (HttpWebRequest)WebRequest.Create(url);

            using (var response = request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawJson = reader.ReadToEnd();
            }

            return rawJson;
        }
        //Process all requests and return a response
        private WebResponse processWebRequest(string url)
        {

            WebRequest request;
            WebResponse response;

            request = WebRequest.Create(url);
            response = request.GetResponse();

            return response;

        }
        private string GetAuthCode()
        {
                var client_id = ConfigurationManager.AppSettings["instagram.clientid"];
                var redirect_uri = ConfigurationManager.AppSettings["instagram.redirecturi"];
                var response_uri = string.Format("https://api.instagram.com/oauth/authorize/?client_id={0}&redirect_uri={1}&response_type=code",
                         client_id,
                         redirect_uri);
                //Response.Redirect(response_uri);
            

            var request = (HttpWebRequest)WebRequest.Create(response_uri);
            
            WebResponse authResponse = request.GetResponse();
            var uri = authResponse.ResponseUri;
            

            // string uricode = uri["code"];
            //WebRequest wrGETURL;
            //wrGETURL = WebRequest.Create(sUrl);
            StreamReader reader = new StreamReader(authResponse.GetResponseStream());
            string responseText = reader.ReadToEnd();
            Stream dataStream = authResponse.GetResponseStream();
            //Stream objStream;
            //objStream = request.GetResponse().GetResponseStream();
            //StreamReader objReader = new StreamReader(objStream, Encoding.UTF8);
            //WebResponse wr = wrGETURL.GetResponse();
            string code = "kkk";

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("client_id", ConfigurationManager.AppSettings["instagram.clientid"].ToString());
            parameters.Add("client_secret", ConfigurationManager.AppSettings["instagram.clientsecret"].ToString());
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("redirect_uri", ConfigurationManager.AppSettings["instagram.redirecturi"].ToString());
            parameters.Add("code", code);

            WebClient client = new WebClient();
            var result = client.UploadValues("https://api.instagram.com/oauth/access_token", "POST", parameters);

            var response = System.Text.Encoding.Default.GetString(result);

            var jsResult = JsonConvert.DeserializeObject(response);
            
            return code;
        }
        
        private UserToken GetUserTokens()
        {
            var rawJson = string.Empty;

            //Reads data from messages.json file
            using (var readerCashe = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Data/token.json")))
            {
                rawJson = readerCashe.ReadToEnd();
            }

            UserToken userToken = new UserToken();
            userToken = JsonConvert.DeserializeObject<UserToken>(rawJson);
            return userToken;
        }
       

        private OAuthInstagramAccessToken GetAccessToken(string code)
        {
            var client = new HttpClient();

            var oAuthUrl = ConfigurationManager.AppSettings["OAuthUrl"];

            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("client_id", ConfigurationManager.AppSettings["instagram.clientid"].ToString()));
            postData.Add(new KeyValuePair<string, string>("client_secret", ConfigurationManager.AppSettings["instagram.clientsecret"].ToString()));
            postData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            postData.Add(new KeyValuePair<string, string>("scope", "public_content"));
            postData.Add(new KeyValuePair<string, string>("redirect_uri", ConfigurationManager.AppSettings["instagram.redirecturi"].ToString()));
            postData.Add(new KeyValuePair<string, string>("code", code));

            HttpContent content = new FormUrlEncodedContent(postData);


            var rslt = client.PostAsync("https://api.instagram.com/oauth/access_token", content).Result;
            var result = rslt.Content.ReadAsStringAsync().Result;
            OAuthInstagramAccessToken access_token = JsonConvert.DeserializeObject<OAuthInstagramAccessToken>(result);
            return access_token;

        }
       
    }
}