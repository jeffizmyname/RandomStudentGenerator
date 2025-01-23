namespace RandomStudentGenerator.CustomControls;

public partial class HappyNumber : ContentView
{
	public HappyNumber()
	{
		InitializeComponent();
        Random random = new Random();
        int number = random.Next(1, 40);
        happyNumber.Text = number.ToString();
    }
}