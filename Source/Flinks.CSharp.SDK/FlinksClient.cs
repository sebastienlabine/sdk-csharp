﻿// - This Source Code Form is subject to the terms of the Mozilla Public
// - License, v. 2.0. If a copy of the MPL was not distributed with this
// - file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.Net;
using Flinks.CSharp.SDK.Model.Authorize;
using Flinks.CSharp.SDK.Model.Constant;
using Flinks.CSharp.SDK.Model.DeleteCard;
using Flinks.CSharp.SDK.Model.Enums;
using Flinks.CSharp.SDK.Model.GetAccountsDetail;
using Flinks.CSharp.SDK.Model.GetAccountsSummary;
using Flinks.CSharp.SDK.Model.GetStatement;
using Flinks.CSharp.SDK.Model.Score;
using Flinks.CSharp.SDK.Model.SetScheduleRefresh;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;
using RestSharp;
using static System.Net.HttpStatusCode;

namespace Flinks.CSharp.SDK
{
    public class FlinksClient
    {
        private string CustomerId { get; }
        private string Instance { get; }
        private string BaseUrl => GetBaseUrl();
        private AuthorizeRequestBody AuthorizeBody { get; set; }
        private RestClient RestClient { get; }
        public ClientStatus ClientStatus { get; set; }
        private string AuthToken { get; set; }

        private readonly JsonSerializerSettings _jsonSerializationSettings = new JsonSerializerSettings()
        {
            //Default Serializer used to put the JSON sent over the requests in the right format.
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Flinks client object
        /// </summary>
        /// <param name="customerId">Flinks customer ID.</param>
        /// <param name="instance">Flinks client instance URL.</param>
        public FlinksClient(string customerId, string instance)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(instance))
            {
                throw new NullReferenceException("The properties customerId and instance can't be null.");
            }

            CustomerId = customerId;
            Instance = instance;
            AuthToken = null;

            RestClient = new RestClient(BaseUrl);
        }

        /// <summary>
        /// Authorize method is used to call the Authorize endpoint at Flinks API. Must be used to start the Authorize process.
        /// </summary>
        /// <param name="institution">Institution (bank) where the you want to retrieve information from.</param>
        /// <param name="userName">The username used by your client in the banking account you want to retrieve information from.</param>
        /// <param name="password">The password used by your client int the banking account you want to retrieve information from.</param>
        /// <param name="save">Used if you want to save the request within the Flinks API.</param>
        /// <param name="mostRecentCached"></param>
        /// <param name="withMfaQuestions"></param>
        /// <param name="requestLanguage"></param>
        /// <param name="scheduleRefresh"></param>
        /// <param name="tag"></param>
        /// <returns>An AuthorizeResult object contains information regarding the status of the Authorize process.</returns>
        public AuthorizeResult Authorize(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            GenerateAuthorizeRequestBody(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            var request = GetBaseRequest(EndpointConstant.Authorize, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(AuthorizeBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            SetClientAuthorizeStatus(response.StatusCode);

            var apiResponse = JsonConvert.DeserializeObject<AuthorizeResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Authorize method is used to call the Authorize endpoint at Flinks API using the cached flow. Must be used to only when you already have authorized before and have an LoginId.
        /// </summary>
        /// <param name="loginId">LoginId generated by the previous response of the the Authorize method.</param>
        /// <returns>An AuthorizeResult object contained information regarding the status of the authorization process.</returns>
        public AuthorizeResult Authorize(Guid loginId)
        {
            AuthorizeBody = new AuthorizeRequestBody()
            {
                LoginId = loginId.ToString(),
                MostRecentCached = true.ToString()
            };

            var request = GetBaseRequest(EndpointConstant.Authorize, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(AuthorizeBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            SetClientAuthorizeStatus(response.StatusCode);

            var apiResponse = JsonConvert.DeserializeObject<AuthorizeResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Used to answer the MFA questions and proceed with the authorization process.
        /// </summary>
        /// <param name="requestId">The requestId generated by the original call to the Authorize method.</param>
        /// <param name="securityChallenges">A list with the SecurityChallanges provided by the original Authorize method filled with the answers for the MFA questions.</param>
        /// <returns>Returns an object containing information about the Authorize process.</returns>
        public AuthorizeResult AnswerMfaQuestionsAndAuthorize(Guid requestId, List<SecurityChallenge> securityChallenges)
        {
            if (!IsClientStatusWaitingForMfaQuestions()) throw new Exception($"The Authorization status has to be {ClientStatus.PENDING_MFA_ANSWERS} to call this method.");

            var mfaAnswers = new Dictionary<string, List<string>>();

            foreach (var securityChallenge in securityChallenges)
            {
                mfaAnswers.Add(securityChallenge.Prompt, new List<string>()
                {
                    securityChallenge.Answer
                });
            }

            AuthorizeBody = new AuthorizeRequestBody()
            {
                RequestId = requestId,
                SecurityResponses = mfaAnswers
            };

            var request = GetBaseRequest(EndpointConstant.Authorize, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(AuthorizeBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            SetClientAuthorizeStatus(response.StatusCode);

            var apiResponse = JsonConvert.DeserializeObject<AuthorizeResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Used to generate an authorize token in cases where you instance has the authorize token feature enabled. Must be called before the authorize.
        /// </summary>
        /// <param name="secretKey">Secret key provided to you by Flinks.</param>
        /// <returns>Token generated by Flinks. This token will be used to Authorize a request to the API.</returns>
        public AuthTokenResult GenerateAuthorizeToken(string secretKey)
        {
            var request = GetBaseRequest(EndpointConstant.GenerateAuthorizeToken, Method.POST);
            request.AddHeader(FlinksHeaderConstant.FlinksAuthKey, secretKey);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<AuthTokenResult>(response.Content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                AuthToken = apiResponse.Token;
            }

            return apiResponse;
        }

        /// <summary>
        /// Used to get summary infos from Flinks API. The client must be Authorized before calling this method.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>Returns an object containg information about the card summary.</returns>
        public AccountsSummaryResult GetAccountSummary(Guid requestId)
        {
            if (!IsClientStatusAuthorized()) throw new Exception($"You can't call GetAccountsSummary when the ClientStatus is not Authorized, you current status is: {ClientStatus}.");

            var request = GetBaseRequest(EndpointConstant.GetAccountsSummary, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(new AuthorizeRequestBody()
            {
                RequestId = requestId
            }, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            SetClientAuthorizeStatus(response.StatusCode);

            var apiResponse = JsonConvert.DeserializeObject<AccountsSummaryResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Retrieve complete details about a user. Calling this endpoint will give you the same information as GetAccountsSummary for each account linked to the session, plus the following:
        /// Most recent transactions (90, or 365 days depending on your call);
        /// * Name of the account holder;
        /// * Full institution, transit, and account number;
        /// * KYC(Know Your Customer) [Name, Civic Address, City, Province, Postal Code, PO Box, E-mail, Telephone Number]
        /// * Telephone Number.
        /// </summary>
        /// <param name="requestId">The requestId generated by the Authorize endpoint request done previously.</param>
        /// <param name="withAccountIdentity">Adds the information to the response about the account such as : Transit, Institution, and Account Number (set to false by default).</param>
        /// <param name="withKyc">Display or hide the Holder name along with the holder information (set to true by default).</param>
        /// <param name="withTransactions">Display or hide the account transactions (set to true by default).</param>
        /// <param name="withBalance">Displays all balances (from each transaction and account) as well as overdraft protection information (set to true by default).</param>
        /// <param name="daysOfTransaction">Set the number of days of transaction you want to display (set to Days90 by default).</param>
        /// <param name="accountsFilter">Filters the accounts returned in the response by adding the unique identifier we provided in the response of the GetAccountsSummary or previously called GetAccountDetails.</param>
        /// <returns>Returns an object containing all the accounts details.</returns>
        public AccountsDetailResult GetAccountDetails(Guid requestId, bool? withAccountIdentity, bool? withKyc, bool? withTransactions, bool? withBalance, DaysOfTransaction? daysOfTransaction, List<Guid> accountsFilter = null)
        {
            if (!IsClientStatusAuthorized()) throw new Exception($"You can't call GetAccountsSummary when the ClientStatus is not Authorized, you current status is: {ClientStatus}.");

            var request = GetBaseRequest(EndpointConstant.GetAccountsDetail, Method.POST);

            var getAccountDetailsRequestBody = GenerateGetAccountsDetailRequestBody(requestId, withAccountIdentity, withKyc, withTransactions, withBalance, daysOfTransaction, accountsFilter);

            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(getAccountDetailsRequestBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<AccountsDetailResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Retrieve complete details about a user. Calling this endpoint will trigger the async flow. 
        /// </summary>
        /// <param name="requestId">The requestId generated by the previous Authorize call.</param>
        /// <returns>Returns an object containing all the accounts details.</returns>
        public AccountsDetailResult GetAccountDetailsAsync(Guid requestId)
        {
            if (!IsClientStatusAuthorized()) throw new Exception($"You can't call GetAccountsSummary when the ClientStatus is not Authorized, you current status is: {ClientStatus}.");

            var request = GetBaseRequest($"{EndpointConstant.GetAccountsDetailAsync}/{requestId}", Method.GET);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<AccountsDetailResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Retrieve the Official PDF Bank Statement of an account.
        /// </summary>
        /// <param name="requestId">The requestId generated by the Authorize endpoint request done previously.</param>
        /// <param name="numberOfStatements">Defines the number of statements per account retrieved.</param>
        /// <param name="accountsFilter">If provided, it will restrict the statements returned for the given account(s).</param>
        /// <returns>Returns an object containing the base64 bytes of the bank official statements file.</returns>
        public StatementResult GetStatements(Guid requestId, NumberOfStatements? numberOfStatements, List<Guid> accountsFilter)
        {
            IsGetStatementValid(numberOfStatements, accountsFilter);

            var request = GetBaseRequest(EndpointConstant.GetStatements, Method.POST);

            var getStatementsRequestBody = GenerateGetStatementsRequestBody(requestId, numberOfStatements, accountsFilter);

            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(getStatementsRequestBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<StatementResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Method used to retrieve statements when the process is sent to a long poll job. Use it when receiving OPERATION_PENDING as result from the GetStatements method.
        /// </summary>
        /// <param name="requestId">The requestId generated by the Authorize endpoint request done previously.</param>
        /// <returns>Returns an object containing the base64 bytes of the bank official statements file.</returns>
        public StatementResult GetStatementsAsync(Guid requestId)
        {
            var request = GetBaseRequest($"{EndpointConstant.GetStatementsAsync}/{requestId}", Method.GET);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<StatementResult>(response.Content);

            apiResponse.ClientStatus = ClientStatus;

            return apiResponse;
        }

        /// <summary>
        /// Used to deactivate or activate the automatic refresh for a given loginId.
        /// </summary>
        /// <param name="loginId">The LoginId retrieved from a previous operation (this endpoint doesn't require an Authorize prior to use).</param>
        /// <param name="isActivated">Enables or disables the automatic nightly refresh of a given LoginId.</param>
        public string SetScheduledRefresh(Guid loginId, bool isActivated)
        {
            var request = GetBaseRequest(EndpointConstant.SetScheduledRefresh, Method.PATCH);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(new ScheduleRefreshRequestBody()
            {
                LoginId = loginId.ToString(),
                IsActivated = isActivated.ToString().ToLower()
            }, _jsonSerializationSettings), ParameterType.RequestBody);


            var response = RestClient.Execute(request);

            return response.Content;
        }

        /// <summary>
        /// Used to delete all traces of information about a card in Flinks database.
        /// </summary>
        /// <param name="loginId">The LoginId retrieved from a previous operation (this endpoint doesn't require an Authorize prior to use).</param>
        public DeleteCardResult DeleteCard(Guid loginId)
        {
            var request = GetBaseRequest($"{EndpointConstant.DeleteCard}/{loginId}", Method.DELETE);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<DeleteCardResult>(response.Content);

            return apiResponse;
        }

        /// <summary>
        /// Used to retrieve a score based on the client information analyzed by Flinks.
        /// </summary>
        /// <param name="loginId">The LoginId of the previous authentication.</param>
        /// <param name="requestId">The RequestId of the previous Authorize call.</param>
        /// <param name="scoreRequestBody">The score information needed to retrieve a score from Flinks</param>
        /// <returns>An score object containing the processed score based on user's input.</returns>
        public ScoreResult GetScore(Guid loginId, Guid requestId, ScoreRequestBody scoreRequestBody)
        {
            var scoreUrl = EndpointConstant.GetScore;
            scoreUrl = scoreUrl.Replace(FlinksSettingsConstant.LoginId, loginId.ToString()).Replace(FlinksSettingsConstant.RequestId, requestId.ToString());

            var request = GetBaseRequest(scoreUrl, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(scoreRequestBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<ScoreResult>(response.Content);

            return apiResponse;
        }

        #region Util
        private string GetBaseUrl()
        {
            var baseUrl = EndpointConstant.BaseUrl;

            baseUrl = baseUrl.Replace(FlinksSettingsConstant.Instance, Instance).Replace(FlinksSettingsConstant.CustomerId, CustomerId);

            return baseUrl;
        }

        private void GenerateAuthorizeRequestBody(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            AuthorizeBody = new AuthorizeRequestBody()
            {
                UserName = userName,
                Password = password,
                Institution = institution,
                Save = save?.ToString().ToLower(),
                MostRecentCached = mostRecentCached?.ToString().ToLower(),
                WithMfaQuestions = withMfaQuestions?.ToString().ToLower(),
                Language = requestLanguage?.ToString().ToLower(),
                ScheduleRefresh = scheduleRefresh.ToString().ToLower(),
                Tag = tag
            };
        }

        private AccountsDetailRequestBody GenerateGetAccountsDetailRequestBody(Guid requestId, bool? withAccountIdentity, bool? withKyc, bool? withTransactions, bool? withBalance, DaysOfTransaction? daysOfTransaction, List<Guid> accountsFilter = null)
        {
            return new AccountsDetailRequestBody()
            {
                RequestId = requestId.ToString(),
                WithAccountIdentity = withAccountIdentity,
                WithKyc = withKyc,
                DaysOfTransaction = daysOfTransaction != null ? daysOfTransaction.ToString() : null,
                WithTransactions = withTransactions,
                WithBalance = withBalance,
                AccountsFilter = accountsFilter
            };
        }

        private StatementsRequestBody GenerateGetStatementsRequestBody(Guid requestId, NumberOfStatements? numberOfStatements, List<Guid> accountsFilter = null)
        {
            return new StatementsRequestBody()
            {
                RequestId = requestId.ToString(),
                NumberOfStatements = numberOfStatements != null ? numberOfStatements.ToString() : null,
                AccountsFilter = accountsFilter
            };
        }

        private RestRequest GetBaseRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);

            request.Parameters.Clear();
            request.AddHeader(FlinksSettingsConstant.ContentType, FlinksSettingsConstant.ApplicationJsonUTF8);

            if (HasAuthToken())
            {
                request.AddHeader(FlinksHeaderConstant.FlinksAuthKey, AuthToken);
            }

            return request;
        }

        private void SetClientAuthorizeStatus(HttpStatusCode httpStatusCode)
        {
            ClientStatus clientStatus;

            switch (httpStatusCode)
            {
                case NonAuthoritativeInformation:
                    {
                        clientStatus = ClientStatus.PENDING_MFA_ANSWERS;

                        break;
                    }
                case OK:
                    {
                        clientStatus = ClientStatus.AUTHORIZED;

                        break;
                    }
                case Unauthorized:
                    {
                        clientStatus = ClientStatus.UNAUTHORIZED;

                        break;
                    }

                default:
                    {
                        clientStatus = ClientStatus.UNKNOWN;

                        break;
                    }
            }

            ClientStatus = clientStatus;
        }

        private bool IsClientStatusWaitingForMfaQuestions()
        {
            return ClientStatus == ClientStatus.PENDING_MFA_ANSWERS;
        }

        private bool IsClientStatusAuthorized()
        {
            return ClientStatus == ClientStatus.AUTHORIZED;
        }

        private static void IsGetStatementValid(NumberOfStatements? numberOfStatements, List<Guid> accountsFilter)
        {
            if (numberOfStatements != null && numberOfStatements == NumberOfStatements.Months12)
            {
                if (accountsFilter == null)
                {
                    throw new Exception("When using NumberOfStatements as Months12, the accounts filter can't be null.");
                }

                if (accountsFilter.Count == 0 || accountsFilter.Count >= 2)
                {
                    throw new Exception("When using NumberOfStatements as Months12, you have to provide a single accountId on accountsFilter.");
                }
            }
        }

        private bool HasAuthToken()
        {
            return !string.IsNullOrEmpty(AuthToken);
        }
        #endregion
    }
}