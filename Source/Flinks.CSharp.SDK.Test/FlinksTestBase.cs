using System;
using System.Collections.Generic;
using Flinks.CSharp.SDK.Model.Authorize;
using Flinks.CSharp.SDK.Model.Enums;
using Flinks.CSharp.SDK.Model.Shared;

namespace Flinks.CSharp.SDK.Test
{
    public class FlinksTestBase
    {
        public string CustomerId = "b3c30383-2ec5-47bd-8ad0-33982d06fe06";
        public string Endpoint = "https://sandbox.flinks.io";

        public static class FlinksClientInstantiationValueTest
        {
            private static readonly List<object[]> Data = new List<object[]>
            {
                new object[] { string.Empty, string.Empty },
                new object[] { null, null},
                new object[] { string.Empty, null}
            };

            public static IEnumerable<object[]> TestData => Data;
        }

        public static class AuthorizeTest
        {
            private static readonly List<object[]> Data = new List<object[]>
            {
                new object[] { Institution.FlinksCapital, "GreatDay", "EveryDay", false, null, null, null, null, null },
                new object[] { Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null, null, null, null },
            };

            public static IEnumerable<object[]> TestData => Data;

        }

        public static class AuthorizeTestWrongPassword
        {
            private static readonly List<object[]> Data = new List<object[]>
            {
                new object[] { Institution.FlinksCapital, "GreatDay", "WrongPassword", false, null, null, null, null, null },
                new object[] { Institution.FlinksCapital, "GreatDay", "WrongPassword", true, null, null, null, null, null },
            };

            public static IEnumerable<object[]> TestData => Data;

        }

        public static void AnswerMfaQuestion(List<SecurityChallenge> securityChallenges)
        {
            foreach (var challenge in securityChallenges)
            {
                if (challenge.Prompt.Contains("city"))
                {
                    challenge.Answer = "Montreal";
                }

                if (challenge.Prompt.Contains("country"))
                {
                    challenge.Answer = "Canada";
                }

                if (challenge.Prompt.Contains("shape"))
                {
                    challenge.Answer = "Triangle";
                }
            }
        }

        public static bool IsSecurityChallengeInFrench(List<SecurityChallenge> securityChallenges)
        {
            foreach (var challenge in securityChallenges)
            {
                if (challenge.Prompt.Contains("Où") || challenge.Prompt.Contains("Quel") || challenge.Prompt.Contains("Quelle"))
                {
                    return true;
                }

                
            }

            return false;
        }

        public Tuple<Guid, FlinksClient> AuthorizeFlow(
            string institution, 
            string userName, 
            string password, 
            bool? mostRecentCached, 
            bool? withMfaQuestions, 
            RequestLanguage? requestLanguage, 
            bool? scheduleRefresh, 
            string tag)
        {
            var apiClient = new FlinksClient(CustomerId, Endpoint);

            var authorizeResponse = apiClient.Authorize(institution, userName, password, false, mostRecentCached,
                withMfaQuestions, requestLanguage, scheduleRefresh, tag);

            if (authorizeResponse.ClientStatus == ClientStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(authorizeResponse.SecurityChallenges);
            }

            var requestId = new Guid(authorizeResponse.RequestId);

            var authorizeResult = apiClient.AnswerMfaQuestionsAndAuthorize(requestId, authorizeResponse.SecurityChallenges);

            return new Tuple<Guid, FlinksClient>(new Guid(authorizeResult.RequestId), apiClient);
        }

    }
}
