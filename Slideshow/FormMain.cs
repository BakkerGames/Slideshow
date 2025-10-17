namespace Slideshow;

public partial class FormMain : Form
{
    private bool closing = false;
    private int sleepDuration = 2000; // milliseconds
    private bool paused = false;
    private int currentIndex = 0;
    private readonly List<string> files = [];

    public FormMain()
    {
        InitializeComponent();
    }

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
        Close();
    }

    private void ShowSlideshow()
    {
        var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
        baseFolder = Path.GetFullPath(Path.Combine(baseFolder, @"..\..\..\..\"));
#endif
        var folder = Path.Combine(baseFolder, "Images");
        if (!Directory.Exists(folder))
        {
            MessageBox.Show($"Folder not found: {folder}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            closing = true;
            Application.Exit();
            Close();
            return;
        }
        files.Clear();
        foreach (var filename in Directory.EnumerateFiles(folder, "*.jpg"))
        {
            files.Add(filename);
        }
        if (files.Count == 0)
        {
            MessageBox.Show($"No .jpg files found in folder: {folder}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            closing = true;
            Application.Exit();
            this.Close();
            return;
        }
        files.Sort();
        currentIndex = 0;
        while (true)
        {
            if (closing)
            {
                break;
            }
            ShowImage(files[currentIndex]);
            Thread.Sleep(sleepDuration);
            while (paused && !closing)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
            if (closing)
            {
                break;
            }
            currentIndex++;
            if (currentIndex >= files.Count)
            {
                currentIndex = 0;
            }
        }
    }

    private void ShowImage(string filename)
    {
        if (closing)
        {
            return;
        }
        pictureBoxMain.Image?.Dispose();
        pictureBoxMain.Image = Image.FromFile(filename);
        Application.DoEvents();
    }

    private void PreviousImage()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = files.Count - 1;
        }
        ShowImage(files[currentIndex]);
    }

    private void NextImage()
    {
        currentIndex++;
        if (currentIndex >= files.Count)
        {
            currentIndex = 0;
        }
        ShowImage(files[currentIndex]);
    }

    private void FormMain_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Escape:
                closing = true;
                Application.Exit();
                this.Close();
                break;
            case Keys.Space:
                paused = !paused;
                break;
            case Keys.Oemplus:
            case Keys.Add:
                sleepDuration = Math.Max(500, sleepDuration - 500);
                paused = false;
                break;
            case Keys.OemMinus:
            case Keys.Subtract:
                sleepDuration = Math.Min(5000, sleepDuration + 500);
                paused = false;
                break;
            case Keys.Oemcomma:
            case Keys.Left:
                paused = true;
                PreviousImage();
                break;
            case Keys.OemPeriod:
            case Keys.Right:
                paused = true;
                NextImage();
                break;
        }
    }
}
