namespace AlfinfData.ViewModels;

public class SeleccionViewModels : ContentPage
{
	public SeleccionViewModels()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}
}