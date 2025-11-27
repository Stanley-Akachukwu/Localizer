using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Lacalizer.Mobile.ViewModels;
using System.Diagnostics;

namespace Lacalizer.Mobile.Views;

[QueryProperty(nameof(Topic), "topic")]
public partial class CameraPage : ContentPage
{
    private readonly CameraVewModel _cvm;

    public CameraPage(CameraVewModel cvm)
    {
        InitializeComponent();
        BindingContext = _cvm = cvm;
    }

    private string _topic;
    public string Topic
    {
        get => _topic;
        set
        {
            _topic = value;

            if (_cvm != null)
                _cvm.SelectedTopic = value; // update label immediately
        }
    }
    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        Console.WriteLine($"Camera Loaded — topic = {Topic}");
        cameraView.Camera = cameraView.Cameras.LastOrDefault();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            
            if (cameraView.Cameras.Count > 0)
            {
                cameraView.Camera = cameraView.Cameras.LastOrDefault();
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            }
        });
    }

    
    private async void Button_Clicked(object sender, EventArgs e)
   {
    var storageStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
    var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
    var microphoneStatus = await Permissions.RequestAsync<Permissions.Microphone>();

    if (storageStatus != PermissionStatus.Granted ||
        cameraStatus != PermissionStatus.Granted ||
        microphoneStatus != PermissionStatus.Granted)
    {
        await DisplayAlert("Permission Denied",
            "Camera, microphone, and storage permissions are required to record a video.",
            "OK");
        return;
    }

    try
    {


#if ANDROID
            string folderPath = Path.Combine(FileSystem.AppDataDirectory, "Localized");
            Directory.CreateDirectory(folderPath);
#else
                    // For Windows or other platforms
                    string folderPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Lacalized");
#endif

            // 2️⃣ Create timestamped filename
            string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.mp4";
            string filePath = Path.Combine(folderPath, fileName);

            // 3️⃣ Start recording
            var result = await cameraView.StartRecordingAsync(filePath, new Size(1920, 1080));

            // ⏱ Record duration
            await Task.Delay(TimeSpan.FromSeconds(7));

            // 4️⃣ Stop recording
            result = await cameraView.StopRecordingAsync();

            // Ensure file exists before uploading
            if (!File.Exists(filePath))
                throw new Exception("Video file not found. Recording may have failed.");

            // 5️⃣ Upload to Azurite Blob Storage
            await UploadToAzuriteAsync(filePath, fileName);

            Console.WriteLine($"Uploaded successfully: {fileName}");

        await DisplayAlert("Recording Complete", $"Video saved at: {filePath}", "OK");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error recording video: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        await DisplayAlert("Error", ex.Message, "OK");
    }

        //string blobName = "20251124144108.mp4"; // Example filename
        //string localPath = await DownloadVideoFromAzuriteAsync(blobName);
    }
    private async Task UploadToAzuriteAsync(string filePath, string blobName)
    {
        try
        {
            string connectionString;

#if ANDROID
            connectionString =
            "AccountName=devstoreaccount1;" +
            "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
            "DefaultEndpointsProtocol=http;" +
            "BlobEndpoint=http://192.168.1.227:10000/devstoreaccount1;" +
            "QueueEndpoint=http://192.168.1.227:10001/devstoreaccount1;" +
            "TableEndpoint=http://192.168.1.227:10002/devstoreaccount1;";

#else
            // Windows or MAUI WinUI
            connectionString =
            "AccountName=devstoreaccount1;" +
            "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
            "DefaultEndpointsProtocol=http;" +
            "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;" +
            "QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;" +
            "TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
#endif

            string containerName = "videos";
            string subFolder = "localized";

            var service = new BlobServiceClient(connectionString);
            var container = service.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            string blobPath = $"{subFolder}/{blobName}";
            var blob = container.GetBlobClient(blobPath);

            //using FileStream fs = File.OpenRead(filePath);
            //await blob.UploadAsync(fs, overwrite: true);

            //Debug.WriteLine($"Uploaded to Azurite: {blobPath}");

            //+++++++++++++++++++++++++++++++++++++++++
            // Define the content type
            BlobHttpHeaders httpHeaders = new BlobHttpHeaders
            {
                ContentType = "video/mp4"
            };

            using (FileStream uploadFileStream = File.OpenRead(filePath))
            {
                await blob.UploadAsync(uploadFileStream, new BlobUploadOptions { HttpHeaders = httpHeaders });
            }
            //+++++++++++++++++++++++++++++++++++++++++

            // 🔥 Delete local file after upload
            if (File.Exists(filePath))
                File.Delete(filePath);
           await SaveVideoItemAsync(blob.Uri.ToString());
        }
        catch (Exception ex)
        {
            Debug.WriteLine("UPLOAD ERROR: " + ex);
        }
    }

    private Task SaveVideoItemAsync(string videoUrl)
    {
        var videoItem = new
        {
            Title = "Localized Video",
            Topic = Topic,
            Language = "Localized Language",
            VideoUri = videoUrl
        };
        // Here you would typically send this videoItem to your backend API
        //handll task complete
        return Task.CompletedTask;
    }

    //private async Task UploadToAzuriteAsync(string filePath, string blobName)
    //{
    //    try
    //    {
    //        string connectionString =
    //            "AccountName=devstoreaccount1;" +
    //            "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
    //            "DefaultEndpointsProtocol=http;" +
    //            "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;" +
    //            "QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;" +
    //            "TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    //        string containerName = "videos";
    //        string subFolder = "localized";

    //        var service = new BlobServiceClient(connectionString);
    //        var container = service.GetBlobContainerClient(containerName);
    //        await container.CreateIfNotExistsAsync();

    //        string blobPath = $"{subFolder}/{blobName}";
    //        var blob = container.GetBlobClient(blobPath);

    //        // Determine MIME type
    //        string contentType = Path.GetExtension(filePath).ToLower() switch
    //        {
    //            ".mp4" => "video/mp4",
    //            ".mov" => "video/quicktime",
    //            _ => "application/octet-stream"
    //        };

    //        using FileStream fs = File.OpenRead(filePath);

    //        // Step 1: Upload file with overwrite
    //        await blob.UploadAsync(fs, overwrite: true);

    //        // Step 2: Set headers (ContentType)
    //        var headers = new BlobHttpHeaders { ContentType = contentType };
    //        await blob.SetHttpHeadersAsync(headers);

    //        Debug.WriteLine($"Uploaded to Azurite: {blobPath}");

    //        // Delete local file
    //        if (File.Exists(filePath))
    //            File.Delete(filePath);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine("UPLOAD ERROR: " + ex);
    //    }
    //}
    private async Task<string> DownloadVideoFromAzuriteAsync(string blobName)
    {
        string connectionString;

#if ANDROID
        connectionString =
            "AccountName=devstoreaccount1;" +
            "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
            "DefaultEndpointsProtocol=http;" +
            "BlobEndpoint=http://192.168.1.227:10000/devstoreaccount1;";
#else
    connectionString =
        "AccountName=devstoreaccount1;" +
        "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
        "DefaultEndpointsProtocol=http;" +
        "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
#endif

        string containerName = "videos";
        string subFolder = "localized";

        var service = new BlobServiceClient(connectionString);
        var container = service.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient($"{subFolder}/{blobName}");

        // Local path to save
        string localPath = Path.Combine(FileSystem.AppDataDirectory, blobName);

        using (var fs = File.OpenWrite(localPath))
        {
            await blob.DownloadToAsync(fs);
        }

        return localPath;
    }


}