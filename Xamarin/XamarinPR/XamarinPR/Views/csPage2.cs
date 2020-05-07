using Xamarin.Forms;
using XamarinPR.ViewModels;

namespace XamarinPR.Views
{
    public class csPage2 : ContentPage
    {
        public csPage2(csPage2ViewModel viewModel)
        {
            BindingContext = viewModel;

            Title = "Notes Detail";

            BackgroundColor = Color.PowderBlue;

            var textLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            textLabel.SetBinding(Label.TextProperty, nameof(csPage2ViewModel.Note));

            var exitButton = new Button
            {
                Text = "Pop",
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(20),
                BackgroundColor = Color.Red,
                TextColor = Color.White,
                FontSize = 20
            };
            exitButton.SetBinding(Button.CommandProperty, nameof(csPage2ViewModel.DismissCommand));

            var stackLayout = new StackLayout
            {
                Margin = new Thickness(20, 40)
            };
            stackLayout.Children.Add(textLabel);
            stackLayout.Children.Add(exitButton);

            Content = stackLayout;
        }
    }
}