using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpIsland
{
  public sealed partial class IslandControl : UserControl
  {
    public event Action LauchApp;

    public IslandControl()
    {
      InitializeComponent();
    }

    private void ClickMe(object sender, RoutedEventArgs e)
    {
      LauchApp?.Invoke();
    }
  }
}