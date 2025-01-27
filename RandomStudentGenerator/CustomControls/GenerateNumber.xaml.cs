namespace RandomStudentGenerator.CustomControls;

using System.Diagnostics;
using static RandomStudentGenerator.StorageHandlers.StorageHandler;
using System;
using System.IO.Ports;

public partial class GenerateNumber : ContentView
{
    private string className;
    private List<int> lastThreePool = new();
    Random random = new Random();
    static SerialPort serialPort;

    public GenerateNumber()
	{
		InitializeComponent();
        serialPort = new SerialPort("COM12", 9600);
        serialPort.DataReceived += SerialPort_DataReceived;
        
    }

    public void UpdateData(string className)
    {
        this.className = className;
    }

    private void generateButton_Clicked(object sender, EventArgs e)
    {
        generatedNumber.Text = generateRandomNumber().ToString();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            // Read all available data
            string data = serialPort.ReadExisting();
            if (data.Equals("1"))
            {
                string dataSend = generateRandomNumber().ToString();
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

    private int generateRandomNumber()
    {

        int classSize = getClassSize(className);
        if (classSize <= 0) return -1;
        int number = random.Next(1, classSize + 1);
        while (lastThreePool.Contains(number))
        {
            number = random.Next(1, classSize + 1);
        }
        lastThreePool.Add(number);
        if (lastThreePool.Count > 3) lastThreePool.RemoveAt(0);
        return number;
    }

    private void connectButton_Clicked(object sender, EventArgs e)
    {
        if (serialPort.IsOpen)
        {
            connectButton.Text = "Disconnect";
            serialPort.Close();
        }
        else
        {
            connectButton.Text = "Connect";
            serialPort.Open();
        }
    }
}