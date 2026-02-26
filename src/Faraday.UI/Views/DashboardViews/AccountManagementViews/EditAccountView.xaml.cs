using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Faraday.UI.ViewModels.DashboardViewModels.AccountManagementViewModels;

namespace Faraday.UI.Views.DashboardViews.AccountManagementViews;

public partial class EditAccountView : UserControl {
    public EditAccountView() {
        InitializeComponent();
    }
    
    private void GenerateColumnsForPreviewDataGrid(EditAccountViewModel vm) {
        if(vm.CsvData.Count == 0)
            return;

        PreviewDataGrid.Columns.Clear();

        object first = vm.CsvData.First();
        PropertyInfo[] props = first.GetType().GetProperties();

        foreach(PropertyInfo prop in props) {
            PreviewDataGrid.Columns.Add(
                new DataGridTextColumn {
                    Header = prop.Name,
                    Binding = new Binding(prop.Name)
                }
            );
        }
    }

    private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
        e.Handled = !Regex.IsMatch(e.Text, @"^-?[0-9.]*$");
    }

    private void NumberTextBox_Pasting(object sender, DataObjectPastingEventArgs e) {
        if(e.DataObject.GetDataPresent(typeof(string))) {
            string text = (string)e.DataObject.GetData(typeof(string));
            if(!Regex.IsMatch(text, "^[0-9]+$"))
                e.CancelCommand();
        }
        else {
            e.CancelCommand();
        }
    }

    private void PreviewDataGrid_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
        if(e.NewValue is EditAccountViewModel vm) {
            vm.PropertyChanged += (s, args) => {
                // When CsvData changes, regenerate columns
                if(args.PropertyName == nameof(vm.CsvData)) {
                    Dispatcher.Invoke(() => GenerateColumnsForPreviewDataGrid(vm));
                }
            };
        }
    }
}