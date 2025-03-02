using RandomStudentGenerator.Models;
using RandomStudentGenerator.StorageHandlers;
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
        StorageHandler.currentClassModel = _classViewModel;
    }

    public void CreateClass(string className)
    {
        _classViewModel.newClass(className);
        UpdateData(className);
    }

    private void addStudentButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(newStudentName.Text) || string.IsNullOrEmpty(newStudentSurname.Text)) return; // tez cos zrobic??
        _classViewModel.addStudent(newStudentName.Text, newStudentSurname.Text);
    }

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        _classViewModel.SelectedDate = presenceDate.Date;
    }

    private void AttendanceCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox toggle && toggle.BindingContext is Student student)
        {
            _classViewModel.setStudentPresence(student, e.Value);
            StorageHandler.savePresence(_classViewModel.Class.className, _classViewModel.Class.students.ToList());
        }
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        _classViewModel.updateClass();
    }

}