using Chat.Api.Constants;
using Chat.Api.Exceptions;

namespace Chat.Api.Helpers;

public static class StaticHelper
{
    public static string GetFullName(string firstName, string lastName)
    {
        return $"{firstName}  {lastName}";
    }

    public static void IsPhoto(IFormFile file)
    {
        var check = file.ContentType 
            is UserConstants.JpgType 
            or UserConstants.PngType;

        if (!check)
            throw new NotPhotoType();
    }

    public static byte[] GetData(IFormFile file)
    {
        var ms = new MemoryStream();

         file.CopyTo(target: ms);

        return ms.ToArray();
    }
}