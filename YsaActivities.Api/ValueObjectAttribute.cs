namespace YsaActivities.Api;

public class ValueObjectAttribute : Attribute 
{
    public string OpenApiType { get; }
    
    public ValueObjectAttribute(string openApiType)
    {
        OpenApiType = openApiType;
    }
}