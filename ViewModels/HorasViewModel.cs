namespace AlfinfData.ViewModels;

public class HorasViewModels : ContentPage
{
	public HorasViewModels()
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