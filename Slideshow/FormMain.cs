namespace Slideshow;

public partial class FormMain : Form
{
    public FormMain()
    {
        InitializeComponent();
    }

    private bool closing = false;

    private void FormMain_Load(object sender, EventArgs e)
    {
        Task.Run(() => ShowSlideshow());
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        closing = true;
    }

    private void FormMain_Click(object sender, EventArgs e)
    {
        closing = true;
        Application.Exit();
        this.Close();
    }

    private void ShowSlideshow()
    {
        var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
        baseFolder = Path.GetFullPath(Path.Combine(baseFolder, @"..\..\..\..\"));
#endif
        var folder = Path.Combine(baseFolder, "Images");
        while (true)
        {
            foreach (var filename in Directory.EnumerateFiles(folder, "*.jpg"))
            {
                pictureBoxMain.Image = Image.FromFile(filename);
                Application.DoEvents();
                Thread.Sleep(2000);
                pictureBoxMain.Image.Dispose();
            }
            if (closing)
            {
                break;
            }
        }
    }

    private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.Handled = true;
        if (e.KeyChar == (char)27) // ESC
        {
            closing = true;
            Application.Exit();
            this.Close();
        }
    }
}
