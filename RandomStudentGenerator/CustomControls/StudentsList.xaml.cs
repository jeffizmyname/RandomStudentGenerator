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
        _classViewModel.setClass(ReadClass(className));
    }
}