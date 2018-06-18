using System.Threading;
using System.Windows.Forms;

namespace papershredder.Programs.USSE.GUI
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();

            new Thread(() =>
            {
                Thread.Sleep(5000);

                Invoke((MethodInvoker)delegate
                {
                    Close();
                });
            }).Start();
        }
    }
}
