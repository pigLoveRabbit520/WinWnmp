using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using SalamanderWnmp.Tool;
using SalamanderWnmp.UserClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using System.Windows.Controls;
using SalamanderWnmp.EditorClass;
using System.Xml;
using System.Windows.Threading;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// CodePanel.xaml 的交互逻辑
    /// </summary>
    public partial class CodePanelWin : Window
    {


        /// <summary>
        /// 执行代码状态
        /// </summary>
        private enum RunCodeStatus
        {
            Running,
            Stop
        }

        private enum ResizeDirection
        {
            BottomRight = 8,
        }

        private Dictionary<ResizeDirection, Cursor> cursors = new Dictionary<ResizeDirection, Cursor>
        {
            {ResizeDirection.BottomRight, Cursors.SizeNWSE},
        };
       

        public CodePanelWin()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(CodePanelWin).Assembly.GetManifestResourceStream("SalamanderWnmp.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);


            InitializeComponent();


            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();


        }


        public event PropertyChangedEventHandler PropertyChanged;
        // 选择的编程语言
        private CodeHelper.ProgramLan selectedLan = CodeHelper.ProgramLan.JavaScript;


        public CodeHelper.ProgramLan SelectedLan
        {
            get
            {
                return this.selectedLan;
            }
            set
            {
                this.selectedLan = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedLan"));
                }
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            string code = this.textEditor.Text;
            if(string.IsNullOrEmpty(code))
            {
                MessageBox.Show("请输入代码");
                return;
            }
            CodeHelper helper = new CodeHelper();
            helper.SetPreWork(() =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action<RunCodeStatus>(SetRunButtonStatus),
                    RunCodeStatus.Running);
            }).SetAfterWork((result) =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action<String>((txt) => { this.txtOutput.Text = txt; }), result);
                DispatcherHelper.UIDispatcher.Invoke(new Action<RunCodeStatus>(SetRunButtonStatus),
                    RunCodeStatus.Stop);
            }).Run(code, SelectedLan);
        }


        /// <summary>
        /// 更改运行按钮的Content
        /// </summary>
        /// <param name="status"></param>
        private void SetRunButtonStatus(RunCodeStatus status)
        {
            switch(status)
            {
                case RunCodeStatus.Running:
                    this.btnRun.IsEnabled = false;
                    break;
                case RunCodeStatus.Stop:
                    this.btnRun.IsEnabled = true;
                    break;
            }
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.lanList.DataContext = this;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtOutput.Text = "";            
        }


        void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                string currentFileName = dlg.FileName;
                textEditor.Load(currentFileName);
                textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }

        void saveFileClick(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".txt";
            if (dlg.ShowDialog() ?? false)
            {
                textEditor.Save(dlg.FileName);
            }
            else
            {
                return;
            }
        }

        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(textEditor.TextArea);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                data.Add(new MyCompletionData("Another item"));
                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #region Folding
        FoldingManager foldingManager;
        object foldingStrategy;

        void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                UpdateFoldings();
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        void UpdateFoldings()
        {
            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
        }

        private void AddFontSize_Click(object sender, RoutedEventArgs e)
        {
            this.textEditor.FontSize++;
        }

        private void MinusFontSize_Click(object sender, RoutedEventArgs e)
        {
            if (this.textEditor.FontSize <= 4)
                return;
            this.textEditor.FontSize--;
        }
    }
    #endregion

}
