using System;

namespace ChildWindowForTaskbar
{
  public class Program
  {
    [STAThread]
    public static void Main()
    {
      using (new UwpIsland.App())
      {
        App app = new App();
        app.InitializeComponent();
        app.Run();
      }
    }
  }
}