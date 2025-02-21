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
    private string ComPortName = "COM4";
    private bool includeAbsent = true;

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
            else if(data.Contains("effects:")) 
            {
                string[] effects = data.Remove(0, 8).Remove(data.Length-9).Split(',');
                MainThread.BeginInvokeOnMainThread(() => {
                    effectsPicker.ItemsSource = effects;
                    effectsPicker.SelectedIndex = 0;
                });
                
            }
            else if(data.Contains("colors:"))
            {
                string[] colors = data.Remove(0, 7).Remove(data.Length - 8).Split(',');
                MainThread.BeginInvokeOnMainThread(() => {
                    colorPicker.ItemsSource = colors;
                    colorPicker.SelectedIndex = 0;
                });
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
        if (classSize <= 0) return -1;
        if(includeAbsent && !StorageHandler.currentClassModel.Students.Any(s => s.CurrentPresence.isPresent == true)) return -1;

        int number;
        do
        {
            number = random.Next(1, classSize + 1);
        } while (lastThreePool.Contains(number)
                 || number == StorageHandler.happyNumber
                 || (!StorageHandler.currentClassModel.Students[number - 1].CurrentPresence.isPresent && includeAbsent));

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

    private void executeCommand(string command, string parameter = "")
    {
        if (serialPort == null || !OperatingSystem.IsWindows()) return;
        serialPort.WriteLine($"command:{command}:{parameter}");
    }

    private void colorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        executeCommand("setColor", colorPicker.SelectedItem.ToString());
    }

    private void effectsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        executeCommand("setEffect", effectsPicker.SelectedItem.ToString());

        }

    private void includeAbsentCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        includeAbsent = !includeAbsent;
    }
}