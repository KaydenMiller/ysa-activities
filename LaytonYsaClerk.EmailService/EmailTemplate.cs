using System.Text;

namespace LaytonYsaClerk.EmailService;

public class EmailTemplate
{
    /// <summary>
    /// The bishop we are sending the email to
    /// </summary>
    private readonly string _toBishop;

    private readonly string _toBishopEmail;
    private readonly string _toPositionTitle;
    /// <summary>
    /// The layton ysa bishop
    /// </summary>
    private readonly string _fromBishop;
    private readonly string _fromBishopEmail;
    private readonly string _fromBishopPhone;
    private readonly string _newMember;
    private readonly string _clerk;

    public EmailTemplate(string toBishop, string toBishopEmail, string toPositionTitle, string fromBishop, string fromBishopEmail, string fromBishopPhone, string newMember, string clerk)
    {
        _toBishop = toBishop;
        _toBishopEmail = toBishopEmail;
        _toPositionTitle = toPositionTitle;
        _fromBishop = fromBishop;
        _fromBishopEmail = fromBishopEmail;
        _fromBishopPhone = fromBishopPhone;
        _newMember = newMember;
        _clerk = clerk;
    }

    public string GetCcHeader()
    {
        return _fromBishopEmail;
    }

    public string GetToHeader()
    {
        return _toBishopEmail;
    }

    public string GetEmailSubject()
    {
        return $"{_newMember}";
    }

    public string GetEmailBody()
    {
        return $"""
            Dear {_toPositionTitle} {_toBishop},
            
            I'm writing on behalf of Bishop {_fromBishop} of the Layton Young Single Adult Ward. We recently received the membership record of {_newMember}, who previously belonged to your ward.
            Are there any special circumstances or special needs we need to be aware of? Is there any reason this person cannot hold a position or be issued a temple recommend? If so, please contact
            Bishop {_fromBishop} at {_fromBishopEmail} or {_fromBishopPhone}.
            
            Thank you,
            
            {_clerk}
            Assistant Ward Clerk
            """;
    }

    public string GetEmailBodyAsBase64()
    {
        var body = GetEmailBody();
        var bytes = Encoding.UTF8.GetBytes(body);
        return Convert.ToBase64String(bytes);
    }
}