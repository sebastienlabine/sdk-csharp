using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Flinks.CSharp.SDK.Model;
using Flinks.CSharp.SDK.Model.Authorization;
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

            System.Console.WriteLine("Using DeleteCard flow...");

            Thread.Sleep(1000);

            MimicSetDeleteCardFlow();

            System.Console.ReadLine();
        }

        private static void MimicGetAccountSummaryFlow()
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

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(response.RequestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetAccountSummary...");

            //Calling Summary
            var summaryAnswer = flinksClient.GetAccountSummary(mfaResponse.RequestId);

            System.Console.WriteLine(JsonConvert.SerializeObject(summaryAnswer, Formatting.Indented));
        }

        private static void MimicGetAccountDetailFlow()
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

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(response.RequestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetAccountDetails...");

            //Calling Summary
            var accountDetails = flinksClient.GetAccountDetails(mfaResponse.RequestId, null, null, null, null, null, null);

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

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(response.RequestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetStatements...");

            //Calling Summary
            //var accountStatements = flinksClient.GetStatements(mfaResponse.RequestId, NumberOfStatements.Months3, null);
            var accountStatements = flinksClient.GetStatementsAsync(mfaResponse.RequestId);

            System.Console.WriteLine(JsonConvert.SerializeObject(accountStatements, Formatting.Indented));
        }

        private static void MimicSetNightlyRefreshFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null, null, true, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(response.RequestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetAccountDetails...");

            //Calling GetDetails
            var accountDetails = flinksClient.GetAccountDetails(response.RequestId, null, null, null, null, null, null);

            System.Console.WriteLine("Calling SetScheduledRefresh...");

            //Changing ScheduledRefresh settings
            var nightlyRefreshResponse = flinksClient.SetScheduledRefresh(accountDetails.Login.Id, false);

            System.Console.WriteLine(nightlyRefreshResponse);
        }

        private static void MimicSetDeleteCardFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null, null, true, null);

            //Pretending to be the client answering the MFA questions
            if (response.AuthorizationStatus == AuthorizationStatus.PENDING_MFA_ANSWERS)
            {
                AnswerMfaQuestion(response.SecurityChallenges);
            }

            System.Console.WriteLine("Answering MFA...");

            //Answering MFA
            var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(response.RequestId, response.SecurityChallenges);


            System.Console.WriteLine("Calling GetAccountDetails...");

            //Calling Summary
            var accountDetails = flinksClient.GetAccountDetails(response.RequestId, null, null, null, null, null, null);

            System.Console.WriteLine("Calling DeleteCard...");

            //Changing ScheduledRefresh settings
            var deletecardResponse = flinksClient.DeleteCard(accountDetails.Login.Id);

            System.Console.WriteLine(JsonConvert.SerializeObject(deletecardResponse, Formatting.Indented));
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
