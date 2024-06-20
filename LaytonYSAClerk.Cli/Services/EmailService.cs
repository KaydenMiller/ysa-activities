using ErrorOr;
using LaytonYSAClerk.Cli.Entities;
using LaytonYsaClerk.EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LaytonYSAClerk.Cli.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _sendGridApiKey;
    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _logger = logger;
        var apikey = configuration.GetSection("SEND_GRID_APIKEY").Get<string>();
        if (apikey is null)
        {
            throw new Exception("SEND_GRID_APIKEY environment variable must not be null");
        }
        _sendGridApiKey = apikey;
    }

    public async Task<ErrorOr<Success>> SendEmailForMember(Member member)
    {
        var template = CreateEmailTemplate(member);
        _logger.LogInformation("Created template for member {MemberName}, {MemberId}", member.FullName, member.MemberId);
        
        var client = new SendGridClient(_sendGridApiKey);
        
        var message = MailHelper.CreateSingleEmail(
            from: new EmailAddress(template.GetFromClerkEmail(), template.GetFromClerkName()),
            to: new EmailAddress(template.GetToBishopEmail(), template.GetToBishopName()),
            subject: template.GetEmailSubject(),
            plainTextContent: template.GetEmailBody(), htmlContent: template.GetHtmlBody()); 
        
        message.AddCc(new EmailAddress(template.GetFromBishopEmail(), template.GetFromBishopName()));
        
        var response = await client.SendEmailAsync(message);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully Sent Email For: {EmailSubject}", template.GetEmailSubject());
            return Result.Success;
        }
        else
        {
            _logger.LogError("FAILED TO SEND FOR {MemberName}, {MemberId}", member.FullName, member.MemberId);
            return Error.Failure("email.send.failure", "Failed to send email for member");
        }
    }
    
    const string FromBishopName = "Trevor Steenblik";
    const string FromBishopEmail = "tsteenblik@gmail.com";
    const string FromBishopPhone = "(801) 540-8891";
    const string FromClerkName = "Kayden Miller";
    const string FromClerkEmail = "kaydenmiller1@gmail.com"; 
    private static EmailTemplate CreateEmailTemplate(Member member)
    {
        Console.WriteLine($"Creating Email Template for member: {member.FullName}");
        return new EmailTemplate(
            member.Unit.LeaderName,// oldUnitDetails.LeaderName,
            member.Unit.LeaderEmail,//oldUnitDetails.LeaderEmail,
            member.Unit.PositionName,//oldUnitDetails.PositionName,
            FromBishopName,
            FromBishopEmail,
            FromBishopPhone,
            member.FullName,
            FromClerkName,
            FromClerkEmail); 
    }
}