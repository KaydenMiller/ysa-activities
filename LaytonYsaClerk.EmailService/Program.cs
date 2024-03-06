using LaytonYsaClerk.EmailService;
using System.Text.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

/* TOGGLE THIS TO ALLOW SENDING OF EMAILS */
const bool SendEmails = true;
const bool ReadMembers = false;

const string MembersSeenFilePath = "./members-seen.json";

if (!File.Exists(MembersSeenFilePath))
{
    await using var fs = File.Create(MembersSeenFilePath);
}

IEnumerable<ChurchUser> churchUsers;
if (ReadMembers)
{ 
    var file = File.ReadAllText(MembersSeenFilePath);
    churchUsers = JsonSerializer.Deserialize<IEnumerable<ChurchUser>>(file) ?? Enumerable.Empty<ChurchUser>();
}
else
{
    churchUsers = await GatherMembers.GetMembers(MembersSeenFilePath);
}

if (!SendEmails) return;
// Use send grid to send emails
var apiKey = Environment.GetEnvironmentVariable("SEND_GRID_APIKEY") ?? throw new ArgumentNullException();
var client = new SendGridClient(apiKey);

var shouldSkipForTesting = false;
foreach (var template in await GatherMembers.FromEmailTemplate(churchUsers))
{
    if (shouldSkipForTesting) continue;
    
    var message = MailHelper.CreateSingleEmail(
        from: new EmailAddress(template.GetFromClerkEmail(), template.GetFromClerkName()),
        to: new EmailAddress(template.GetToBishopEmail(), template.GetToBishopName()),
        subject: template.GetEmailSubject(),
        plainTextContent: template.GetEmailBody(), htmlContent: template.GetHtmlBody()); 
    
    message.AddCc(new EmailAddress(template.GetFromBishopEmail(), template.GetFromBishopName()));

    var response = await client.SendEmailAsync(message);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Successfully Sent Email For: {template.GetEmailSubject()}");
        // shouldSkipForTesting = true;
    }
    else
    {
        Console.WriteLine($"FAILED TO SEND FOR: {template.GetEmailSubject()}");
        Console.WriteLine($"{response.StatusCode} - {await response.Body.ReadAsStringAsync()}");
        throw new Exception($"Failed to send email; stopping");
    }
}