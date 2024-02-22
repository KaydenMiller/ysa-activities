/*
using System.Text.Json.Serialization;
using Flurl;
using Flurl.Http;

const string FromBishopEmail = "tsteenblik@gmail.com";
const string FromClerkEmail = "kaydenmiller1@gmail.com";

const string ResponseType = "code";
const string ClientId = "0oa5b6krts7UNNkID357";
const string NonceEndpoint = "https://id.churchofjesuschrist.org/api/v1/internal/device/nonce";
const string AuthorizationEndpoint = "https://id.churchofjesuschrist.org/oauth2/v1/authorize";
const string TokenEndpoint = "https://id.churchofjesuschrist.org/oauth2/v1/token";
const string AuthnEndpoint = "https://id.churchofjesuschrist.org/api/v1/authn";
const string VerifyEndpoint =
    "https://id.churchofjesuschrist.org/api/v1/authn/factors/password/verify?rememberDevice=false";
const string DeviceFingerprintEndpoint = "https://id.churchofjesuschrist.org/auth/services/devicefingerprint";
const string AuthnIntrospectEndpoint = "https://id.churchofjesuschrist.org/api/v1/authn/introspect";

var cookieJar = new CookieJar();

var response = await AuthorizationEndpoint
   .WithCookies(cookieJar)
   .AppendQueryParam("response_type", ResponseType)
    // .AppendQueryParam("response_mode", "query")
   .AppendQueryParam("client_id", ClientId)
   .AppendQueryParam("scope", "openid profile")
   .AppendQueryParam("redirect_uri", "https://www.churchofjesuschrist.org/services/platform/v4/login")
   .AppendQueryParam("state", "https://www.churchofjesuschrist.org/my-home?lang=eng")
   .GetAsync();

var introspectResponse = await AuthnIntrospectEndpoint
   .WithCookies(cookieJar)
   .PostJsonAsync(new
    {
        stateToken = "00DAM4HgPuXAhQGsxgIYEoZAZY2fFtPgIYZ7XBuQl4"
    });

var nonceResponse = await NonceEndpoint
   .WithCookies(cookieJar)
   .PostAsync();
Console.WriteLine(await nonceResponse.GetStringAsync());

var deviceFingerprintResponse = await DeviceFingerprintEndpoint
   .WithCookies(cookieJar)
   .GetAsync();

// make sure to get the oktaStateToken from the cookies as it is also the same as the authn stateToken
var oktaStateToken = cookieJar.SingleOrDefault(c => c.Name == "oktaStateToken")?.Value ?? "00DAM4HgPuXAhQGsxgIYEoZAZY2fFtPgIYZ7XBuQl4";
Console.WriteLine($"Okta State Token: {oktaStateToken}");
var authResponse = await AuthnEndpoint
   .WithCookies(cookieJar)
   .PostJsonAsync(new
    {
        options = new
        {
            multiOptionalFactorEnrol = true,
            warnBeforePasswordExpired = true
        },
        stateToken = oktaStateToken,
        username = "KaydenMiller"
    });

oktaStateToken = cookieJar.SingleOrDefault(c => c.Name == "oktaStateToken")?.Value ?? "00DAM4HgPuXAhQGsxgIYEoZAZY2fFtPgIYZ7XBuQl4";
Console.WriteLine($"Okta State Token: {oktaStateToken}");

var verifyResponse = await VerifyEndpoint
   .WithSettings(options =>
    {
        options.Redirects.Enabled = false;
    })
   .WithCookies(cookieJar)
   .PostJsonAsync(new
    {
        password = "ZAQ!2wsx",
        stateToken = oktaStateToken
    });

var locationHeader = verifyResponse.Headers.FirstOrDefault(h => h.Name == "Location");
Console.WriteLine($"Location: {locationHeader}");



class NonceResponse
{
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; }
    
    [JsonPropertyName("expiresAt")]
    public int ExpiresAt { get; set; }
}
*/