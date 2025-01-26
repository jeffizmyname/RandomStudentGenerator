using RandomStudentGenerator.ViewModels;
using System.Diagnostics;
using static RandomStudentGenerator.StorageHandlers.StorageHandler;

namespace RandomStudentGenerator.CustomControls;

public partial class StudentsList : ContentView
{
	public ClassViewModel _classViewModel { get; set; }
    public StudentsList()
	{
		InitializeComponent();
        _classViewModel = new ClassViewModel();
        studentsList.BindingContext = _classViewModel;
    }

    public void UpdateData(string className)
    {
        _classViewModel.setClass(className);
    }

    public void CreateClass(string className)
    {
        _classViewModel.newClass(className);
        UpdateData(className);
    }

    private void addStudentButton_Clicked(object sender, EventArgs e)
    {
        _classViewModel.addStudent(newStudentName.Text, newStudentSurname.Text);
    }
}