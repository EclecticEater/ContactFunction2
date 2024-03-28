using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Communication.Email;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public class Contact
    {
        private readonly ILogger<Contact> _logger;

        public Contact(ILogger<Contact> logger)
        {
            _logger = logger;
        }

        [Function("Contact")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string emailAddress = data?.emailAddress;
            string userName = data?.userName;
            string message = data?.message;


            try
            {
                // Prepare the email request
                string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
                var emailClient = new EmailClient(connectionString);


                EmailSendOperation emailSendOperation = emailClient.Send(
                    WaitUntil.Completed,
                    senderAddress: "DoNotReply@f3ff103a-4ef2-48bb-9534-d909eff3ecfe.azurecomm.net",
                    recipientAddress: "justinpop24@gmail.com",
                    subject: "Message from: " + emailAddress,
                    htmlContent: $"<html><h1>Here is there message: {message}</h1l></html>",
                    plainTextContent: $"Here is there message: {message}");

                EmailSendOperation emailSendOperation2 = emailClient.Send(
                    WaitUntil.Completed,
                    senderAddress: "DoNotReply@f3ff103a-4ef2-48bb-9534-d909eff3ecfe.azurecomm.net",
                    recipientAddress: emailAddress,
                    subject: "Message from: " + emailAddress,
                    htmlContent: $"<html><h1>You have sent an email to Justin Popkowski</h1l></html>",
                    plainTextContent: "You have sent an email to Justin Popkowski");

                emailSendOperation2.WaitForCompletionAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
            }
       
        }
    }
}
