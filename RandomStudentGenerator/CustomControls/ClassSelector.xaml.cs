using System.Diagnostics;

namespace RandomStudentGenerator.CustomControls;

public partial class ClassSelector : ContentView
{
	public ClassSelector()
	{
		InitializeComponent();
	}

    private async Task PickMultipleTextFilesAsync()
    {
        try
        {
            var customFileType = new FilePickerFileType(
                  new Dictionary<DevicePlatform, IEnumerable<string>>
                  {
                       { DevicePlatform.iOS, new[] { "public.text" } },
                       { DevicePlatform.Android, new[] { "text/plain" } },
                       { DevicePlatform.WinUI, new[] { ".txt" } },
                       { DevicePlatform.macOS, new[] { "txt" } },
                  });

            var pickOptions = new PickOptions
            {
                PickerTitle = "Select Text Files",
                FileTypes = customFileType
            };

            var result = await FilePicker.Default.PickMultipleAsync(pickOptions);

            if (result != null)
            {
                foreach (var file in result)
                {
                    string fileName = file.FileName;
                    using var stream = await file.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    string content = await reader.ReadToEndAsync();

                    Debug.WriteLine($"File: {fileName}");
                    Debug.WriteLine($"Content: {content}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async void addClassButton_Clicked(object sender, EventArgs e)
    {
        await PickMultipleTextFilesAsync();
    }
}