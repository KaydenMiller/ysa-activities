using LaytonYsaClerk.EmailService;
using Microsoft.Extensions.Configuration;

namespace LaytonYSAClerk.Cli.Services;

public class WebsiteService
{
    private readonly IConfiguration _configuration;

    public WebsiteService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<IEnumerable<ChurchUser>> GetMembersFromWebsite()
    {
        var members = await GatherMembers.GetMembers("KaydenMiller", "ZAQ!2wsx", 1);
        return members;
    }
}