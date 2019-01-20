using Flinks.CSharp.SDK.Model;
using Flinks.CSharp.SDK.Model.Shared;

namespace Flinks.CSharp.SDK.Console
{
    class Program
    {
        static void Main()
        {
            var flinksClient = new FlinksClient("b3c30383-2ec5-47bd-8ad0-33982d06fe06", "https://sandbox.flinks.io");

            var response = flinksClient.Authorize(Institution.FlinksCapital, "GreatDay", "EveryDay", true, null, null, null);

            switch (response.AuthorizationStatus)
            {
                case AuthorizationStatus.PENDING_MFA_ANSWERS:
                    {
                        foreach (var challenge in response.SecurityChallenges)
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

                        var mfaResponse = flinksClient.AnswerMfaQuestionsAndAuthorize(response.RequestId, response.SecurityChallenges);



                        return;
                    }
                default:
                    {
                        return;
                    }
            }


            System.Console.WriteLine(RequestLanguage.en.ToString());
            System.Console.ReadLine();
        }
    }
}
