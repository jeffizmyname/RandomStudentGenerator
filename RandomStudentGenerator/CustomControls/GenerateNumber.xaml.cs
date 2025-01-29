namespace RandomStudentGenerator.CustomControls;

using System.Diagnostics;
using System;
using System.IO.Ports;
using RandomStudentGenerator.StorageHandlers;
using System.Text.Json;

public partial class GenerateNumber : ContentView
{
    private string? classNameCurrent;
    private List<int> lastThreePool = new();
    Random random = new Random();
    private static SerialPort? serialPort;
    private string ComPortName = "COM12";

    public GenerateNumber()
    {
        InitializeComponent();
        if (OperatingSystem.IsWindows())
        {
            serialPort = new SerialPort(ComPortName, 9600);
            serialPort.DataReceived += SerialPort_DataReceived;
        }
        LoadLastThreePool();
    }

    private async void generateButton_Clicked(object sender, EventArgs e)
    {
        int result = await generateRandomNumber();
        generatedNumber.Text = result.ToString();
    }

    private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        if (serialPort == null || !OperatingSystem.IsWindows()) return;
        try
        {
            string data = serialPort.ReadExisting();
            if (data.Equals("1"))
            {
                string dataSend = (await generateRandomNumber()).ToString();
                MainThread.BeginInvokeOnMainThread(() => generatedNumber.Text = dataSend);
                serialPort.WriteLine(dataSend);
                Debug.WriteLine("data to send: " + dataSend);
            }
            Debug.WriteLine(data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading data: {ex.Message}");
        }
    }

    private async Task<int> generateRandomNumber()
    {
        if(classNameCurrent != StorageHandler.currentClass)
        {
            classNameCurrent = StorageHandler.currentClass;
            lastThreePool.Clear();
            await SecureStorage.SetAsync("lastThreePool", JsonSerializer.Serialize(lastThreePool));
        }

        int classSize = StorageHandler.currentClassSize;
        if (classSize <= 0) return -1; // do sth if cwass s-size is wess *screeches* than 4 to pwevent infinyit woop

        int number;
        do
        {
            number = random.Next(1, classSize + 1);

        } while (lastThreePool.Contains(number) || number == StorageHandler.happyNumber);

        lastThreePool.Add(number);
        if (lastThreePool.Count > 3) lastThreePool.RemoveAt(0);

        await SecureStorage.SetAsync("lastThreePool", JsonSerializer.Serialize(lastThreePool));
        return number;
    }

    private async void LoadLastThreePool()
    {
        string? lastThreePoolString = await SecureStorage.GetAsync("lastThreePool");
        if (lastThreePoolString != null)
        {
            lastThreePool = JsonSerializer.Deserialize<List<int>>(lastThreePoolString) ?? new List<int>();
        }
    }

    private void connectButton_Clicked(object sender, EventArgs e)
    {
        if (serialPort == null || !OperatingSystem.IsWindows()) return;

        if (serialPort.IsOpen)
        {
            connectButton.Text = "Disconnect";
            serialPort.Close();
        }
        else
        {
            connectButton.Text = "Connect";
            try
            {
                serialPort.Open();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Port {0} is in use", ComPortName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening port: {ex.Message}");
            }
        }
    }
}