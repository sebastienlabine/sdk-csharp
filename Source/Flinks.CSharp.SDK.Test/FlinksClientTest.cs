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
        public void Should_retrieve_receive_an_not_authorized_response_from_the_api(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var response = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            Assert.Equal(401, response.HttpStatusCode);
            Assert.Equal(ClientStatus.UNAUTHORIZED, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_retrieve_mfa_questions_from_api(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var response = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            Assert.Equal(203, response.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
        }

        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_answer_mfa_questions_and_receive_authorized(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            Assert.Equal(200, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.AUTHORIZED, apiClient.ClientStatus);
        }


        [Theory]
        [MemberData(nameof(AuthorizeTest.TestData), MemberType = typeof(AuthorizeTest))]
        public void Should_answer_mfa_questions_and_receive_non_authorative_information(string institution, string userName, string password, bool? save, bool? mostRecentCached, bool? withMfaQuestions, RequestLanguage? requestLanguage, bool? scheduleRefresh, string tag = null)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, save, mostRecentCached, withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var answerMfaQuestionsAndAuthorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, new List<SecurityChallenge>());

            Assert.Equal(203, answerMfaQuestionsAndAuthorizeResult.HttpStatusCode);
            Assert.Equal(ClientStatus.PENDING_MFA_ANSWERS, apiClient.ClientStatus);
        }
    }
}
