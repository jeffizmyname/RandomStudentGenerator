using System.Diagnostics;
using static RandomStudentGenerator.StorageHandlers.StorageHandler;

namespace RandomStudentGenerator.CustomControls;

public partial class ClassSelector : ContentView
{
    public event EventHandler<string>? SelectorChanged;
    public event EventHandler<string>? newClass;
    public ClassSelector()
    {
        InitializeComponent();
        classPicker.ItemsSource = ReadClassNames();
    }

    private async Task PickMultipleTextFilesAsync()
    {
        try
        {
            var customFileType = new FilePickerFileType(
                  new Dictionary<DevicePlatform, IEnumerable<string>>
                  {
                       { DevicePlatform.WinUI, new[] { ".csv" } },
                       { DevicePlatform.macOS, new[] { "csv" } },
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

                    Debug.WriteLine($"FilePAth: {file.FullPath}");
                    //Debug.WriteLine($"File: {fileName}");
                    //Debug.WriteLine($"Content: {content}");
                    AddClass(file.FullPath);
                    classPicker.ItemsSource = ReadClassNames();
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

    private void newClassButton_Clicked(object sender, EventArgs e)
    {
        newClass?.Invoke(this, newClassName.Text);
        classPicker.ItemsSource = ReadClassNames();
        classPicker.SelectedItem = newClassName.Text;
    }

    private void classPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedItem = classPicker.SelectedItem as string;
        if (selectedItem != null)
        {
            SelectorChanged?.Invoke(this, selectedItem);
        }
    }
}