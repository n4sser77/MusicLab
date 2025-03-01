using System.Net.Http.Headers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;


namespace FileHandler;

public partial class UploadFileBtn : Button
{


    public UploadFileBtn()
    {
        // Set properties, if needed
        this.Text = "Upload File";
        this.Clicked += OnButtonClicked;
    }

    public async Task UploadFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            // Ensure the file stream is valid and has content.
            if (fileStream == null || fileStream.Length == 0)
            {
                Console.WriteLine("The file stream is empty.");
                return;
            }

            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();

            // Wrap the file stream in StreamContent
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

            // The name "File" here must match what the server's API expects.
            content.Add(fileContent, "File", fileName);

            // Post the form data to the API endpoint.
            var response = await client.PostAsync("http://localhost:5106/api/FileUpload", content);

            // Log the response.
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("File uploaded successfully.");
            }
            else
            {
                Console.WriteLine($"Error uploading file: {response.StatusCode}");
            }

            // Throw an exception if the status is not success.
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during upload: {ex.Message}");
            throw;
        }
    }
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        // Let the user pick a file.
        var result = await FilePicker.PickAsync();
        if (result == null)
        {
            Console.WriteLine("File picking was canceled.");
            return; // User canceled
        }

        string fileName = result.FileName;

        //// Optionally, save a local copy (if needed)
        //string destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        //using (var readStream = await result.OpenReadAsync())
        //using (var fileStream = File.Create(destinationPath))
        //{
        //    await readStream.CopyToAsync(fileStream);
        //}
        //Console.WriteLine($"Local copy saved to: {destinationPath}");

        // Re-open the file stream for uploading.
        // (If you want to avoid reading the file twice, you could avoid the local copy step,
        // but be sure not to reuse a consumed stream.)
        using Stream uploadStream = await result.OpenReadAsync();
        Console.WriteLine($"File {fileName} is being uploaded, stream length: {uploadStream.Length}");

        // Upload the file stream to the server.
        await UploadFileAsync(uploadStream, fileName);
    }
}