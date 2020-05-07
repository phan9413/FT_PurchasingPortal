using System;
using Xamarin.Forms;
using XamarinPR.ViewModels;
using XamarinPR.Models;

namespace XamarinPR.Views
{
    public class csPage1 : ContentPage
    {
        Image XamarinImage;
        Editor NoteEditor;
        Button SaveButton, DeleteButton;
        public csPage1()
        {
            BackgroundColor = Color.PowderBlue;
            BindingContext = new csPage1ViewModel();

            XamarinImage = new Image
            {
                Source = "xamarin_logo.png"
            };
            NoteEditor = new Editor
            {
                Placeholder = "Enter Note",
                BackgroundColor = Color.White,
                Margin = new Thickness(10)
            };
            NoteEditor.SetBinding(Editor.TextProperty, nameof(csPage1ViewModel.TheNote));

            SaveButton = new Button
            {
                Text = "Save",
                TextColor = Color.White,
                BackgroundColor = Color.Green,
                Margin = new Thickness(10)
            };
            SaveButton.SetBinding(Button.CommandProperty, nameof(csPage1ViewModel.SaveCommand));
            //SaveButton.Clicked += SaveButton_Clicked;

            DeleteButton = new Button
            {
                Text = "Erase",
                TextColor = Color.White,
                BackgroundColor = Color.Red,
                Margin = new Thickness(10)
            };
            DeleteButton.SetBinding(Button.CommandProperty, nameof(csPage1ViewModel.EraseCommand));
            //DeleteButton.Clicked += DeleteButton_Clicked;

            var collectionView = new CollectionView
            {
                ItemTemplate = new NotesTemplate(),
                SelectionMode = SelectionMode.Single
            };
            collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(csPage1ViewModel.AllNotes));
            collectionView.SetBinding(CollectionView.SelectedItemProperty, nameof(csPage1ViewModel.SelectedNote));
            collectionView.SetBinding(CollectionView.SelectionChangedCommandProperty, nameof(csPage1ViewModel.SelectedNoteCommand));

            var grid = new Grid
            {
                Margin = new Thickness(20, 40),

                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(2.5, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(2.0, GridUnitType.Star) }
                }
            };

            grid.Children.Add(XamarinImage, 0, 0);
            Grid.SetColumnSpan(XamarinImage, 2);

            grid.Children.Add(NoteEditor, 0, 1);
            Grid.SetColumnSpan(NoteEditor, 2);

            grid.Children.Add(SaveButton, 0, 2);
            grid.Children.Add(DeleteButton, 1, 2);

            grid.Children.Add(collectionView, 0, 3);
            Grid.SetColumnSpan(collectionView, 2);

            Content = grid;
        }


        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            NoteEditor.Text = "";
        }
        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            NoteEditor.Text = string.Empty;
        }
    }
    class NotesTemplate : DataTemplate
    {
        public NotesTemplate() : base(LoadTemplate)
        { }

        static StackLayout LoadTemplate()
        {
            var textLabel = new Label();
            textLabel.SetBinding(Label.TextProperty, nameof(csPage1Model.text));

            var frame = new Frame
            {
                VerticalOptions = LayoutOptions.Center,
                Content = textLabel
            };

            return new StackLayout
            {
                Children = { frame },
                Padding = new Thickness(10, 10)
            };
        }
    }
}