using System;
using System.Collections.Generic;
using System.Net;
using Flinks.CSharp.SDK.Model;
using Flinks.CSharp.SDK.Model.Authorization;
using Flinks.CSharp.SDK.Model.Constant;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;
using RestSharp;

namespace Flinks.CSharp.SDK
{
    public class FlinksClient
    {
        private string CustomerId { get; }
        private string Endpoint { get; }
        private string BaseUrl => GetBaseUrl();
        private AuthorizationRequestBody AuthorizationBody { get; set; }
        private RestClient RestClient { get; set; }

        private readonly JsonSerializerSettings _jsonSerializationSettings = new JsonSerializerSettings()
        {
            //Default Serializer used to put the JSON sent over the requests in the right format.
            NullValueHandling = NullValueHandling.Ignore
        };


        public FlinksClient(string customerId, string endpoint)
        {
            CustomerId = customerId;
            Endpoint = endpoint;

            RestClient = new RestClient(BaseUrl);
        }

        public AuthorizationResponse Authorize(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, string tag = null)
        {
            GenerateAuthorizeRequestBody(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, tag);

            var request = GetBaseRequest(EndpointConstant.Authorize, Method.POST);

            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(AuthorizationBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(response.Content);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NonAuthoritativeInformation:
                    {
                        apiResponse.AuthorizationStatus = AuthorizationStatus.PENDING_MFA_ANSWERS;

                        return apiResponse;
                    }
                default:
                    {
                        return null;
                    }
            }
        }



        public AuthorizationResponse AnswerMfaQuestionsAndAuthorize(string requestId, List<SecurityChallenge> securityChallenges)
        {
            var mfaAnswers = new Dictionary<string, List<string>>();

            foreach (var securityChallenge in securityChallenges)
            {
                mfaAnswers.Add(securityChallenge.Prompt, new List<string>()
                {
                    securityChallenge.Answer
                });
            }

            AuthorizationBody = new AuthorizationRequestBody()
            {
                RequestId = requestId,
                SecurityResponses = mfaAnswers
            };

            var request = GetBaseRequest(EndpointConstant.Authorize, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(AuthorizationBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(response.Content);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NonAuthoritativeInformation:
                    {
                        apiResponse.AuthorizationStatus = AuthorizationStatus.PENDING_MFA_ANSWERS;

                        return apiResponse;
                    }

                case HttpStatusCode.OK:
                    {
                        apiResponse.AuthorizationStatus = AuthorizationStatus.AUTHORIZED;

                        return apiResponse;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        #region Util
        private string GetBaseUrl()
        {
            var baseUrl = EndpointConstant.BaseUrl;

            baseUrl = baseUrl.Replace(FlinksSettingsConstant.Endpoint, Endpoint).Replace(FlinksSettingsConstant.CustomerId, CustomerId);

            return baseUrl;
        }

        private void GenerateAuthorizeRequestBody(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, string tag = null)
        {
            AuthorizationBody = new AuthorizationRequestBody()
            {
                UserName = userName,
                Password = password,
                Institution = institution,
                Save = save?.ToString().ToLower(),
                MostRecentCached = mostRecentCached?.ToString().ToLower(),
                WithMfaQuestions = withMfaQuestions?.ToString().ToLower(),
                Language = requestLanguage?.ToString().ToLower(),
                Tag = tag
            };
        }

        private static RestRequest GetBaseRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);

            request.Parameters.Clear();
            request.AddHeader(FlinksSettingsConstant.ContentType, FlinksSettingsConstant.ApplicationJsonUTF8);
            return request;
        }
        #endregion
    }
}
