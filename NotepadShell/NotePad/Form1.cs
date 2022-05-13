using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.TabControl;

/// <summary>
/// Пространство имён NotePad.
/// </summary>
namespace NotePad
{
    /// <summary>
    /// Наша форма.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Коллекция Page.
        /// </summary>
        static TabPageCollection pagesCollection;

        /// <summary>
        /// Номер последней страницы.
        /// </summary>
        static int pageNumber;

        /// <summary>
        /// Кортеж цветов.
        /// </summary>
        static (Color boxBack, Color boxFront, Color menuBack, Color menuFront) colorSet;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            pagesCollection = new TabPageCollection(owner: tabControl1);
            pageNumber = 1;
            colorSet = (Color.White, Color.Black, Color.White, Color.Black);

            menuStrip1.BackColor = colorSet.menuBack;
            menuStrip1.ForeColor = colorSet.menuFront;

            contextMenuStrip1.BackColor = colorSet.menuBack;
            contextMenuStrip1.ForeColor = colorSet.menuFront;

            this.BackColor = Color.White;
        }


        private void NotePad_Load(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void formatToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Создание новой страницы.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tempPage = new TabPage();
                var tempBox = new RichTextBox();

                tempPage.Text = "Page " + pageNumber;
                pageNumber += 1;
                tempBox.Parent = tempPage;
                tempBox.Dock = DockStyle.Fill;

                tempBox.BackColor = colorSet.boxBack;
                tempBox.ForeColor = colorSet.boxFront;

                pagesCollection.Add(tempPage);

                tempPage.Controls[0].TextChanged += SelectedTab_TextChanged;
                tempBox.ContextMenuStrip = contextMenuStrip1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Окно справки для пользователя.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pathToREADME = $"..{Path.DirectorySeparatorChar}" +
                               $"..{Path.DirectorySeparatorChar}" +
                               $"..{Path.DirectorySeparatorChar}.." +
                               $"{Path.DirectorySeparatorChar}README.txt";
            
            string info = string.Empty;
            try
            {
                info = File.ReadAllText(pathToREADME);
            }
            catch (Exception)
            {
                info = "README-файл был удалён или переименован";
            }

            MessageBox.Show(info, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Открыть файл.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsPagesExists())
                {
                    MessageBox.Show(text: "прежде чем открыть файл, создайте для него Page");
                    return;
                }

                var openFileDialog1 = new OpenFileDialog();
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = openFileDialog1.FileName;

                    // RichTextBox текущего Page
                    var tempBox = (RichTextBox)tabControl1.SelectedTab.Controls[0];

                    if (Path.GetExtension(filename) == ".rtf")
                    {
                        tempBox.LoadFile(Path.Combine(filename), RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        // читаем файл в строку
                        string fileText = File.ReadAllText(filename);
                        tempBox.Text = fileText;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Сохранение файла.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsPagesExists())
                    return;

                var saveFileDialog1 = new SaveFileDialog();
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // получаем выбранный файл
                    string filename = saveFileDialog1.FileName;
                    // список обектов текущего TabPage из одного элемента RichTextBox
                    var selectedPage = tabControl1.SelectedTab;

                    // сохраняем текст в файл
                    if (Path.GetExtension(filename) == ".rtf")
                    {
                        var tempBox = (RichTextBox)selectedPage.Controls[0];
                        tempBox.ForeColor = Color.Black;
                        tempBox.SaveFile(filename);
                        tempBox.ForeColor = Color.White;
                    }
                    else
                    {
                        File.WriteAllText(filename, selectedPage.Controls[0].Text);
                    }

                    // убираем звёздочку из заголовка
                    var pageName = tabControl1.SelectedTab.Text;
                    if (pageName[^1] == '*')
                    {
                        pageName = pageName.Remove(pageName.Length - 1);
                        selectedPage.Text = pageName;
                    }

                    MessageBox.Show("Файл сохранен!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Закрытие Page.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void closeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!IsPagesExists())
                    return;

                if (MessageBox.Show(text: $"Вы действительно хотите закрыть {tabControl1.SelectedTab.Text}?",
                    caption: "Закрытие текущей страницы", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                tabControl1.SelectedTab.Controls[0].TextChanged -= SelectedTab_TextChanged;
                pagesCollection.Remove(tabControl1.SelectedTab);

                if (!tabControl1.HasChildren)
                    pageNumber = 1;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Очистить все страницы.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void clearAllPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsPagesExists())
                    return;

                if (MessageBox.Show(text: $"Вы действительно хотите очистить все страницы NotePad?",
                    caption: "Очистка всех страниц", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                foreach (TabPage page in pagesCollection)
                {
                    foreach (RichTextBox textBox in page.Controls)
                    {
                        textBox.Clear();
                    }

                    // удаление звёздочки в названии page
                    var tempText = page.Text;
                    if (tempText[^1] == '*')
                    {
                        tempText = tempText.Remove(tempText.Length - 1);
                        page.Text = tempText;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Закрыть все страницы.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void closeAllPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsPagesExists())
                    return;

                if (MessageBox.Show(text: $"Вы действительно хотите закрыть все страницы NotePad?",
                    caption: "Закрытие всех страниц", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                foreach (TabPage page in pagesCollection)
                    page.Controls[0].TextChanged -= SelectedTab_TextChanged;

                pagesCollection.Clear();
                pageNumber = 1;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Проверка на существование страниц в TabPage1.
        /// </summary>
        /// <returns> булевое значение существования хотя бы одной страницы. </returns>
        private bool IsPagesExists()
        {
            if (!tabControl1.HasChildren)
            {
                MessageBox.Show(text: "в текущем NotePad отсутствуют страницы");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Белая тема.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(text: $"Вы действительно хотите поменять тему NotePad?\nПримечание:" +
                    $" изменение темы приведёт к сбросу цвета текста",
                    caption: "Поменять тему", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                colorSet = (Color.White, Color.Black, Color.White, Color.Black);

                foreach (TabPage page in pagesCollection)
                {
                    page.Controls[0].BackColor = colorSet.boxBack;
                    page.Controls[0].ForeColor = colorSet.boxFront;
                }

                menuStrip1.BackColor = colorSet.menuBack;
                menuStrip1.ForeColor = colorSet.menuFront;

                contextMenuStrip1.BackColor = colorSet.menuBack;
                contextMenuStrip1.ForeColor = colorSet.menuFront;

                ActiveForm.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Тёмная тема.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(text: $"Вы действительно хотите поменять тему NotePad?\nПримечание:" +
                    $" изменение темы приведёт к сбросу цвета текста",
                    caption: "Поменять тему", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                colorSet = (Color.Black, Color.White, Color.Black, Color.White);

                foreach (TabPage page in pagesCollection)
                {
                    page.Controls[0].BackColor = colorSet.boxBack;
                    page.Controls[0].ForeColor = colorSet.boxFront;
                }

                menuStrip1.BackColor = colorSet.menuBack;
                menuStrip1.ForeColor = colorSet.menuFront;

                contextMenuStrip1.BackColor = colorSet.menuBack;
                contextMenuStrip1.ForeColor = colorSet.menuFront;

                ActiveForm.BackColor = Color.Black;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Тема цвета чертополох.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void thistleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(text: $"Вы действительно хотите поменять тему NotePad?\nПримечание:" +
                    $" изменение темы приведёт к сбросу цвета текста",
                    caption: "Поменять тему", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                colorSet = (Color.Thistle, Color.Black, Color.Thistle, Color.Black);

                foreach (TabPage page in pagesCollection)
                {
                    page.Controls[0].BackColor = colorSet.boxBack;
                    page.Controls[0].ForeColor = colorSet.boxFront;
                }

                menuStrip1.BackColor = colorSet.menuBack;
                menuStrip1.ForeColor = colorSet.menuFront;

                contextMenuStrip1.BackColor = colorSet.menuBack;
                contextMenuStrip1.ForeColor = colorSet.menuFront;

                ActiveForm.BackColor = Color.Thistle;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Бисквитная тема.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void bisqueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(text: $"Вы действительно хотите поменять тему NotePad?\nПримечание:" +
                    $" изменение темы приведёт к сбросу цвета текста",
                    caption: "Поменять тему", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                colorSet = (Color.Bisque, Color.Black, Color.Bisque, Color.Black);

                foreach (TabPage page in pagesCollection)
                {
                    page.Controls[0].BackColor = colorSet.boxBack;
                    page.Controls[0].ForeColor = colorSet.boxFront;
                }

                menuStrip1.BackColor = colorSet.menuBack;
                menuStrip1.ForeColor = colorSet.menuFront;

                contextMenuStrip1.BackColor = colorSet.menuBack;
                contextMenuStrip1.ForeColor = colorSet.menuFront;

                ActiveForm.BackColor = Color.Bisque;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Пудрово-синяя тема.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void powderBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(text: $"Вы действительно хотите поменять тему NotePad?\nПримечание:" +
                    $" изменение темы приведёт к сбросу цвета текста",
                    caption: "Поменять тему", buttons: MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                colorSet = (Color.PowderBlue, Color.Black, Color.PowderBlue, Color.Black);

                foreach (TabPage page in pagesCollection)
                {
                    page.Controls[0].BackColor = colorSet.boxBack;
                    page.Controls[0].ForeColor = colorSet.boxFront;
                }

                menuStrip1.BackColor = colorSet.menuBack;
                menuStrip1.ForeColor = colorSet.menuFront;

                contextMenuStrip1.BackColor = colorSet.menuBack;
                contextMenuStrip1.ForeColor = colorSet.menuFront;

                ActiveForm.BackColor = Color.PowderBlue;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        private void helpToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Закрытие формы.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                bool hasUnsavedPage = false;
                foreach (TabPage page in pagesCollection)
                {
                    if (page.Text[^1] == '*')
                    {
                        hasUnsavedPage = true;
                        break;
                    }
                }
                if (!hasUnsavedPage)
                    return;

                var pressedButton = MessageBox.Show(text: $"Сохранить изменённые вкладки NotePad?" +
                    $"\nПримечание: все текущие несохранённые вкладки могут быть утеряны",
                    caption: "Закрытие NotePad", buttons: MessageBoxButtons.YesNoCancel);

                if (pressedButton == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (pressedButton == DialogResult.Yes)
                {
                    foreach (TabPage page in pagesCollection)
                    {
                        if (page.Text[^1] == '*')
                        {
                            var saveFileDialog1 = new SaveFileDialog();
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                // получаем выбранный файл
                                string filename = saveFileDialog1.FileName;
                                // список обектов текущего TabPage из одного элемента RichTextBox
                                var myList = tabControl1.SelectedTab.Controls;
                                // сохраняем текст в файл
                                File.WriteAllText(filename, myList[0].Text);
                            }
                        }
                    }
                    MessageBox.Show("Все текущие файлы сохранены!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Текст изменён.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void SelectedTab_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Text[^1] == '*')
                    return;

                tabControl1.SelectedTab.Text += '*';

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Сохранить все страницы.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void saveAllPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsPagesExists())
                    return;

                foreach (TabPage page in pagesCollection)
                {
                    if (page.Text[^1] == '*')
                    {
                        var saveFileDialog1 = new SaveFileDialog();
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            // получаем выбранный файл
                            string filename = saveFileDialog1.FileName;

                            // список обектов текущего TabPage из одного элемента RichTextBox
                            var myList = page.Controls;


                            // сохраняем текст в файл
                            if (Path.GetExtension(filename) == ".rtf")
                            {
                                var tempBox = (RichTextBox)page.Controls[0];
                                tempBox.ForeColor = Color.Black;
                                tempBox.SaveFile(filename);
                                tempBox.ForeColor = Color.White;

                            }
                            else
                            {
                                File.WriteAllText(filename, page.Controls[0].Text);
                            }

                            // удаление звёздочки в названии page
                            var tempText = page.Text;
                            if (tempText[^1] == '*')
                            {
                                tempText = tempText.Remove(tempText.Length - 1);
                                page.Text = tempText;
                            }
                        }
                    }
                }
                MessageBox.Show("Сохранение завершено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Горячие клавиши.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Alt && e.Shift && e.KeyCode == Keys.D4)
                {
                    Close();
                }
                else if (e.Alt && e.Shift && e.KeyCode == Keys.D5)
                {
                    Form1 form = new();
                    form.Visible = true;
                    form.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Выделить весь текст.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void selectAllTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tempBox = (RichTextBox)tabControl1.SelectedTab.Controls[0];
                if (tempBox.TextLength > 0)
                    tempBox.SelectAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Вырезать фрагмент текста.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tempBox = (RichTextBox)tabControl1.SelectedTab.Controls[0];
                if (tempBox.TextLength > 0)
                    tempBox.Cut();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Копировать фрагмент текста.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tempBox = (RichTextBox)tabControl1.SelectedTab.Controls[0];
                if (tempBox.TextLength > 0)
                    tempBox.Copy();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Вставить фрагмент текста.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tempBox = (RichTextBox)tabControl1.SelectedTab.Controls[0];
                tempBox.Paste();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Установить формат текста.
        /// </summary>
        /// <param name="sender"> Отправитель. </param>
        /// <param name="e"> Аргументы события. </param>
        private void setFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var fontDialog1 = new FontDialog();
                fontDialog1.ShowDialog();
                var tempBox = (RichTextBox)tabControl1.SelectedTab.Controls[0];
                tempBox.SelectionFont = fontDialog1.Font;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}