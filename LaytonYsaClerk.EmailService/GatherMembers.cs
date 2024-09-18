using System.Text.Json;
using Microsoft.Playwright;

namespace LaytonYsaClerk.EmailService;

public static class GatherMembers
{
    const string FromBishopName = "Trevor Steenblik";
    const string FromBishopEmail = "tsteenblik@gmail.com";
    const string FromBishopPhone = "(801) 540-8891";
    const string FromClerkName = "Kayden Miller";
    const string FromClerkEmail = "kaydenmiller1@gmail.com";
    
    public static async Task<IEnumerable<ChurchUser>> GetMembers(string username, string password, int monthsToGather)
    {
        using var playwright = await Playwright.CreateAsync();

        var chrome = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false 
        });

        var page = await chrome.NewPageAsync();

        Console.WriteLine("Performing page navigation");
        await page.GotoAsync("https://www.churchofjesuschrist.org/my-home?lang=eng");
        await page.ClickAsync("#signInButton");
        await page.FillAsync("//input[@autocomplete=\"username\"]", username);
        await page.ClickAsync("//input[@type=\"submit\"]");
        await page.ClickAsync("//input[@name=\"credentials.passcode\"]");
        await page.FillAsync("//input[@type=\"password\"]", password);
        await page.ClickAsync("//input[@type=\"submit\"]");
        Console.WriteLine("Finished Input of Username and Password");
        await page.WaitForTimeoutAsync(3000);
        await page.GotoAsync("https://lcr.churchofjesuschrist.org/report/members-moved-in?lang=eng");
        await page.WaitForTimeoutAsync(3000);
        Console.WriteLine("Gathering response from church about Members moved in");
        var wardMembersResponse = await page.EvaluateAsync<string>("async (uri) => JSON.stringify(await (await fetch(uri)).json())", GetNewMembersUri(530409, monthsToGather).ToString());

        var members = (JsonSerializer.Deserialize<IEnumerable<ChurchUser>>(wardMembersResponse) ?? Enumerable.Empty<ChurchUser>()).ToList();

        foreach (var member in members)
        {
            Console.WriteLine($"Processing Unit for Member: {member.FullName}");
            var oldUnitDetailsJson = await page.EvaluateAsync<string>(
                "async (uri) => JSON.stringify(await (await fetch(uri)).json())",
                GetUnitDetailsUri(member.PriorUnitNumber).ToString());

            if (oldUnitDetailsJson.Contains("DOCTYPE"))
            {
                // This is a weird behavior that happens sometimes we just need to skip it for now
                Console.WriteLine($"Weird Behavior for user '{member.FullName}' with member id '{member.MemberId}'");
                continue;
            }
            
            var oldUnitDetails = JsonSerializer.Deserialize<UnitDetails>(oldUnitDetailsJson);
            if (oldUnitDetails is null)
            {
                throw new Exception($"Unit details didn't exist for member: '{member.FullName}'!");
            }

            member.UnitDetails = oldUnitDetails;
        }

        return members;
        
        Uri GetNewMembersUri(int wardNumber, int months)
        {
            return new Uri($"https://lcr.churchofjesuschrist.org/api/report/members-moved-in/unit/{wardNumber}/{months}?lang=eng");
        }

        Uri GetUnitDetailsUri(string unit)
        {
            return new Uri($"https://lcr.churchofjesuschrist.org/api/cdol/details/unit/{unit}?lang=eng");
        }
    }

    public static async Task WriteMembers(string filePath, IEnumerable<ChurchUser> churchUsers)
    {
        File.Delete(filePath);
        await File.WriteAllTextAsync(filePath, JsonSerializer
           .Serialize(churchUsers, new JsonSerializerOptions() { WriteIndented = true }));
    }

    public static async Task<IEnumerable<EmailTemplate>> FromEmailTemplate(IEnumerable<ChurchUser> members)
    {
        var emailTemplates = new List<EmailTemplate>();
        
        foreach (var member in members)
        {
            Console.WriteLine($"Creating Email Template for member: {member.FullName}");
            emailTemplates.Add(new EmailTemplate(member.UnitDetails.LeaderName,// oldUnitDetails.LeaderName,
                member.UnitDetails.LeaderEmail,//oldUnitDetails.LeaderEmail,
                member.UnitDetails.PositionName,//oldUnitDetails.PositionName,
                FromBishopName,
                FromBishopEmail,
                FromBishopPhone,
                member.FullName,
                FromClerkName,
                FromClerkEmail)); 
        }
        
        foreach (var template in emailTemplates)
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Subject: {template.GetEmailSubject()}");
            Console.WriteLine($"Email To: {template.GetToHeader()}");
            Console.WriteLine($"Email Cc: {template.GetCcHeader()}");
            Console.WriteLine(template.GetEmailBody());
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("-----------------------------------------");
        }
        
        return emailTemplates;
    }
}

