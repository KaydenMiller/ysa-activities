using System.Text.Json;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using LaytonYsaClerk.EmailService;
using Microsoft.Playwright;

/* TOGGLE THIS TO ALLOW SENDING OF EMAILS */
const bool SendEmails = false;


const string MembersSeenFilePath = "./members-seen.json";
const string FromBishopName = "Trevor Steenblik";
const string FromBishopEmail = "tsteenblik@gmail.com";
const string FromBishopPhone = "(801) 540-8891";
const string FromClerkName = "Kayden Miller";
const string FromClerkEmail = "kaydenmiller1@gmail.com";

if (!File.Exists(MembersSeenFilePath))
{
    await using var fs = File.Create(MembersSeenFilePath);
}

using var playwright = await Playwright.CreateAsync();

var chrome = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
{
    Headless = false 
});

var page = await chrome.NewPageAsync();

Console.WriteLine("Performing page navigation");
await page.GotoAsync("https://www.churchofjesuschrist.org/my-home?lang=eng");
await page.ClickAsync("#signInButton");
await page.FillAsync("//input[@autocomplete=\"username\"]", "KaydenMiller");
await page.ClickAsync("input[type=\"password\"][name=\"credentials.passcode\"]");
await page.FillAsync("//input[@type=\"password\"]", "ZAQ!2wsx");
await page.ClickAsync("//input[@type=\"submit\"]");
Console.WriteLine("Finished Input of Username and Password");
await page.WaitForTimeoutAsync(3000);
await page.GotoAsync("https://lcr.churchofjesuschrist.org/report/members-moved-in?lang=eng");
await page.WaitForTimeoutAsync(3000);
Console.WriteLine("Gathering response from church about Members moved in");
var wardMembersResponse = await page.EvaluateAsync<string>("async (uri) => JSON.stringify(await (await fetch(uri)).json())", GetNewMembersUri(1).ToString());

var members = (JsonSerializer.Deserialize<IEnumerable<ChurchUser>>(wardMembersResponse) ?? Enumerable.Empty<ChurchUser>()).ToList();
Console.WriteLine($"Reading file data from system for file: {MembersSeenFilePath}");
var fileData = await File.ReadAllTextAsync(MembersSeenFilePath);
if (string.IsNullOrWhiteSpace(fileData)) fileData = "[]";
var seenMembers =
    (JsonSerializer.Deserialize<IEnumerable<ChurchUser>>(fileData) ?? Enumerable.Empty<ChurchUser>()).ToList();
Console.WriteLine($"Gathering Member Ids from previously seen members: {seenMembers.Count()} total");
var seenMemberIds = seenMembers.Select(m => m.MemberId);

var membersNotYetSeen = members.Where(m => !seenMemberIds.Contains(m.MemberId));

var emailTemplates = new List<EmailTemplate>();
foreach (var member in membersNotYetSeen)
{
    Console.WriteLine($"Processing Unit for Member: {member.FullName}");
    var oldUnitDetailsJson = await page.EvaluateAsync<string>(
        "async (uri) => JSON.stringify(await (await fetch(uri)).json())",
        GetUnitDetailsUri(member.PriorUnitNumber).ToString());
    var oldUnitDetails = JsonSerializer.Deserialize<UnitDetails>(oldUnitDetailsJson);
    if (oldUnitDetails is null)
    {
        throw new Exception($"Unit details didn't exist for member: '{member.FullName}'!");
    }
    Console.WriteLine($"Creating Email Template for member: {member.FullName}");
    emailTemplates.Add(new EmailTemplate(oldUnitDetails.LeaderName,
        oldUnitDetails.LeaderEmail,
        oldUnitDetails.PositionName,
        FromBishopName,
        FromBishopEmail,
        FromBishopPhone,
        member.FullName,
        FromClerkName));
}

File.Delete(MembersSeenFilePath);
await File.WriteAllTextAsync(MembersSeenFilePath, JsonSerializer.Serialize(members, new JsonSerializerOptions() { WriteIndented = true }));

foreach (var template in emailTemplates)
{
    Console.WriteLine($"Subject: {template.GetEmailSubject()}");
    Console.WriteLine($"Email To: {template.GetToHeader()}");
    Console.WriteLine($"Email Cc: {template.GetCcHeader()}");
    Console.WriteLine(template.GetEmailBody());
    Console.WriteLine("-----------------------------------------");
    Console.WriteLine("-----------------------------------------");
}

if (!SendEmails) return;
const string LaytonYsaApiKey = "AIzaSyB1YYunpwftrQQsc2hwHXgD1kNsXbb79TE";
const string ClientId = "";
const string DiscoveryDoc = "https://www.googleapis.com/discovery/v1/apis/gmail/v1/rest";
var scope = GmailService.Scope.GmailSend;

var client = new GmailService(new BaseClientService.Initializer()
{
    ApiKey = LaytonYsaApiKey,
    ApplicationName = "Layton YSA CLI Api Key",
});


var messages = new List<Message>();
foreach (var template in emailTemplates)
{
    var message = new Message()
    {
        Payload = new MessagePart()
        {
            Headers = new List<MessagePartHeader>()
            {
                // new MessagePartHeader()
                // {
                //     Name = "Cc",
                //     Value = template.GetCcHeader()
                // },
                new MessagePartHeader()
                {
                    Name = "To",
                    Value = "kaydenmiller1@gmail.com"
                },
            },
            Body = new MessagePartBody()
            {
                Data = template.GetEmailBodyAsBase64()
            }
        }
    };
    var sendRequest = new UsersResource.MessagesResource.SendRequest(client, message, "me");
    messages.Add(message);
}



return;

Uri GetNewMembersUri(int months)
{
    return new Uri($"https://lcr.churchofjesuschrist.org/api/report/members-moved-in/unit/266329/{months}?lang=eng");
}

Uri GetUnitDetailsUri(string unit)
{
    return new Uri($"https://lcr.churchofjesuschrist.org/api/cdol/details/unit/{unit}?lang=eng");
}