using Cromwell.Generator;
using Cromwell.Ui;
using Inanna.Ui;

[assembly: ViewPair(typeof(CredentialHeaderView), typeof(CredentialHeaderViewModel))]
[assembly: ViewPair(typeof(PaneView), typeof(PaneViewModel))]
[assembly: ViewPair(typeof(CredentialView), typeof(CredentialViewModel))]
[assembly: ViewPair(typeof(RootCredentialsView), typeof(RootCredentialsViewModel))]
[assembly: ViewPair(typeof(MainView), typeof(MainViewModel))]
[assembly: ViewPair(typeof(CredentialParametersView), typeof(CredentialParametersViewModel))]
[assembly: ViewPair(typeof(StackView), typeof(StackViewModel))]
[assembly: ViewPair(typeof(DialogView), typeof(DialogViewModel))]
[assembly: ViewPair(typeof(ExceptionView), typeof(ExceptionViewModel))]
[assembly: ViewPair(typeof(AppSettingView), typeof(AppSettingViewModel))]
[assembly: ViewPair(typeof(NavigationBarView), typeof(NavigationBarViewModel))]
[assembly: ViewPair(typeof(EditCredentialView), typeof(EditCredentialViewModel))]