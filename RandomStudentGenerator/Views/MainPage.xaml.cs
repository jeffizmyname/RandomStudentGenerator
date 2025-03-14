﻿namespace RandomStudentGenerator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            classSelector.SelectorChanged += (s, data) => studentList.UpdateData(data);
            classSelector.SelectorChanged += (s, data) => numberGenerator.UpdateData(data);
            classSelector.NewClass += (s, data) => studentList.CreateClass(data);
        }

    }

}
