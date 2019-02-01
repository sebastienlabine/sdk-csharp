using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Flinks.CSharp.SDK.Model;
using Flinks.CSharp.SDK.Model.Authorization;
using Flinks.CSharp.SDK.Model.Enums;
using Flinks.CSharp.SDK.Model.Score;
using Flinks.CSharp.SDK.Model.Shared;
using Newtonsoft.Json;

namespace Flinks.CSharp.SDK.Console
{
    class Program
    {
        static void Main()
        {
            //System.Console.WriteLine("Using GetSummary flow...");

            //Thread.Sleep(1000);

            //MimicGetAccountSummaryFlow();

            //Thread.Sleep(1000);

            //System.Console.WriteLine("Using GetAccountDetails flow...");

            //Thread.Sleep(1000);

            //MimicGetAccountDetailFlow();

            //System.Console.WriteLine("Using GetStatements flow...");

            //Thread.Sleep(1000);

            //MimicGetStatementsFlow();

            //System.Console.WriteLine("Using SetScheduledRefresh flow...");

            //Thread.Sleep(1000);

            //MimicSetNightlyRefreshFlow();

            //System.Console.WriteLine("Using DeleteCard flow...");

            //Thread.Sleep(1000);

            //MimicDeleteCardFlow();

            System.Console.WriteLine("Using GetScore flow...");

            Thread.Sleep(1000);

            MimicGetScoreFlow();

            System.Console.ReadLine();
        }

        private static void MimicGetAccountSummaryFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null,
                null, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            var requestId = new Guid(response.RequestId);

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(requestId, response.SecurityChallenges);

            System.Console.WriteLine("Calling GetAccountSummary...");

            var mfaRequestId = new Guid(mfaResponse.RequestId);

            //Calling Summary
            var summaryAnswer = flinksClient.GetAccountSummary(mfaRequestId);

            System.Console.WriteLine(JsonConvert.SerializeObject(summaryAnswer, Formatting.Indented));
        }

        private static void MimicGetAccountDetailFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null,null, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            var requestId = new Guid(response.RequestId);

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(requestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetAccountDetails...");

            var mfaRequestId = new Guid(mfaResponse.RequestId);

            //Calling Summary
            var accountDetails = flinksClient.GetAccountDetails(mfaRequestId, null, null, null, null, null, null);

            System.Console.WriteLine(JsonConvert.SerializeObject(accountDetails, Formatting.Indented));
        }

        private static void MimicGetStatementsFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null, null, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            var requestId = new Guid(response.RequestId);

            //Answering MFA
            var mfaResponse =  flinksClient.AnswerMfaQuestionsAndAuthorize(requestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetStatements...");

            var mfaRequestId = new Guid(mfaResponse.RequestId);

            //Calling Summary
            var accountStatements = flinksClient.GetStatementsAsync(mfaRequestId);

            System.Console.WriteLine(JsonConvert.SerializeObject(accountStatements, Formatting.Indented));
        }

        private static void MimicSetNightlyRefreshFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null,
                null, true, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            var requestId = new Guid(response.RequestId);

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(requestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetAccountDetails...");

            var mfaRequestId = new Guid(mfaResponse.RequestId);

            //Calling GetDetails
            var accountDetails = flinksClient.GetAccountDetails(mfaRequestId, null, null, null, null, null, null);

            System.Console.WriteLine("Calling SetScheduledRefresh...");

            var loginId = new Guid(accountDetails.Login.Id);

            //Changing ScheduledRefresh settings
            var nightlyRefreshResponse = flinksClient.SetScheduledRefresh(loginId, false);

            System.Console.WriteLine(nightlyRefreshResponse);
        }

        private static void MimicDeleteCardFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null,
                null, true, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            var requestId = new Guid(response.RequestId);

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(requestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetAccountDetails...");

            var mfaRequestId = new Guid(response.RequestId);

            //Calling Summary
            var accountDetails = flinksClient.GetAccountDetails(mfaRequestId, null, null, null, null, null, null);

            System.Console.WriteLine("Calling DeleteCard...");

            var loginId = new Guid(accountDetails.Login.Id);

            //Changing ScheduledRefresh settings
            var deletecardResponse = flinksClient.DeleteCard(loginId);

            System.Console.WriteLine(JsonConvert.SerializeObject(deletecardResponse, Formatting.Indented));
        }

        private static void MimicGetScoreFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null,
                null, true, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            var mfaRequestId = new Guid(response.RequestId);

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(mfaRequestId, response.SecurityChallenges);

            System.Console.WriteLine("Calling GetAccountDetails...");

            var authRequestId = new Guid(response.RequestId);

            //Calling Summary
            var accountDetails = flinksClient.GetAccountDetails(authRequestId, null, null, null, null, null, null);

            System.Console.WriteLine("Calling Authorize in cached mode...");

            var mostRecentCacheResponse = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay",
                true, true, null, null, true, null);

            System.Console.WriteLine("Calling GetScore...");

            var loginId = new Guid(mfaResponse.Login.Id);
            var requestId = new Guid(mostRecentCacheResponse.RequestId);
            var scoreRequestBody = new ScoreRequestBody()
            {
                LoanAmount = "1000.00",
                UserPersonalInformation = new UserPersonalInformation()
                {
                    FirstName = "Nicolas",
                    LastName = "Jourdain",
                    Sex = Sex.Male,
                    BirthDate = "1988-08-14",
                    Email = "nicolas.jourdain2@gmail.com",
                    SocialInsuranceNumber = "123456789",
                    Address = new Address()
                    {
                        CivicAddress = "123 fake street",
                        City = "Montreal",
                        Province = "Quebec",
                        PostalCode = "H0H0H0",
                        Country = "Canada",
                        PoBox = ""
                    }
                }
            };

            //Changing ScheduledRefresh settings
            var scoreResults = flinksClient.GetScore(loginId, requestId, scoreRequestBody);

            System.Console.WriteLine(JsonConvert.SerializeObject(scoreResults, Formatting.Indented));
        }


        private static void AnswerMfaQuestion(List<SecurityChallenge> securityChallenges)
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
    }
}
