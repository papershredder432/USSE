using System.Threading;
using System.Windows.Forms;

namespace papershredder.Programs.USSE.GUI
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();

            // Starts a thread to close this after 5 seconds.
            new Thread(() =>
            {
                Thread.Sleep(5000);
                Close();
            }).Start();
        }
    }
}
