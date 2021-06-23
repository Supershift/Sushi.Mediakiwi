namespace Sushi.Mediakiwi.Framework
{
    public interface iMappedUrlCreator
    {
        string CreateMappedUrl(string urlMappingName, bool UseSpaceReplacement = false, params object[] args);
    }

 
}
