using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace TextMindPro
{
    public partial class MainWindow : Window
    {
        private TextEditorCore editorCore;
        private AI.AIAssistant aiAssistant;

        public MainWindow()
        {
            InitializeComponent();
            editorCore = new TextEditorCore(richTextBoxEditor);
            aiAssistant = new AI.AIAssistant(editorCore);

            // Inicializar el motor de IA de forma asíncrona.
            InitializeAIEngine();
        }

        private async void InitializeAIEngine()
        {
            try
            {
                await App.AIEngine.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing AI Engine: {ex.Message}");
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|XAML Files (*.xaml)|*.xaml|DOCX Files (*.docx)|*.docx"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    editorCore.LoadFromFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}");
                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|XAML Files (*.xaml)|*.xaml"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    editorCore.SaveToFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}");
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            editorCore.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            editorCore.Redo();
        }

        private void InsertTable_Click(object sender, RoutedEventArgs e)
        {
            // Inserta una tabla de ejemplo de 3 filas x 3 columnas.
            editorCore.InsertTable(3, 3);
        }

        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
            };
            if (dlg.ShowDialog() == true)
            {
                editorCore.InsertImage(dlg.FileName);
            }
        }

        private async void CheckGrammar_Click(object sender, RoutedEventArgs e)
        {
            string text = editorCore.GetPlainText();
            try
            {
                var result = await aiAssistant.CheckGrammar(text);
                if (result.Suggestions != null && result.Suggestions.Count > 0)
                {
                    string message = "Errores gramaticales encontrados:\n";
                    foreach (var error in result.Suggestions)
                    {
                        message += $"- {error.Description}: {error.ErrorText} -> {error.SuggestedText}\n";
                    }
                    MessageBox.Show(message, "Corrección Gramatical");
                }
                else
                {
                    MessageBox.Show("No se encontraron errores gramaticales.", "Corrección Gramatical");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar gramática: {ex.Message}");
            }
        }

        private async void GenerateSuggestions_Click(object sender, RoutedEventArgs e)
        {
            string text = editorCore.GetPlainText();
            try
            {
                var suggestions = await App.AIEngine.GenerateSuggestions(text);
                string message = "Sugerencias:\n";
                foreach (var suggestion in suggestions)
                {
                    message += $"- {suggestion}\n";
                }
                MessageBox.Show(message, "Sugerencias de IA");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar sugerencias: {ex.Message}");
            }
        }
    }
}
