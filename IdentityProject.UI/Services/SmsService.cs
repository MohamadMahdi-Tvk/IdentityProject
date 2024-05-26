using System.Net;

namespace IdentityProject.UI.Services
{
    public class SmsService
    {
        public void Send(string phonenumber, string code)
        {
            var client = new WebClient();

            string url = $"http://panel.kavenegar.com/v1/apikey/verify/lookup.json?receptor={phonenumber}&token={code}&template=VerifyBugetoAccount";

            var content = client.DownloadString(url);
        }


    }
}
