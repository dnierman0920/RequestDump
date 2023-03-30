using System.Net;
using System.Text;

namespace RequestDump;


class ApiReference
{
    private bool _old;
    private Uri _baseAddress;

    public bool Old
    {
        get => _old;
    }

    public Uri BaseAddress
    {
        get => _baseAddress;
    }

    public ApiReference(bool old, Uri baseAddress)
    {
        _old = old;
        _baseAddress = baseAddress;
    }

}

static class RequestDump
{
    private static HttpClient client = new();

    private static async Task<Object> getResponse(Uri uri)
    {
        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = uri,
            Headers = {
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            }
        };
        try
        {
            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            return response;
        }
        catch (Exception e)
        {
            return e;
        }

    }

    private static string CreateFilePath(string responsePath, ApiReference apiReference, bool body, string id)
    {
        string subDirectory = apiReference.Old ? "Old" : "New";
        string contentType = body ? "Body" : "Header";
        return responsePath + "/" + subDirectory + "/" + contentType + "-" + id + ".txt";
    }

    private static Uri CreateUriEndpoint(Uri baseAddress, string id)
    {
        return new Uri(baseAddress.ToString() + "/" + id);
    }

    static void WriteToFile(byte[] info, string filePath)
    {
        using FileStream fileStreamNew = new FileStream(filePath, FileMode.OpenOrCreate);
        fileStreamNew.Write(info, 0, info.Length);
        fileStreamNew.Close();
    }

    public static async void writeResponsesToFile(ApiReference apiReference, IEnumerable<string> ids, string responsePath)
    {

        foreach (string id in ids)
        {
            Uri ApiEndpoint = CreateUriEndpoint(apiReference.BaseAddress, id);
            Object response = await getResponse(ApiEndpoint);
            string bodyResponseFilePath = CreateFilePath(responsePath, apiReference, true, id);
            string headerResponseFilePath = CreateFilePath(responsePath, apiReference, false, id);
            try
            {
                HttpResponseMessage ResponseMessage = (HttpResponseMessage)response;

                byte[] headerInfo = new UTF8Encoding(true).GetBytes(ResponseMessage.Content.Headers.ToString());
                WriteToFile(headerInfo, headerResponseFilePath);

                byte[] contentInfo = await ResponseMessage.Content.ReadAsByteArrayAsync();
                WriteToFile(contentInfo, bodyResponseFilePath);

            }
            catch
            {
                try
                {
                    Exception responseException = (Exception)response;
                    byte[] responseBytes = new UTF8Encoding(true).GetBytes(responseException.Message);
                    WriteToFile(responseBytes, headerResponseFilePath);
                    WriteToFile(responseBytes, bodyResponseFilePath);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }

        }
    }
}


// ############################################

// ############################################

/*
class RequestDump
{
    enum ResponseSection
    {
        Header,
        Content
    }

    enum ApiReference
    {
        Old,
        New
    }

    private IEnumerable<string> _ids;
    private Uri _oldApiBaseAddress;
    private Uri _newApiBaseAddress;
    private string _responsesFolderPath;

    public RequestDump(IEnumerable<string> ids, Uri oldApi, Uri newApi, string responsesFolderPath)
    {
        _responsesFolderPath = responsesFolderPath;
        _oldApiBaseAddress = oldApi;
        _newApiBaseAddress = newApi;
        _ids = ids;
    }

    HttpClient client = new();
    private string oldApi;
    private string newApi;
    private string responsesFolderPath;

    async Task<Object> getResponse(Uri uri, string id)
    {
        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = uri,
            Headers = {
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            }
        };
        try
        {
            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            return response;
        }
        catch (Exception e)
        {
            return e;
        }

    }

    string CreateFilePath(string id, ApiReference apiReference, ResponseSection responseSection)
    {
        return _responsesFolderPath + "/" + apiReference + "/" + responseSection + "-" + id + ".txt";
    }

    Uri CreateUriEndpoint(Uri baseAddress, string id)
    {
        return new Uri(baseAddress.ToString() + "/" + id);
    }

    void WriteToFile(byte[] info, string filePath)
    {
        using FileStream fileStreamNew = new FileStream(filePath, FileMode.OpenOrCreate);
        fileStreamNew.Write(info, 0, info.Length);
        fileStreamNew.Close();
    }

    public async void writeResponsesToFile()
    {
        foreach (string id in _ids)
        {
            Uri oldApiEndpoint = CreateUriEndpoint(_oldApiBaseAddress, id);
            Object oldResponse = await getResponse(oldApiEndpoint, id);
            string oldContentFilePath = CreateFilePath(id, ApiReference.Old, ResponseSection.Content);
            try
            {
                HttpResponseMessage oldResponseMessage = (HttpResponseMessage)oldResponse;
                byte[] oldContentInfo = await oldResponseMessage.Content.ReadAsByteArrayAsync();
                WriteToFile(oldContentInfo, oldContentFilePath);

                string oldHeader = oldResponseMessage.Content.Headers.ToString();
                byte[] oldHeaderInfo = new UTF8Encoding(true).GetBytes(oldHeader);
                string oldHeaderFilePath = CreateFilePath(id, ApiReference.Old, ResponseSection.Header);
                WriteToFile(oldHeaderInfo, oldHeaderFilePath);
            }
            catch
            {
                try
                {
                    Exception oldResponseException = (Exception)oldResponse;
                    byte[] oldResponseBytes = new UTF8Encoding(true).GetBytes(oldResponseException.Message);
                    WriteToFile(oldResponseBytes, oldContentFilePath);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }






            // string newApiEndpoint = CreateUriEndpoint(_newApiBaseAddress, id);

            // HttpResponseMessage newResponse = await getResponse(newApiEndpoint, id);

            // byte[] newContent = await newResponse.Content.ReadAsByteArrayAsync();
            // string newContentFilePath = CreateFilePath(id, ApiReference.New, ResponseSection.Content);
            // WriteToFile(newContent, newContentFilePath);

            // string newHeader = newResponse.Content.Headers.ToString();
            // byte[] newHeaderInfo = new UTF8Encoding(true).GetBytes(newHeader);
            // string newHeaderFilePath = CreateFilePath(id, ApiReference.New, ResponseSection.Header);
            // WriteToFile(newHeaderInfo, newHeaderFilePath);
            // }


        }

    }
}

*/