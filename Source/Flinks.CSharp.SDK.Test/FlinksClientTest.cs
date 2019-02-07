using Xunit;
using System;
using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Authorize;
using Flinks.CSharp.SDK.Model.Enums;
using Flinks.CSharp.SDK.Model.Shared;

namespace Flinks.CSharp.SDK.Test
{
    public class FlinksClientTest : FlinksTestBase
    {
        [Theory]
        [MemberData(nameof(FlinksClientInstantiationValueTest.TestData), MemberType = typeof(FlinksClientInstantiationValueTest))]
        public void Should_throw_an_exception_if_customerId_or_endpoint_is_null_or_empty(string customerId, string endpoint)
        {
            Assert.Throws<NullReferenceException>(() => new FlinksClient(customerId, endpoint));
        }

        [Theory]
        [MemberData(nameof(AuthorizeTestWrongPassword.TestData), MemberType = typeof(AuthorizeTestWrongPassword))]
        public void Should_Authorize_and_receive_an_not_authorized_response_from_the_api(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var response = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            Assert.Equal(401, response.HttpStatusCode);
            Assert.Equal(ClientStatus.UNAUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_Authorize_and_retrieve_mfa_questions_from_api(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var response = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            Assert.Equal(203, response.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_Authorize_and_not_retrieve_mfa_questions_from_api_with_when_using_MostRecentCached(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var response = apiClient.Authorize(institution, userName, password, false, true, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            Assert.Equal(200, response.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_Authorize_and_not_retrieve_mfa_questions_from_api_with_WithMfaQuestions(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResult = apiClient.Authorize(institution, userName, password, save, mostRecentCached, true, requestLanguage, scheduleRefresh, tag);

            Assert.Equal(203, authorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
            Assert.NotEmpty(authorizeResult.SecurityChallenges);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_Authorize_and_not_retrieve_mfa_questions_from_api_in_french(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResult = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, RequestLanguage.fr, scheduleRefresh, tag);

            Assert.True(IsSecurityChallengeInFrench(authorizeResult.SecurityChallenges));
            Assert.Equal(203, authorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
            Assert.NotEmpty(authorizeResult.SecurityChallenges);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_Authorize_and_set_nigthly_refresh_for_card(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResult = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, null, true, tag);

            Assert.Equal(203, authorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
            Assert.NotEmpty(authorizeResult.SecurityChallenges);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_Authorize_and_add_a_tag(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var tagValue = "I am a tag";

            var authorizeResult = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, null, true, tagValue);

            Assert.Equal(tagValue, authorizeResult.Tag);
            Assert.Equal(203, authorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
            Assert.NotEmpty(authorizeResult.SecurityChallenges);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_AnswerMfaQuestionsAndAuthorize_and_receive_authorized_message_from_the_api(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            Assert.Equal(requestId.ToString(), answerMfaQuestionsAndAuthorizeResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_AnswerMfaQuestionsAndAuthorize_and_receive_non_authoritative_from_api(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, new List<SecurityChallenge>());

            Assert.Equal(203, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsSummaryResult(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var getSummaryResult = apiClient.GetAccountSummary(requestId);

            Assert.NotNull(getSummaryResult.Accounts);
            Assert.NotNull(getSummaryResult.Login);
            Assert.Equal(requestId.ToString(), getSummaryResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_throw_an_exception_if_calling_GetAccountsSummary_if_ClientStatus_is_not_authorized(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            var requestId = new Guid(authorizeResponse.RequestId);

            Assert.Throws<Exception>(() => apiClient.GetAccountSummary(requestId));
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_throw_an_exception_if_calling_GetAccountsDetail_if_ClientStatus_is_not_authorized(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            var requestId = new Guid(authorizeResponse.RequestId);

            Assert.Throws<Exception>(() => apiClient.GetAccountDetails(requestId, null, null, null, null, null, null));
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsDetailResult(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var getAccountsDetailResult = apiClient.GetAccountDetails(requestId, null, null, null, null, null, null);

            Assert.NotNull(getAccountsDetailResult.Accounts);
            Assert.NotNull(getAccountsDetailResult.Login);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsDetailResult_WithNoAccountIdentity(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var getAccountsDetailResult = apiClient.GetAccountDetails(requestId, false, null, null, null, null, null);

            Assert.Null(getAccountsDetailResult.Accounts[0].AccountNumber);
            Assert.Null(getAccountsDetailResult.Accounts[0].InstitutionNumber);
            Assert.Null(getAccountsDetailResult.Accounts[0].TransitNumber);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsDetailResult_with_no_KYC(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var getAccountsDetailResult = apiClient.GetAccountDetails(requestId, null, false, null, null, null, null);

            Assert.Null(getAccountsDetailResult.Accounts[0].Holder);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsDetailResult_with_no_transactions(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var getAccountsDetailResult = apiClient.GetAccountDetails(requestId, null, null, false, null, null, null);

            Assert.Empty(getAccountsDetailResult.Accounts[0].Transactions);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsDetailResult_with_no_balance(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var getAccountsDetailResult = apiClient.GetAccountDetails(requestId, null, null, null, false, null, null);

            Assert.Null(getAccountsDetailResult.Accounts[0].Balance.Available);
            Assert.Null(getAccountsDetailResult.Accounts[0].Balance.Current);
            Assert.Null(getAccountsDetailResult.Accounts[0].Balance.Limit);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsDetailResult_with_DaysOfTransaction_Days90_and_Days365(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var getAccountsDetailResult = apiClient.GetAccountDetails(requestId, null, null, null, null, DaysOfTransaction.Days90, null);


            var apiClient2 = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse2 = apiClient2.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse2.SecurityChallenges);
            }

            var requestId2 = new Guid(authorizeResponse2.RequestId);

            var answerMfaQuestionsAndAuthorizeResult2 = apiClient2.AnswerMfaQuestionsAndAuthorize(requestId2, authorizeResponse2.SecurityChallenges);

            var getAccountsDetailResult2 = apiClient2.GetAccountDetails(requestId2, null, null, null, null, DaysOfTransaction.Days365, null);

            Assert.NotEmpty(getAccountsDetailResult.Accounts[0].Transactions);
            Assert.NotEmpty(getAccountsDetailResult2.Accounts[0].Transactions);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(requestId2.ToString(), getAccountsDetailResult2.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult2.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient2.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_GetAccountsDetailResult_with_account_filter(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            var accountId = new Guid("fa37574c-65c8-48ed-cf2d-08d5a43b5981");

            var getAccountsDetailResult = apiClient.GetAccountDetails(requestId, null, null, null, null, null, new List<Guid>()
            {
                accountId
            });


            Assert.True(getAccountsDetailResult.Accounts.Count < 2);
            Assert.Equal(accountId, getAccountsDetailResult.Accounts[0].Id);
            Assert.NotNull(getAccountsDetailResult.Accounts);
            Assert.NotNull(getAccountsDetailResult.Login);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_call_GetAccountsDetailAsync_and_retrieve_GetAccountsDetailResult(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            apiClient.GetAccountDetails(requestId, null, null, null, null, null, null);

            var getAccountsDetailResult = apiClient.GetAccountDetailsAsync(requestId);

            Assert.NotNull(getAccountsDetailResult.Accounts);
            Assert.NotNull(getAccountsDetailResult.Login);
            Assert.Equal(requestId.ToString(), getAccountsDetailResult.RequestId);
            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }
    }
}