using Cromwell.Generator;
using Cromwell.Ui;
using EditCredentialViewModel = Cromwell.Ui.EditCredentialViewModel;
using MainViewModel = Cromwell.Ui.MainViewModel;

[assembly: ViewPair(typeof(MainView), typeof(MainViewModel))]
[assembly: ViewPair(typeof(EditCredentialView), typeof(EditCredentialViewModel))]
[assembly: ViewPair(typeof(StackView), typeof(StackViewModel))]
[assembly: ViewPair(typeof(DialogView), typeof(DialogViewModel))]