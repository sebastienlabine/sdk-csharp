﻿using System;
using System.Collections.Generic;
using System.Net;
using Flinks.CSharp.SDK.Model;
using Flinks.CSharp.SDK.Model.AccountDetail;
using Flinks.CSharp.SDK.Model.AccountSummary;
using Flinks.CSharp.SDK.Model.Authorization;
using Flinks.CSharp.SDK.Model.Constant;
using Flinks.CSharp.SDK.Model.DeleteCard;
using Flinks.CSharp.SDK.Model.ScheduleRefresh;
using Flinks.CSharp.SDK.Model.Shared;
using Flinks.CSharp.SDK.Model.Statement;
using Newtonsoft.Json;
using RestSharp;
using static System.Net.HttpStatusCode;

namespace Flinks.CSharp.SDK
{
    public class FlinksClient
    {
        private string CustomerId { get; }
        private string Endpoint { get; }
        private string BaseUrl => GetBaseUrl();
        private AuthorizationRequestBody AuthorizationBody { get; set; }
        private RestClient RestClient { get; }
        public AuthorizationStatus AuthorizationStatus { get; set; }

        private readonly JsonSerializerSettings _jsonSerializationSettings = new JsonSerializerSettings()
        {
            //Default Serializer used to put the JSON sent over the requests in the right format.
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Flinks client object
        /// </summary>
        /// <param name="customerId">Flinks customer ID.</param>
        /// <param name="endpoint">Flinks client endpoint URL.</param>
        public FlinksClient(string customerId, string endpoint)
        {
            CustomerId = customerId;
            Endpoint = endpoint;

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
        /// <param name="tag"></param>
        /// <returns>An AuthorizationResponse object contained information regarding the status of the authorization process.</returns>
        public AuthorizationResponse Authorize(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            GenerateAuthorizeRequestBody(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            var request = GetBaseRequest(EndpointConstant.Authorize, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(AuthorizationBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            SetClientAuthorizationStatus(response.StatusCode);

            var apiResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(response.Content);

            apiResponse.AuthorizationStatus = AuthorizationStatus;

            return apiResponse;
        }

        /// <summary>
        /// Used to answer the MFA questions and proceed with the authorization process.
        /// </summary>
        /// <param name="requestId">The requestId generated by the original call to the Authorize method.</param>
        /// <param name="securityChallenges">A list with the SecurityChallanges provided by the original Authorize method filled with the answers for the MFA questions.</param>
        /// <returns></returns>
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

            SetClientAuthorizationStatus(response.StatusCode);

            var apiResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(response.Content);

            apiResponse.AuthorizationStatus = AuthorizationStatus;

            return apiResponse;
        }

        /// <summary>
        /// Used to get summary infos from Flinks API. The client must be Authorized before calling this method.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public AccountSummary GetAccountSummary(string requestId)
        {
            var request = GetBaseRequest(EndpointConstant.GetAccountsSummary, Method.POST);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(new AuthorizationRequestBody()
            {
                RequestId = requestId
            }, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            SetClientAuthorizationStatus(response.StatusCode);

            var apiResponse = JsonConvert.DeserializeObject<AccountSummary>(response.Content);

            apiResponse.AuthorizationStatus = AuthorizationStatus;

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
        /// <returns></returns>
        public AccountDetail GetAccountDetails(string requestId, bool? withAccountIdentity, bool? withKyc, bool? withTransactions, bool? withBalance, DaysOfTransaction? daysOfTransaction, List<string> accountsFilter = null)
        {
            var request = GetBaseRequest(EndpointConstant.GetAccountsDetail, Method.POST);

            var getAccountDetailsRequestBody = GenerateGetAccountsDetailRequestBody(requestId, withAccountIdentity, withKyc, withTransactions, withBalance, daysOfTransaction, accountsFilter);

            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(getAccountDetailsRequestBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<AccountDetail>(response.Content);

            apiResponse.AuthorizationStatus = AuthorizationStatus;

            return apiResponse;
        }

        /// <summary>
        /// Retrieve the Official PDF Bank Statement of an account.
        /// </summary>
        /// <param name="requestId">The requestId generated by the Authorize endpoint request done previously.</param>
        /// <param name="numberOfStatements">Defines the number of statements per account retrieved.</param>
        /// <param name="accountsFilter">If provided, it will restrict the statements returned for the given account(s).</param>
        /// <returns></returns>
        public Statements GetStatements(string requestId, NumberOfStatements? numberOfStatements, List<Guid> accountsFilter)
        {
            var request = GetBaseRequest(EndpointConstant.GetStatements, Method.POST);

            var getStatementsRequestBody = GenerateGetStatementsRequestBody(requestId, numberOfStatements, accountsFilter);

            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(getStatementsRequestBody, _jsonSerializationSettings), ParameterType.RequestBody);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<Statements>(response.Content);

            apiResponse.AuthorizationStatus = AuthorizationStatus;

            return apiResponse;
        }

        /// <summary>
        /// Method used to retrieve statements when the process is sent to a long poll job. Use it when receiving OPERATION_PENDING as result from the GetStatements method.
        /// </summary>
        /// <param name="requestId">The requestId generated by the Authorize endpoint request done previously.</param>
        /// <returns></returns>
        public Statements GetStatementsAsync(string requestId)
        {
            var request = GetBaseRequest($"{EndpointConstant.GetStatementsAsync}/{requestId}", Method.GET);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<Statements>(response.Content);

            apiResponse.AuthorizationStatus = AuthorizationStatus;

            return apiResponse;
        }

        /// <summary>
        /// Used to deactivate or activate the automatic refresh for a given loginId.
        /// </summary>
        /// <param name="loginId">The LoginId retrieved from a previous operation (this endpoint doesn't require an Authorize prior to use).</param>
        /// <param name="isActivated">Enables or disables the automatic nightly refresh of a given LoginId.</param>
        public string SetScheduledRefresh(string loginId, bool isActivated)
        {
            var request = GetBaseRequest(EndpointConstant.SetScheduledRefresh, Method.PATCH);
            request.AddParameter(FlinksSettingsConstant.ApplicationJsonUTF8, JsonConvert.SerializeObject(new SetScheduleRefreshRequestBody()
            {
                LoginId = loginId,
                IsActivated = isActivated.ToString().ToLower()
            }, _jsonSerializationSettings), ParameterType.RequestBody);


            var response = RestClient.Execute(request);

            return response.Content;
        }

        /// <summary>
        /// Used to delete all traces of information about a card in Flinks database.
        /// </summary>
        /// <param name="loginId">The LoginId retrieved from a previous operation (this endpoint doesn't require an Authorize prior to use).</param>
        public DeleteCard DeleteCard(string loginId)
        {
            var request = GetBaseRequest($"{EndpointConstant.DeleteCard}/{loginId}", Method.DELETE);

            var response = RestClient.Execute(request);

            var apiResponse = JsonConvert.DeserializeObject<DeleteCard>(response.Content);

            return apiResponse;
        }

        #region Util
        private string GetBaseUrl()
        {
            var baseUrl = EndpointConstant.BaseUrl;

            baseUrl = baseUrl.Replace(FlinksSettingsConstant.Endpoint, Endpoint).Replace(FlinksSettingsConstant.CustomerId, CustomerId);

            return baseUrl;
        }

        private void GenerateAuthorizeRequestBody(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
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
                ScheduleRefresh = scheduleRefresh.ToString().ToLower(),
                Tag = tag
            };
        }

        private GetAccountDetailRequestBody GenerateGetAccountsDetailRequestBody(string requestId, bool? withAccountIdentity, bool? withKyc, bool? withTransactions, bool? withBalance, DaysOfTransaction? daysOfTransaction, List<string> accountsFilter = null)
        {
            return new GetAccountDetailRequestBody()
            {
                RequestId = requestId,
                WithAccountIdentity = withAccountIdentity,
                WithKyc = withKyc,
                DaysOfTransaction = daysOfTransaction != null ? daysOfTransaction.ToString() : null,
                WithTransactions = withTransactions,
                WithBalance = withBalance,
                AccountsFilter = accountsFilter
            };
        }

        private GetStatementsRequestBody GenerateGetStatementsRequestBody(string requestId, NumberOfStatements? numberOfStatements, List<Guid> accountsFilter = null)
        {
            return new GetStatementsRequestBody()
            {
                RequestId = requestId,
                NumberOfStatements = numberOfStatements != null ? numberOfStatements.ToString() : null,
                AccountsFilter = accountsFilter
            };
        }

        private static RestRequest GetBaseRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);

            request.Parameters.Clear();
            request.AddHeader(FlinksSettingsConstant.ContentType, FlinksSettingsConstant.ApplicationJsonUTF8);
            return request;
        }

        private void SetClientAuthorizationStatus(HttpStatusCode httpStatusCode)
        {
            var authorizationStatus = AuthorizationStatus.UNKNOWN;

            switch (httpStatusCode)
            {
                case NonAuthoritativeInformation:
                    {
                        authorizationStatus = AuthorizationStatus.PENDING_MFA_ANSWERS;

                        break;
                    }
                case OK:
                    {
                        authorizationStatus = AuthorizationStatus.AUTHORIZED;

                        break;
                    }

                default:
                    {
                        authorizationStatus = AuthorizationStatus.UNKNOWN;

                        break;
                    }
            }

            AuthorizationStatus = authorizationStatus;
        }
        #endregion
    }
}