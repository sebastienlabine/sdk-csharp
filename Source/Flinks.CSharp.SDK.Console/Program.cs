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
            System.Console.WriteLine("Using GetSummary flow...");

            Thread.Sleep(1000);

            MimicGetAccountSummaryFlow();

            Thread.Sleep(1000);

            System.Console.WriteLine("Using GetAccountDetails flow...");

            Thread.Sleep(1000);

            MimicGetAccounteDetailFlow();

            System.Console.ReadLine();
        }

        private static void MimicGetAccountSummaryFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null, null);

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

        private static void MimicGetAccounteDetailFlow()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            System.Console.WriteLine("Calling Authorize...");

            //Calling basic Authorize
            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null, null);

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
