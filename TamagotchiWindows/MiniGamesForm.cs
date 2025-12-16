using System;
using System.Drawing;
using System.Windows.Forms;
using TamagotchiWindows.Models;

namespace TamagotchiWindows
{
    public partial class MiniGamesForm : Form
    {
        // поля класса
        private VirtualPet pet;
        private Random random;

        // конструктор формы 
        public MiniGamesForm(VirtualPet pet)
        {
            this.pet = pet; // сохраняем ссылку на питомца 
            this.random = new Random();  // Инициализируем генератор случайных чисел


            InitializeComponents();

            this.KeyPreview = true;   // Разрешаем обработку клавиш
            this.KeyDown += (s, e) =>   // Обработчик нажатия клавиш
                                        //this.KeyDown	Событие, которое возникает при нажатии клавиши, когда текущий объект имеет фокус
                                        //s - Параметр sender (отправитель). Объект, который вызвал событие (в данном случае — сам элемент this).
                                        //e - 	Параметр EventArgs (аргументы события). Для события KeyDown это объект типа KeyEventArgs,
                                        //который содержит важную информацию, например, какая именно клавиша была нажата (e.KeyCode), была ли нажата клавиша-модификатор (Shift, Ctrl, Alt) и т.д.
            {
                if (e.KeyCode == Keys.Escape)  // Если нажата клавиша Escape
                    this.Close();
            };
        }

        private void InitializeComponents()
        {
            this.Size = new Size(500, 450);
            this.Text = "🎮 Мини-игры";
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Создаем основной контейнер с тремя строками
            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,   // Заполняем всю форму
                RowCount = 3,            //три строки 
                ColumnCount = 1          // одна колонка 
            };

            // Настройки стилей строк
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 100f)); // Заголовок  фиксированная высота
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // Игры    занимают оставшееся пространство
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60f));  // Кнопка закрытия   фиксированная высота

            // === ЗАГОЛОВОК ===
            Panel headerPanel = new Panel
            {
                BackColor = Color.FromArgb(147, 112, 219),
                Dock = DockStyle.Fill   // Заполняет свою ячейку
            };

            Label titleLabel = new Label
            {
                Text = "🎮 МИНИ-ИГРЫ",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,  // Выравнивание по центру
                Dock = DockStyle.Fill    // Заполняет панель
            };

            // Подзаголовок
            Label subTitleLabel = new Label
            {
                Text = "Выберите игру для вашего питомца",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(240, 240, 240),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,   // Располагается внизу
                Height = 25    // Располагается внизу
            };

            // Добавляем элементы на панель заголовка
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subTitleLabel);
            mainTable.Controls.Add(headerPanel, 0, 0);   // Добавляем в первую строку

            // === ИГРЫ ===
            TableLayoutPanel gamesTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,    // Три строки для трех игр
                ColumnCount = 1,  // Одна колонка
                Padding = new Padding(20, 15, 20, 15)  // Отступы
            };


            // Распределяем высоту равномерно между тремя играми
            gamesTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            //new RowStyle(...): Создает объект, описывающий, как должна себя вести одна строка в таблице.
            //SizeType.Percent: Указывает, что размер строки должен быть задан в процентах
            //от общей доступной высоты таблицы, а не в абсолютных пикселях или автоматически (Auto).
            //33.33f: Указывает точное процентное значение. Эта строка займет 33.33% доступного вертикального пространства.

            gamesTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            gamesTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));


            //// Массив с информацией об играх
            object[] games = new object[]
            {
                new {
                    Text = "🎯 Угадай число",
                    Description = "Угадай число от 1 до 10\n+15 счастья за победу",
                    Handler = new EventHandler(PlayGuessNumberGame),   // Обработчик игры
                    Color = Color.FromArgb(50, 205, 50)
                },
                new {
                    Text = "➗ Математика",
                    Description = "Реши простой пример\n+10 счастья за правильный ответ",
                    Handler = new EventHandler(PlayMathGame),
                    Color = Color.FromArgb(30, 144, 255)
                },
                new {
                    Text = "⚡ Реакция",
                    Description = "Нажми вовремя!\n+20 счастья за быструю реакцию",
                    Handler = new EventHandler(PlayReactionGame),
                    Color = Color.FromArgb(255, 140, 0)
                }
            };


            // Создаем панели для каждой игры
            for (int i = 0; i < games.Length; i++)
            {
                // Используем dynamic для работы с анонимным типом
                //объекты, объявленные как dynamic, могут в течение работы программы менять свой тип.
                dynamic game = games[i];


                // Панель для одной игры
                Panel gamePanel = new Panel
                {
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(15, 10, 15, 10)
                };


                // Кнопка для запуска игры
                Button gameButton = new Button
                {
                    Text = game.Text,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    BackColor = game.Color,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,  // Курсор-рука при наведении
                    Tag = game.Handler,     // Сохраняем обработчик в Tag
                    Dock = DockStyle.Left,
                    Width = 160,
                    Height = 40
                };
                gameButton.FlatAppearance.BorderSize = 0;    // Убираем рамку 
                // Обработчик клика - вызывает сохраненный в Tag обработчик
                gameButton.Click += (s, e) => ((EventHandler)((Button)s).Tag)(s, e);


                // Метка с описанием игры
                Label descLabel = new Label
                {
                    Text = game.Description,  // Описание из массива
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(100, 100, 100),
                    Dock = DockStyle.Fill,
                    Margin = new Padding(170, 8, 0, 0),    // Отступ слева для кнопки
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false    // Отключаем авторазмер
                };
                // Добавляем элементы на панель игры
                gamePanel.Controls.Add(descLabel);
                gamePanel.Controls.Add(gameButton);
                // Добавляем панель игры в таблицу
                gamesTable.Controls.Add(gamePanel, 0, i);
            }

            mainTable.Controls.Add(gamesTable, 1, 0); // Добавляем панель игр во вторую строку

            // === ПАНЕЛЬ С КНОПКОЙ ЗАКРЫТИЯ ===
            Panel closePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent   // Прозрачный фон
            };

            // Кнопка закрытия формы
            Button closeButton = new Button
            {
                Text = "Закрыть (Esc)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(105, 105, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,  // Плоский стиль
                Cursor = Cursors.Hand,   // Курсор-рука
                Width = 130,
                Height = 35
            };
            closeButton.FlatAppearance.BorderSize = 0;   // Убираем рамку
            closeButton.Click += (s, e) => this.Close();  // Обработчик закрытия формы

            // Центрируем кнопку при изменении размера панели
            closeButton.Location = new Point(
                (closePanel.Width - closeButton.Width) / 2,
                (closePanel.Height - closeButton.Height) / 2
            );
            closePanel.SizeChanged += (s, e) =>
            {
                closeButton.Location = new Point(
                    (closePanel.Width - closeButton.Width) / 2,
                    (closePanel.Height - closeButton.Height) / 2
                );
            };

            closePanel.Controls.Add(closeButton);
            mainTable.Controls.Add(closePanel, 2, 0);  // Добавляем в третью строку 

            // Добавляем основной контейнер на форму
            this.Controls.Add(mainTable);
        }




        // === ИГРА "УГАДАЙ ЧИСЛО" ===
        private void PlayGuessNumberGame(object sender, EventArgs e)
        {
            int secretNumber = random.Next(1, 11); // Генерируем случайное число от 1 до 10

            try
            {
                // Используем стандартный диалог ввода
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "Я загадал число от 1 до 10. Попробуй угадать!",
                    "🎯 Угадай число", "", -1, -1);

                if (int.TryParse(input, out int guess))  // Пытаемся преобразовать ввод в число
                {
                    if (guess == secretNumber)// Если угадали
                    {
                        MessageBox.Show("🎉 Правильно! Ты выиграл!\n+15 к счастью", "Победа!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        pet.Happiness = Math.Min(100, pet.Happiness + 15);
                    }
                    else
                    {
                        MessageBox.Show($"❌ Не угадал! Я загадал число: {secretNumber}\n-5 к счастью",
                            "Проигрыш", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        pet.Happiness = Math.Max(0, pet.Happiness - 5);
                    }
                }
            }
            catch// Если возникла ошибка, используем запасной вариант
            {
                PlayGuessNumberGameFallback();
            }
        }



        // Запасной вариант игры "Угадай число" 
        private void PlayGuessNumberGameFallback()
        {
            // Реализация без Microsoft.VisualBasic
            int secretNumber = random.Next(1, 11);
            // Создаем свою форму для ввода
            using (Form form = new Form())
            {
                form.Text = "🎯 Угадай число";
                form.Size = new Size(300, 150);
                form.StartPosition = FormStartPosition.CenterScreen;

                Label label = new Label()// Метка с инструкцией
                {
                    Text = "Я загадал число от 1 до 10. Введите ваш вариант:",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                TextBox textBox = new TextBox() // Поле для ввода числа
                {
                    Location = new Point(20, 50),
                    Size = new Size(100, 20)
                };

                Button okButton = new Button()// Кнопка OK
                {
                    Text = "OK",
                    Location = new Point(130, 50),
                    Size = new Size(60, 25),
                    DialogResult = DialogResult.OK// Устанавливаем результат диалога
                };
                // Добавляем элементы на форму
                form.Controls.AddRange(new Control[] { label, textBox, okButton });
                form.AcceptButton = okButton;// Устанавливаем кнопку по умолчанию

                if (form.ShowDialog() == DialogResult.OK) // Если нажали OK
                {
                    if (int.TryParse(textBox.Text, out int guess))// Пытаемся преобразовать ввод
                    {
                        if (guess == secretNumber)
                        {
                            MessageBox.Show("🎉 Правильно! Ты выиграл!\n+15 к счастью", "Победа!");
                            pet.Happiness = Math.Min(100, pet.Happiness + 15);
                        }
                        else
                        {
                            MessageBox.Show($"❌ Не угадал! Я загадал число: {secretNumber}\n-5 к счастью", "Проигрыш");
                            pet.Happiness = Math.Max(0, pet.Happiness - 5);
                        }
                    }
                }
            }
        }
        // === ИГРА "МАТЕМАТИКА" ===
        private void PlayMathGame(object sender, EventArgs e)
        {
            // Генерируем два случайных числа
            int a = random.Next(1, 10);
            int b = random.Next(1, 10);
            int correctAnswer = a + b; // Вычисляем правильный ответ

            try
            {
                // Используем стандартный диалог ввода
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Сколько будет {a} + {b}?",
                    "➗ Математическая игра", "", -1, -1);

                if (int.TryParse(input, out int answer)) // Пытаемся преобразовать ввод
                {
                    if (answer == correctAnswer) // Если ответ правильный
                    {
                        MessageBox.Show("🎉 Правильно! Ты отлично считаешь!\n+10 к счастью", "Победа!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        pet.Happiness = Math.Min(100, pet.Happiness + 10); // Увеличиваем счастье
                    }
                    else
                    {
                        MessageBox.Show($"❌ Неправильно! Правильный ответ: {correctAnswer}\n-3 к счастью",
                            "Проигрыш", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        pet.Happiness = Math.Max(0, pet.Happiness - 3); // Уменьшаем счастье
                    }
                }
            }
            catch
            {
                PlayMathGameFallback(a, b, correctAnswer); // Запасной вариант
            }
        }

        // Запасной вариант игры "Математика"
        private void PlayMathGameFallback(int a, int b, int correctAnswer)
        {
            using (Form form = new Form())
            {
                form.Text = "➗ Математическая игра";
                form.Size = new Size(300, 150);
                form.StartPosition = FormStartPosition.CenterScreen;

                Label label = new Label() // Метка с вопросом
                {
                    Text = $"Сколько будет {a} + {b}?",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                // Поле для ввода ответа
                TextBox textBox = new TextBox()
                {
                    Location = new Point(20, 50),
                    Size = new Size(100, 20)
                };

                // Кнопка OK
                Button okButton = new Button()
                {
                    Text = "OK",
                    Location = new Point(130, 50),
                    Size = new Size(60, 25),
                    DialogResult = DialogResult.OK
                };

                // Добавляем элементы
                form.Controls.AddRange(new Control[] { label, textBox, okButton });
                form.AcceptButton = okButton;  // Кнопка по умолчанию

                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (int.TryParse(textBox.Text, out int answer))
                    {
                        if (answer == correctAnswer)
                        {
                            MessageBox.Show("🎉 Правильно! Ты отлично считаешь!\n+10 к счастью", "Победа!");
                            pet.Happiness = Math.Min(100, pet.Happiness + 10);
                        }
                        else
                        {
                            MessageBox.Show($"❌ Неправильно! Правильный ответ: {correctAnswer}\n-3 к счастью", "Проигрыш");
                            pet.Happiness = Math.Max(0, pet.Happiness - 3);
                        }
                    }
                }
            }
        }


        // === ИГРА "РЕАКЦИЯ" ===
        private void PlayReactionGame(object sender, EventArgs e)
        {
            // Инструкция к игре
            MessageBox.Show("Нажми OK как можно быстрее, когда увидишь сообщение 'ЖМИ СЕЙЧАС!'",
                "⚡ Игра на реакцию", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Создаем таймер для задержки
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = random.Next(2000, 5000); // Случайная задержка от 2 до 5 секунд

            DateTime startTime = DateTime.Now; // Время начала измерения реакции

            double reactionTime = 0;  // Переменная для хранения времени реакции

            // Обработчик срабатывания таймера
            timer.Tick += (s, args) =>
            {
                timer.Stop();  // Останавливаем таймер
                startTime = DateTime.Now;   // Фиксируем время появления сообщения



                // Показываем сообщение "ЖМИ СЕЙЧАС!"
                if (MessageBox.Show("ЖМИ СЕЙЧАС!", "⚡ ЖМИ!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {

                    // Вычисляем время реакции (в миллисекундах)
                    reactionTime = (DateTime.Now - startTime).TotalMilliseconds;

                    // Оцениваем реакцию и начисляем очки счастья
                    if (reactionTime < 500) // Очень быстрая реакция (< 0.5 сек)
                    {
                        MessageBox.Show($"🎉 Отличная реакция! {reactionTime:0} мс\n+20 к счастью", "Супер!");
                        pet.Happiness = Math.Min(100, pet.Happiness + 20);
                    }
                    else if (reactionTime < 1000) // Хорошая реакция (0.5 - 1 сек)
                    {
                        MessageBox.Show($"😊 Хорошая реакция! {reactionTime:0} мс\n+10 к счастью", "Хорошо!");
                        pet.Happiness = Math.Min(100, pet.Happiness + 10);
                    }
                    else  // Медленная реакция (> 1 сек)
                    {
                        MessageBox.Show($"🐌 Можно лучше! {reactionTime:0} мс\n+5 к счастью", "Неплохо!");
                        pet.Happiness = Math.Min(100, pet.Happiness + 5);
                    }
                }
            };

            timer.Start(); // Запускаем таймер
        }
    }
}