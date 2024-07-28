using System;
using System.Windows.Forms;
using Markdig;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Reflection.Metadata;
using System.Security.Cryptography.Xml;
using System.Reflection;
using LibGit2Sharp;
using ReverseMarkdown;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using NHunspell;
using FastColoredTextBoxNS;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Markdig;

namespace Markdown_Editor
{ 
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer autoSaveTimer;
        private List<string> recentFiles = new List<string>();

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Set form properties
            this.Text = "Advanced Markdown Editor";
            this.Size = new System.Drawing.Size(1200, 800);

            // Set theme (Light or Dark)
            SetDarkTheme();

            // Setup Auto-Save Timer
            autoSaveTimer = new System.Windows.Forms.Timer();
            autoSaveTimer.Interval = 300000; // 5 minutes
            autoSaveTimer.Tick += AutoSave;
            autoSaveTimer.Start();

            // Initialize MenuStrip
            MenuStrip menuStrip = new MenuStrip();

            // File Menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Recent Files", null, (s, e) => AddRecentFiles());
            fileMenu.DropDownItems.Add("Print Preview", null, (s, e) => AddPrintPreview());
            fileMenu.DropDownItems.Add("Export to PDF", null, (s, e) => AddExportToPdf());
            fileMenu.DropDownItems.Add("Import HTML", null, (s, e) => AddImportHtml());
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => this.Close());

            // Edit Menu
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Edit");
            editMenu.DropDownItems.Add("Find & Replace", null, (s, e) => AddFindReplace());
            editMenu.DropDownItems.Add("Insert Tables", null, (s, e) => AddInsertTables());
            editMenu.DropDownItems.Add("Auto Formatting", null, (s, e) => AddAutoFormatting());

            // View Menu
            ToolStripMenuItem viewMenu = new ToolStripMenuItem("View");
            viewMenu.DropDownItems.Add("Full Screen Mode", null, (s, e) => AddFullScreenMode());
            viewMenu.DropDownItems.Add("Split View", null, (s, e) => AddSplitView());

            // Tools Menu
            ToolStripMenuItem toolsMenu = new ToolStripMenuItem("Tools");
            toolsMenu.DropDownItems.Add("Word Count", null, (s, e) => AddWordCount());
            toolsMenu.DropDownItems.Add("Syntax Highlighting", null, (s, e) => AddSyntaxHighlighting());
            toolsMenu.DropDownItems.Add("Spell Check", null, (s, e) => AddSpellCheck());
            toolsMenu.DropDownItems.Add("Drag & Drop Images", null, (s, e) => AddDragDropImages());
            toolsMenu.DropDownItems.Add("Version Control", null, (s, e) => AddVersionControl());

            // Plugins Menu
            ToolStripMenuItem pluginsMenu = new ToolStripMenuItem("Plugins");
            pluginsMenu.DropDownItems.Add("Plugin System", null, (s, e) => AddPluginSystem());

            // Add menus to MenuStrip
            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(editMenu);
            menuStrip.Items.Add(viewMenu);
            menuStrip.Items.Add(toolsMenu);
            menuStrip.Items.Add(pluginsMenu);

            // Add MenuStrip to the form
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Other initializations
            AddToolbar();
            AddStatusBar();
            AddFileWatcher();

            // Event handlers
            richTextBox1.TextChanged += (s, e) => UpdateHtmlPreview();
            richTextBox1.KeyDown += RichTextBox1_KeyDown;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //InitializeCustomComponents();
        }

        private void AutoSave(object? sender, EventArgs e)
        {
            // Auto-save functionality
            string markdownText = richTextBox1.Text;
            File.WriteAllText("autosave.md", markdownText);
        }

        private void SetLightTheme()
        {
            this.BackColor = System.Drawing.Color.White;
            richTextBox1.BackColor = System.Drawing.Color.White;
            richTextBox1.ForeColor = System.Drawing.Color.Black;
            richTextBox2.BackColor = System.Drawing.Color.White;
            richTextBox2.ForeColor = System.Drawing.Color.Black;
        }

        private void SetDarkTheme()
        {
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            richTextBox1.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            richTextBox1.ForeColor = System.Drawing.Color.White;
            richTextBox2.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            richTextBox2.ForeColor = System.Drawing.Color.White;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the Markdown text from richTextBox1
            string markdownText = richTextBox1.Text;

            // Convert the Markdown text to HTML
            string html = Markdown.ToHtml(markdownText);

            // Display the HTML in richTextBox2
            richTextBox2.Text = html;
        }


        private void LoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                richTextBox1.Text = File.ReadAllText(filePath);
                AddToRecentFiles(filePath);
            }
        }

        private void SaveFile(string filePath)
        {
            File.WriteAllText(filePath, richTextBox1.Text);
            AddToRecentFiles(filePath);
        }

        private void AddToolbar()
        {
            ToolStrip toolStrip = new ToolStrip();
            ToolStripButton boldButton = new ToolStripButton("B");
            boldButton.Click += (s, e) => ApplyBold();
            ToolStripButton italicButton = new ToolStripButton("I");
            italicButton.Click += (s, e) => ApplyItalic();
            ToolStripButton headerButton = new ToolStripButton("H1");
            headerButton.Click += (s, e) => ApplyHeader();
            ToolStripButton linkButton = new ToolStripButton("Link");
            linkButton.Click += (s, e) => ApplyLink();

            toolStrip.Items.Add(boldButton);
            toolStrip.Items.Add(italicButton);
            toolStrip.Items.Add(headerButton);
            toolStrip.Items.Add(linkButton);

            this.Controls.Add(toolStrip);
        }

        private void ApplyBold()
        {
            InsertText("**bold**");
        }

        private void ApplyItalic()
        {
            InsertText("*italic*");
        }

        private void ApplyHeader()
        {
            InsertText("# Header");
        }

        private void ApplyLink()
        {
            InsertText("[link](url)");
        }

        private void InsertText(string text)
        {
            int selectionStart = richTextBox1.SelectionStart;
            richTextBox1.Text = richTextBox1.Text.Insert(selectionStart, text);
            richTextBox1.SelectionStart = selectionStart + text.Length;
        }

        private void AddWordCount()
        {
            StatusStrip statusStrip = new StatusStrip();
            ToolStripStatusLabel wordCountLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(wordCountLabel);
            this.Controls.Add(statusStrip);

            richTextBox1.TextChanged += (s, e) =>
            {
                int wordCount = richTextBox1.Text.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                wordCountLabel.Text = $"Word Count: {wordCount}";
            };
        }

        private void AddStatusBar()
        {
            StatusStrip statusStrip = new StatusStrip();
            ToolStripStatusLabel cursorPositionLabel = new ToolStripStatusLabel();
            ToolStripStatusLabel filePathLabel = new ToolStripStatusLabel();
            ToolStripStatusLabel wordCountLabel = new ToolStripStatusLabel();

            statusStrip.Items.Add(cursorPositionLabel);
            statusStrip.Items.Add(filePathLabel);
            statusStrip.Items.Add(wordCountLabel);

            this.Controls.Add(statusStrip);

            richTextBox1.TextChanged += (s, e) =>
            {
                int wordCount = richTextBox1.Text.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                wordCountLabel.Text = $"Word Count: {wordCount}";
            };

            richTextBox1.SelectionChanged += (s, e) =>
            {
                int line = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) + 1;
                int column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexOfCurrentLine() + 1;
                cursorPositionLabel.Text = $"Line: {line}, Column: {column}";
            };
        }

        private void AddFindReplace()
        {
            ToolStripMenuItem findReplaceMenuItem = new ToolStripMenuItem("Find/Replace");
            findReplaceMenuItem.Click += (s, e) => ShowFindReplaceDialog();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(findReplaceMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void ShowFindReplaceDialog()
        {
            Form findReplaceForm = new Form();
            findReplaceForm.Text = "Find and Replace";
            findReplaceForm.Size = new System.Drawing.Size(400, 200);

            Label findLabel = new Label { Text = "Find:", Location = new System.Drawing.Point(10, 20) };
            TextBox findTextBox = new TextBox { Location = new System.Drawing.Point(100, 20), Width = 200 };

            Label replaceLabel = new Label { Text = "Replace:", Location = new System.Drawing.Point(10, 60) };
            TextBox replaceTextBox = new TextBox { Location = new System.Drawing.Point(100, 60), Width = 200 };

            Button findButton = new Button { Text = "Find", Location = new System.Drawing.Point(320, 20) };
            findButton.Click += (s, e) => FindText(findTextBox.Text);

            Button replaceButton = new Button { Text = "Replace", Location = new System.Drawing.Point(320, 60) };
            replaceButton.Click += (s, e) => ReplaceText(findTextBox.Text, replaceTextBox.Text);

            findReplaceForm.Controls.Add(findLabel);
            findReplaceForm.Controls.Add(findTextBox);
            findReplaceForm.Controls.Add(replaceLabel);
            findReplaceForm.Controls.Add(replaceTextBox);
            findReplaceForm.Controls.Add(findButton);
            findReplaceForm.Controls.Add(replaceButton);

            findReplaceForm.ShowDialog();
        }

        private void FindText(string text)
        {
            int index = richTextBox1.Text.IndexOf(text);
            if (index != -1)
            {
                richTextBox1.Select(index, text.Length);
                richTextBox1.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("Text not found.");
            }
        }

        private void ReplaceText(string findText, string replaceText)
        {
            richTextBox1.Text = richTextBox1.Text.Replace(findText, replaceText);
        }


        private void AddRecentFiles()
        {
            ToolStripMenuItem recentFilesMenuItem = new ToolStripMenuItem("Recent Files");

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(recentFilesMenuItem);
            this.Controls.Add(menuStrip);

            // Populate recent files list
            foreach (string file in recentFiles)
            {
                ToolStripMenuItem fileItem = new ToolStripMenuItem(file);
                fileItem.Click += (s, e) => LoadFile(file);
                recentFilesMenuItem.DropDownItems.Add(fileItem);
            }
        }

        private void AddToRecentFiles(string filePath)
        {
            if (!recentFiles.Contains(filePath))
            {
                recentFiles.Add(filePath);
                if (recentFiles.Count > 10)
                {
                    recentFiles.RemoveAt(0);
                }
            }
        }

        private void AddSpellCheck()
        {
            Button spellCheckButton = new Button { Text = "Spell Check", Location = new System.Drawing.Point(10, 50) };
            spellCheckButton.Click += (s, e) => SpellCheck();

            this.Controls.Add(spellCheckButton);
        }

        private void SpellCheck()
        {
            using (Hunspell hunspell = new Hunspell("en_US.aff", "en_US.dic"))
            {
                string[] words = richTextBox1.Text.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words)
                {
                    if (!hunspell.Spell(word))
                    {
                        richTextBox1.SelectionStart = richTextBox1.Text.IndexOf(word);
                        richTextBox1.SelectionLength = word.Length;
                        richTextBox1.SelectionColor = Color.Red;
                    }
                }
            }
        }


        private void AddPrintPreview()
        {
            ToolStripMenuItem printPreviewMenuItem = new ToolStripMenuItem("Print Preview");
            printPreviewMenuItem.Click += (s, e) => ShowPrintPreviewDialog();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(printPreviewMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void ShowPrintPreviewDialog()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);

            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox1.Text, new Font("Arial", 12), Brushes.Black, 100, 100);
        }


        private void AddExportToPdf()
        {
            ToolStripMenuItem exportToPdfMenuItem = new ToolStripMenuItem("Export to PDF");
            exportToPdfMenuItem.Click += (s, e) => ExportToPdf();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(exportToPdfMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void ExportToPdf()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(saveFileDialog.FileName))
                {
                    iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                    iText.Layout.Document document = new iText.Layout.Document(pdf);
                    document.Add(new iText.Layout.Element.Paragraph(richTextBox1.Text));
                    document.Close();
                }
            }
        }



        private void AddImportHtml()
        {
            ToolStripMenuItem importHtmlMenuItem = new ToolStripMenuItem("Import HTML");
            importHtmlMenuItem.Click += (s, e) => ImportHtml();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(importHtmlMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void ImportHtml()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "HTML files (*.html)|*.html";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string htmlContent = File.ReadAllText(openFileDialog.FileName);
                var converter = new Converter();
                string markdownContent = converter.Convert(htmlContent);
                richTextBox1.Text = markdownContent;
            }
        }


        private void AddFullScreenMode()
        {
            ToolStripMenuItem fullScreenMenuItem = new ToolStripMenuItem("Full Screen");
            fullScreenMenuItem.Click += (s, e) => ToggleFullScreen();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(fullScreenMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void ToggleFullScreen()
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void AddSplitView()
        {
            SplitContainer splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Vertical;
            splitContainer.Panel1.Controls.Add(richTextBox1);
            splitContainer.Panel2.Controls.Add(richTextBox2);

            this.Controls.Add(splitContainer);
        }


        private void AddSyntaxHighlighting()
        {
            var editor = new FastColoredTextBox();
            editor.Dock = DockStyle.Fill;
            editor.Language = Language.Custom;
            editor.Text = richTextBox1?.Text ?? string.Empty;

            // Apply custom syntax highlighting for Markdown
            editor.TextChanged += (sender, e) =>
            {
                editor.ClearStylesBuffer();
                editor.Range.ClearStyle(StyleIndex.All);

                // Define styles
                TextStyle headerStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
                TextStyle boldStyle = new TextStyle(null, null, FontStyle.Bold);

                // Apply header style
                var headerRegex = new Regex(@"^#+\s.*$", RegexOptions.Multiline);
                editor.Range.SetStyle(headerStyle, headerRegex);

                // Apply bold style
                var boldRegex = new Regex(@"\*\*(.*?)\*\*");
                editor.Range.SetStyle(boldStyle, boldRegex);
            };

            this.Controls.Clear();
            this.Controls.Add(editor);
        }


        private void AddDragDropImages()
        {
            richTextBox1.AllowDrop = true;
            richTextBox1.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };
            richTextBox1.DragDrop += (s, e) =>
            {
                string[]? strings = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] files = strings;
                foreach (string file in files)
                {
                    if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".gif"))
                    {
                        string markdownImage = $"![Image]({file})";
                        InsertText(markdownImage);
                    }
                }
            };
        }

        private void AddInsertTables()
        {
            ToolStripMenuItem insertTableMenuItem = new ToolStripMenuItem("Insert Table");
            insertTableMenuItem.Click += (s, e) => InsertTable();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(insertTableMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void InsertTable()
        {
            string table = "| Header 1 | Header 2 |\n| --- | --- |\n| Cell 1 | Cell 2 |";
            richTextBox1.SelectedText = table;
        }


        private void AddAutoFormatting()
        {
            richTextBox1.TextChanged += (s, e) => AutoFormatMarkdown();
        }
        private void AutoFormatMarkdown()
        {
            // Basic auto-formatting logic for Markdown
            richTextBox1.TextChanged += (s, e) =>
            {
                int currentSelectionStart = richTextBox1.SelectionStart;
                int currentSelectionLength = richTextBox1.SelectionLength;

                string[] lines = richTextBox1.Text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    // Auto-format headings
                    if (lines[i].StartsWith("#"))
                    {
                        lines[i] = lines[i].TrimEnd() + "\n";
                    }
                    // Auto-format lists
                    else if (lines[i].StartsWith("- "))
                    {
                        lines[i] = lines[i].TrimEnd() + "\n";
                    }
                }

                richTextBox1.Text = string.Join("\n", lines);

                richTextBox1.SelectionStart = currentSelectionStart;
                richTextBox1.SelectionLength = currentSelectionLength;
            };
        }


        private void AddFileWatcher()
        {
            string? executablePath = Application.ExecutablePath;
            string? directoryPath = Path.GetDirectoryName(executablePath);

            // Check if directoryPath is null before using it
            if (directoryPath == null)
            {
                throw new InvalidOperationException("The executable path does not contain directory information.");
            }

            FileSystemWatcher fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = directoryPath;
            fileWatcher.Filter = "*.md";
            fileWatcher.Changed += (s, e) =>
            {
                // Check if e.FullPath is not null or empty before calling ReloadFile
                if (!string.IsNullOrEmpty(e.FullPath))
                {
                    ReloadFile(e.FullPath);
                }
            };
            fileWatcher.EnableRaisingEvents = true;
        }


        private void ReloadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                richTextBox1.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    richTextBox1.Text = File.ReadAllText(filePath);
                });
            }
        }


        private void AddVersionControl()
        {
            ToolStripMenuItem versionControlMenuItem = new ToolStripMenuItem("Version Control");
            versionControlMenuItem.Click += (s, e) => OpenVersionControlDialog();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(versionControlMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void OpenVersionControlDialog()
        {
            Form gitForm = new Form();
            gitForm.Text = "Version Control";
            gitForm.Size = new System.Drawing.Size(400, 300);

            Button commitButton = new Button { Text = "Commit", Location = new System.Drawing.Point(10, 10) };
            commitButton.Click += (s, e) => CommitChanges();

            Button pushButton = new Button { Text = "Push", Location = new System.Drawing.Point(10, 50) };
            pushButton.Click += (s, e) => PushChanges();

            gitForm.Controls.Add(commitButton);
            gitForm.Controls.Add(pushButton);

            gitForm.ShowDialog();
        }

        private void CommitChanges()
        {
            /* using (var repo = new Repository("path/to/your/repo"))
             {
                 Commands.Stage(repo, "*");

                 Signature author = new Signature("Your Name", "you@example.com", DateTime.Now);
                 Signature committer = author;

                 // Commit to the repository
                 repo.Commit("Your commit message", author, committer);
             }*/
        }

        private void PushChanges()
        {
            using (var repo = new Repository("path/to/your/repo"))
            {
                var options = new PushOptions();
                options.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = "your_username",
                    Password = "your_password"
                };

                // Push to the remote repository
                repo.Network.Push(repo.Branches["main"], options);
            }
        }


        private void AddPluginSystem()
        {
            ToolStripMenuItem pluginsMenuItem = new ToolStripMenuItem("Plugins");
            pluginsMenuItem.Click += (s, e) => LoadPlugins();

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Items.Add(pluginsMenuItem);
            this.Controls.Add(menuStrip);
        }

        private void LoadPlugins()
        {
            string pluginsPath = Path.Combine(Application.StartupPath, "Plugins");

            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
            }

            foreach (string file in Directory.GetFiles(pluginsPath, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFrom(file);
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IMarkdownPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        IMarkdownPlugin plugin = (IMarkdownPlugin)Activator.CreateInstance(type);
                        plugin.Run(richTextBox1, richTextBox2);
                    }
                }
            }
        }

        // Define a plugin interface
        public interface IMarkdownPlugin
        {
            void Run(RichTextBox editor, RichTextBox preview);
        }

        // Example plugin implementation
        public class ExamplePlugin : IMarkdownPlugin
        {
            public void Run(RichTextBox editor, RichTextBox preview)
            {
                MessageBox.Show("Example plugin executed!");
            }
        }



        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.B)
            {
                ApplyBold();
            }
            else if (e.Control && e.KeyCode == Keys.I)
            {
                ApplyItalic();
            }
        }

        private void UpdateHtmlPreview()
        {
            string markdownText = richTextBox1.Text;
            string html = Markdown.ToHtml(markdownText);
            richTextBox2.Text = html;
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // Form Load event
        /*private void Form1_Load(object sender, EventArgs e)
        {
            AddToolbar();
            AddWordCount();
            AddStatusBar();
            AddFindReplace();
            AddRecentFiles();
            AddSpellCheck();
            AddPrintPreview();
            AddExportToPdf();
            AddImportHtml();
            AddFullScreenMode();
            AddSplitView();
            AddSyntaxHighlighting();
            AddDragDropImages();
            AddInsertTables();
            AddAutoFormatting();
            AddFileWatcher();
            AddVersionControl();
            AddPluginSystem();
        }*/
    }
}
