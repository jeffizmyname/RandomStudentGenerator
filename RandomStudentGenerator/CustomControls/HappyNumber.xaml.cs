using RandomStudentGenerator.StorageHandlers;
using System.Diagnostics;

namespace RandomStudentGenerator.CustomControls;

public partial class HappyNumber : ContentView
{
	public HappyNumber()
	{
		InitializeComponent();
        GenerateHappyNumber();
    }

    async void GenerateHappyNumber()
    {
        string? lastDate = await SecureStorage.GetAsync("lastDate");
        Debug.WriteLine("last date: " + lastDate);
        if (lastDate == null)
        {
            lastDate = DateTime.Now.ToString();
            await SecureStorage.SetAsync("lastDate", lastDate);
        }

        DateTime lastDateTime = DateTime.Parse(lastDate);
        DateTime now = DateTime.Now;

        if (lastDateTime.Date != now.Date)
        {
            Random random = new Random();
            int number = random.Next(1, StorageHandler.getMaxHappyNumber());
            happyNumber.Text = number.ToString();
            StorageHandler.happyNumber = number;
            await SecureStorage.SetAsync("lastDate", now.ToString());
            await SecureStorage.SetAsync("happyNumber", number.ToString());

        }
        else
        {
            string? happyNumber = await SecureStorage.GetAsync("happyNumber");
            Debug.WriteLine("happy number: " + happyNumber);
            if (happyNumber != null)
            {
                this.happyNumber.Text = happyNumber;
            } 
            else
            {
                Random random = new Random();
                int number = random.Next(1, StorageHandler.getMaxHappyNumber());
                StorageHandler.happyNumber = number;
                this.happyNumber.Text = number.ToString();
                await SecureStorage.SetAsync("happyNumber", number.ToString());

            }
        }

    }
}