namespace RandomStudentGenerator.CustomControls;

using System.Diagnostics;
using System;
using System.IO.Ports;
using RandomStudentGenerator.StorageHandlers;
using System.Text.Json;
using Microsoft.Maui.Controls;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class GenerateNumber : ContentView 
{
    private Dictionary<string, List<int>> classNumberPools = new();
    private string? classNameCurrent;
    Random random = new Random();
    private static SerialPort? serialPort;
    private string ComPortName = "COM4";
    private bool includeAbsent = false;


    public GenerateNumber()
    {
        InitializeComponent();
        if (OperatingSystem.IsWindows())
        {
            serialPort = new SerialPort(ComPortName, 9600);
            serialPort.DataReceived += SerialPort_DataReceived;
        }
        LoadNumberPools();
        LoadCheckBox();
    }

    private async void GenerateButton_Clicked(object sender, EventArgs e)
    {
        int result = await GenerateRandomNumber();
        generatedNumber.Text = result.ToString();
    }

    private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        if (serialPort == null || !OperatingSystem.IsWindows()) return;
        try
        {
            string data = serialPort.ReadLine();
            Debug.Write(data);
            if (data.Contains("1"))
            {
                string dataSend = (await GenerateRandomNumber()).ToString();
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
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading data: {ex}");
        }
    }

    private async Task<int> GenerateRandomNumber()
    {
        if (classNameCurrent != StorageHandler.currentClass)
        {
            classNameCurrent = StorageHandler.currentClass;

            if (!classNumberPools.ContainsKey(classNameCurrent))
            {
                classNumberPools[classNameCurrent] = new List<int>();
            }
        }

        List<int> lastThreePool = classNumberPools[classNameCurrent];

        int classSize = StorageHandler.currentClassSize;
        if (classSize <= 0) return -1;
        if(!includeAbsent && !StorageHandler.currentClassModel.Students.Any(s => s.CurrentPresence.isPresent == true)) return -1;

        int number;
        do
        {
            number = random.Next(1, classSize + 1);
        } while (lastThreePool.Contains(number)
                 || number == StorageHandler.happyNumber
                 || (!StorageHandler.currentClassModel.Students[number - 1].CurrentPresence.isPresent && !includeAbsent));

        lastThreePool.Add(number);
        if (lastThreePool.Count > 3) lastThreePool.RemoveAt(0);

        Debug.WriteLine("Generated number: " + number);
        Debug.WriteLine("Updated pool: " + String.Join(",", lastThreePool));

        LastDrawsList.Text = $"Last draws: {string.Join("  ", lastThreePool)}";

        await SaveNumberPools();
        return number;
    }

    private async void LoadNumberPools()
    {
        string? savedPools = await SecureStorage.GetAsync("classNumberPools");
        if (!string.IsNullOrEmpty(savedPools))
        {
            classNumberPools = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(savedPools)
                               ?? new Dictionary<string, List<int>>();
            if(classNameCurrent != null && classNumberPools.ContainsKey(classNameCurrent))
                LastDrawsList.Text = $"Last draws: {string.Join("  ", classNumberPools[classNameCurrent])}";
        }
    }

    private async void LoadCheckBox()
    {
        string? savedIncludeAbsent = await SecureStorage.GetAsync("includeAbsent");
        if (!string.IsNullOrEmpty(savedIncludeAbsent))
        {
            includeAbsent = bool.Parse(savedIncludeAbsent);
            includeAbsentCheckBox.IsChecked = includeAbsent;
        }
    }

    private async Task SaveNumberPools()
    {
        await SecureStorage.SetAsync("classNumberPools", JsonSerializer.Serialize(classNumberPools));
    }

    private void ConnectButton_Clicked(object sender, EventArgs e)
    {
        if (serialPort == null || !OperatingSystem.IsWindows()) return;

        if (serialPort.IsOpen) 
        {
            connectButton.Text = "Connect";
            connectButton.StyleClass = new[] { "buttonError" };
            serialPort.Close();
            colorPicker.IsEnabled = false;
            effectsPicker.IsEnabled = false;
        }
        else 
        {
            connectButton.Text = "Disconnect";
            connectButton.StyleClass = new[] { "empty" };
            colorPicker.IsEnabled = true;
            effectsPicker.IsEnabled = true;
            try
            {
                serialPort.Open();
                Debug.WriteLine("Port opened", ComPortName);
                ExecuteCommand("effects");
                Thread.Sleep(1000);
                ExecuteCommand("colors");
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("Error: Port is in use", ComPortName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening port: {ex.Message}");
            }
        }
    }

    private void ExecuteCommand(string command, string parameter = "")
    {
        if (serialPort == null || !OperatingSystem.IsWindows()) return;
        serialPort.WriteLine($"command:{command}:{parameter}");
    }

    private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        ExecuteCommand("setColor", colorPicker.SelectedItem.ToString());
    }

    private void EffectsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        ExecuteCommand("setEffect", effectsPicker.SelectedItem.ToString());
    }

    private async void IncludeAbsentCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        includeAbsent = e.Value;
        await SecureStorage.SetAsync("includeAbsent", includeAbsent.ToString());
    }

    public void UpdateData(string className)
    {
        if(classNumberPools.ContainsKey(className))
        {
            LastDrawsList.Text = $"Last draws: {string.Join("  ", classNumberPools[className])}";
        } else
        {
            LastDrawsList.Text = "Last draws: ";
        }
    }
}