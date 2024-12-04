using DB;
using Microsoft.EntityFrameworkCore;

namespace FoodBotSeeHashtag;

public class Program
{
    private static Bot bot;

    public static MainContext context;

    [STAThread]
    static void Main(string[] args)
    {
        context = new MainContext();
        context.Database.Migrate();

        bot = new Bot();

        //ApplicationConfiguration.Initialize();
        //Application.Run(new Form1());

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using (NotifyIcon icon = new NotifyIcon())
        {
            icon.Text = "FoodBot";
            icon.Icon = System.Drawing.Icon.ExtractAssociatedIcon($"{Path.GetDirectoryName(Application.ExecutablePath)}\\Data\\icon.ico");

            icon.ContextMenuStrip = new ContextMenuStrip();
            //icon.ContextMenuStrip.Items.Add("Show Form", null, (s, e) => { new Form1().Show(); });
            icon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => { Application.Exit(); });

            icon.Visible = true;
            Application.Run();
            icon.Visible = false;
        }

        bot.StopAsync();
    }


}