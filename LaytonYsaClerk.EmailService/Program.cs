using LaytonYsaClerk.EmailService;
using System.Text.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

/* TOGGLE THIS TO ALLOW SENDING OF EMAILS */
const bool SendEmails = true;
const bool ReadMembers = false;
var apiKey = Environment.GetEnvironmentVariable("SEND_GRID_APIKEY") ?? throw new ArgumentNullException();

const string MembersSeenFilePath = "./members-seen.json";

if (!File.Exists(MembersSeenFilePath))
{
    await using var fs = File.Create(MembersSeenFilePath);
}

List<ChurchUser> churchUsers;
if (ReadMembers)
{ 
    var file = File.ReadAllText(MembersSeenFilePath);
    churchUsers = (JsonSerializer.Deserialize<IEnumerable<ChurchUser>>(file)).ToList() ?? [];
}
else
{
    var file = File.ReadAllText(MembersSeenFilePath);
    var alreadyFoundUsers = (JsonSerializer.Deserialize<IEnumerable<ChurchUser>>(file)).ToList() ?? [];
    var alreadyFoundUserIds = alreadyFoundUsers.Select(u => u.MemberId);
    var webUsers = (await GatherMembers.GetMembers("KaydenMiller", "ZAQ!2wsx", 1))
       .Where(u => !alreadyFoundUserIds.Contains(u.MemberId))
       .ToList();

    churchUsers = [..alreadyFoundUsers, ..webUsers];
}

var membersToEmailAbout = churchUsers.Where(m => m.NewMemberEmailSentDate is null).ToList();

if (!membersToEmailAbout.Any())
{
    Console.WriteLine("------------------------------");
    Console.WriteLine("No new members to email about!");
    Console.WriteLine("------------------------------");
    return;
}
else
{
    Console.WriteLine("------------------------------");
    Console.WriteLine($"There are {churchUsers.Count} members to email!");
    Console.WriteLine("------------------------------");
}

if (!SendEmails) return;
// Use send grid to send emails

var updatedMembers = churchUsers.Select(u =>
{
    u.NewMemberEmailSentDate = DateTime.UtcNow;
    return u;
});
await GatherMembers.WriteMembers(MembersSeenFilePath, updatedMembers);

var client = new SendGridClient(apiKey);

var shouldSkipForTesting = false;
foreach (var template in await GatherMembers.FromEmailTemplate(membersToEmailAbout))
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