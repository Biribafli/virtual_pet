using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using TamagotchiWindows.Models;
using System.Linq; // Добавляем для работы с LINQ

namespace TamagotchiWindows
{

    // Основной класс формы приложения - главное окно игры
    public partial class MainForm : Form
    {
        // Основные объекты игры
        private VirtualPet pet;  // Питомец - главный объект игры
        private AchievementSystem achievements;  // Система достижений
        private SaveSystem saveSystem;   // Система сохранения/загрузки
        private System.Timers.Timer gameTimer;  // Таймер для обновления состояния игры
        private DateTime lastDayUpdate;  // Время последнего обновления дней

        // Элементы интерфейса
        // Прогресс-бары
        private ProgressBar hungerBar, happinessBar, healthBar, energyBar, hygieneBar, explorationBar;
        private Label statusLabel; // Метка для отображения статуса питомца
        private PictureBox petPicture;  // Картинка питомца
        private Button sleepButton;  // Кнопка сна/пробуждения
        private Dictionary<string, Image> petImages;  // Коллекция изображений питомца
        // Метки с процентами
        private Label hungerPercentLabel, happinessPercentLabel, healthPercentLabel, energyPercentLabel, hygienePercentLabel, explorationPercentLabel;
        // Панели для прогресс-баров
        private Panel hungerPanel, happinessPanel, healthPanel, energyPanel, hygienePanel, explorationPanel;


        //Это коллекция уникальных строк без дубликатов.
        private HashSet<string> shownAchievements = new HashSet<string>();// Показанные достижения
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED - включаем двойную буферизацию
                return cp;
            }
        }
        // Конструктор главной формы
        public MainForm()
        {
            // Включение двойной буферизации для всех элементов управления
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.DoubleBuffer, true);
            this.UpdateStyles();
            // Настройка формы
            this.FormBorderStyle = FormBorderStyle.None;// Убираем рамку окна
            this.WindowState = FormWindowState.Maximized;// Полноэкранный режим
            this.Text = "Виртуальный питомец Тамагочи";
            this.StartPosition = FormStartPosition.CenterScreen;// Центрируем на экране
            this.KeyPreview = true; // Разрешаем обработку клавиш
            this.KeyDown += (s, e) =>  // Обработчик нажатия клавиш (Escape для выхода)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        this.Close(); // Закрытие формы при подтверждении
                }
            };

            InitializeGame();// Инициализация объектов игры
            LoadPetImages(); // Загрузка изображений питомца
            CreateBeautifulLayout();// Создание пользовательского интерфейса
            SetupTimer();// Настройка игрового таймера
            UpdatePetDisplay(); // Первоначальное обновление отображения
        }
        private void SetDefaultBackground()
        {
            this.BackColor = Color.FromArgb(255, 247, 175);
        }

        // Метод создания интерфейса
        private void CreateBeautifulLayout()
        {
            this.Controls.Clear(); // Очищаем все существующие элементы

            // Путь к фоновому изображению - исправлено с "imags" на "images"
            // string imagePath = Path.Combine("images", "background.jpg");
             
            string imagePath = @"C:\Users\Пользователь\Desktop\TamagotchiWindows  норм версия\TamagotchiWindows\Images\фон.jpg";
                
            //string imagePath = Path.Combine(Application.StartupPath, "images", "background.jpg");
            // Градиентный фон как запасной вариант
            // this.BackColor = Color.FromArgb(255, 247, 175);

            if (File.Exists(imagePath))
            {
                try
                {
                    // Создаем фоновое изображение
                    this.BackgroundImage = Image.FromFile(imagePath);
                    this.BackgroundImageLayout = ImageLayout.Stretch; // Растягиваем на всю форму
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить фон: {ex.Message}");
                    // Фон по умолчанию
                    this.BackColor = Color.FromArgb(255, 247, 175);
                }
            }
            else
            {
                // Если файл не найден, используем цветной фон
                this.BackColor = Color.FromArgb(255, 247, 175);
                MessageBox.Show($"Фоновое изображение не найдено!\nИскали по пути: {imagePath}");
            }


            // Верхняя панель с заголовком
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top, // Прикрепляем к верху
                Height = 100,
                BackColor = Color.FromArgb(70, 130, 180)
            };
            // Заголовок игры
            Label titleLabel = new Label
            {
                Text = "🐾 ВИРТУАЛЬНЫЙ ПИТОМЕЦ ТАМАГОЧИ 🐾",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Добавляем элементы на верхнюю панель
            headerPanel.Controls.Add(titleLabel);

            // Основной контент
            TableLayoutPanel contentPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,  // Заполняет оставшееся пространство
                RowCount = 1,  // Одна строка
                ColumnCount = 3, // Три колонки
                Padding = new Padding(20), // Внутренние отступы
                Margin = new Padding(0, 70, 0, 120), // Внешние отступы (отступ от верхней и нижней панели)
                BackColor = Color.Transparent// Прозрачный фон
            };

            // Настройка ширины колонок (30% | 40% | 30%)
            contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));
            contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));

            // Левая колонка - изображение питомца
            Panel leftPanel = CreatePetImagePanel();
            contentPanel.Controls.Add(leftPanel, 0, 0);

            // Центральная колонка - статус
            Panel centerPanel = CreateStatusPanel();
            contentPanel.Controls.Add(centerPanel, 1, 0);

            // Правая колонка - прогресс-бары
            Panel rightPanel = CreateStatsPanel();
            contentPanel.Controls.Add(rightPanel, 2, 0);

            // Нижняя панель с кнопками
            Panel bottomPanel = CreateBottomPanel();
            bottomPanel.Dock = DockStyle.Bottom; // Прикрепляем к низу
            bottomPanel.Height = 120;

            // Добавляем все панели
            this.Controls.Add(headerPanel);  // Верхняя панель
            this.Controls.Add(contentPanel);// Основной контент
            this.Controls.Add(bottomPanel);// Нижняя панель
        }


        // Метод создания панели с изображением питомца
        private Panel CreatePetImagePanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill, // Заполняет свою колонку
                BackColor = Color.Transparent, // Прозрачный фон
                Padding = new Padding(10)// Внутренние отступы
            };

            // Рамка для изображения
            Panel imageFrame = new Panel
            {
                Size = new Size(300, 300),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),// Отступ внутри рамки
                Margin = new Padding(0, 0, 0, 0) // Внешние отступы
            };

            // PictureBox для отображения питомца
            petPicture = new PictureBox
            {
                Size = new Size(280, 280),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill
            };

            imageFrame.Controls.Add(petPicture);// Добавляем изображение в рамку

            // Обработчик изменения размера для центрирования рамки
            panel.SizeChanged += (s, e) =>
            {
                imageFrame.Location = new Point(
                    (panel.Width - imageFrame.Width) / 2, // Центр по горизонтали
                    (panel.Height - imageFrame.Height) / 2 // Центр по вертикали
                );
            };

            panel.Controls.Add(imageFrame); // Добавляем рамку на панель
            return panel;
        }


        // Метод создания панели со статусом питомца
        private Panel CreateStatusPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(10)
            };


            // Контейнер для статуса
            Panel statusBox = new Panel
            {
                Size = new Size(450, 550),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20), // Внутренние отступы
                Margin = new Padding(0, 0, 0, 0)
            };

            // Метка для отображения статуса
            statusLabel = new Label
            {
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(70, 130, 180),
                Text = "Загрузка...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft // Выравнивание по левому краю
            };

            // Заголовок панели статуса
            Label statusTitle = new Label
            {
                Text = "📊 СТАТУС ПИТОМЦА",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(70, 130, 180),
                Height = 40
            };

            statusBox.Controls.Add(statusLabel); // Добавляем метку статуса
            statusBox.Controls.Add(statusTitle);  // Добавляем заголовок

            // Центрирование
            panel.SizeChanged += (s, e) =>
            {
                statusBox.Location = new Point(
                    (panel.Width - statusBox.Width) / 2,
                    (panel.Height - statusBox.Height) / 2
                );
            };

            panel.Controls.Add(statusBox);
            return panel;
        }


        // Метод создания панели с прогресс-барами
        private Panel CreateStatsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(10)
            };

            // Контейнер для статистики
            Panel statsBox = new Panel
            {
                Size = new Size(450, 550), // Увеличиваем высоту для новых баров
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(25, 20, 25, 20),
                Margin = new Padding(0, 0, 0, 0)
            };

            // Заголовок панели статистики
            Panel statsHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(147, 112, 219),
                Padding = new Padding(10, 0, 10, 0)  // Горизонтальные отступы
            };

            Label statsTitle = new Label
            {
                Text = "📈 ПОКАЗАТЕЛИ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter  // Центрирование
            };

            statsHeader.Controls.Add(statsTitle);// Добавляем текст в заголовок
            statsBox.Controls.Add(statsHeader);   // Добавляем заголовок в контейнер

            // Начальные координаты для размещения элементов
            int y = 90; // Начальная позиция Y
            int barWidth = 250; // Ширина прогресс-бара
            int percentLabelX = 120 + barWidth + 10; // Позиция метки с процентом
            int labelWidth = 50;  // Ширина метки с процентом
            int verticalSpacing = 75; // Вертикальное расстояние между строками

            // Голод
            Label hungerLabel = new Label
            {
                Text = "🍖 Голод",
                Location = new Point(10, y),
                Size = new Size(110, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(139, 69, 19)
            };

            hungerPanel = CreateCustomProgressBarPanel(barWidth, 20); // Создаем панель для прогресс-бара
            hungerPanel.Location = new Point(120, y);  // Позиция прогресс-бара

            hungerBar = new ProgressBar
            {
                Size = new Size(barWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.LightGray
            };
            hungerBar.Parent = hungerPanel;// Помещаем прогресс-бар в панель
            hungerBar.Location = new Point(0, 0); // Позиция внутри панели

            hungerPercentLabel = new Label
            {
                Location = new Point(percentLabelX, y),  // Позиция метки с процентом
                Size = new Size(labelWidth, 20),
                Font = new Font("Segoe UI", 10),
                Text = "100%",
                TextAlign = ContentAlignment.MiddleLeft
            };

            y += verticalSpacing; // Смещаем вниз для следующего элемента

            // Счастье
            Label happinessLabel = new Label
            {
                Text = "😊 Счастье",
                Location = new Point(10, y),
                Size = new Size(110, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 140, 0)
            };

            happinessPanel = CreateCustomProgressBarPanel(barWidth, 20);
            happinessPanel.Location = new Point(120, y);

            happinessBar = new ProgressBar
            {
                Size = new Size(barWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.LightGray
            };
            happinessBar.Parent = happinessPanel;
            happinessBar.Location = new Point(0, 0);

            happinessPercentLabel = new Label
            {
                Location = new Point(percentLabelX, y),
                Size = new Size(labelWidth, 20),
                Font = new Font("Segoe UI", 10),
                Text = "100%",
                TextAlign = ContentAlignment.MiddleLeft
            };

            y += verticalSpacing;

            // Здоровье
            Label healthLabel = new Label
            {
                Text = "❤ Здоровье",
                Location = new Point(10, y),
                Size = new Size(110, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 20, 60)
            };

            healthPanel = CreateCustomProgressBarPanel(barWidth, 20);
            healthPanel.Location = new Point(120, y);

            healthBar = new ProgressBar
            {
                Size = new Size(barWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.LightGray
            };
            healthBar.Parent = healthPanel;
            healthBar.Location = new Point(0, 0);

            healthPercentLabel = new Label
            {
                Location = new Point(percentLabelX, y),
                Size = new Size(labelWidth, 20),
                Font = new Font("Segoe UI", 10),
                Text = "100%",
                TextAlign = ContentAlignment.MiddleLeft
            };

            y += verticalSpacing;

            // Энергия
            Label energyLabel = new Label
            {
                Text = "⚡ Энергия",
                Location = new Point(10, y),
                Size = new Size(110, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 144, 255)
            };

            energyPanel = CreateCustomProgressBarPanel(barWidth, 20);
            energyPanel.Location = new Point(120, y);

            energyBar = new ProgressBar
            {
                Size = new Size(barWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.LightGray
            };
            energyBar.Parent = energyPanel;
            energyBar.Location = new Point(0, 0);

            energyPercentLabel = new Label
            {
                Location = new Point(percentLabelX, y),
                Size = new Size(labelWidth, 20),
                Font = new Font("Segoe UI", 10),
                Text = "100%",
                TextAlign = ContentAlignment.MiddleLeft
            };

            y += verticalSpacing;

            // НОВЫЙ: Чистота
            Label hygieneLabel = new Label
            {
                Text = "🛁 Чистота",
                Location = new Point(10, y),
                Size = new Size(110, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 224, 208)
            };

            hygienePanel = CreateCustomProgressBarPanel(barWidth, 20);
            hygienePanel.Location = new Point(120, y);

            hygieneBar = new ProgressBar
            {
                Size = new Size(barWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 80, // Начальное значение
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.LightGray
            };
            hygieneBar.Parent = hygienePanel;
            hygieneBar.Location = new Point(0, 0);

            hygienePercentLabel = new Label
            {
                Location = new Point(percentLabelX, y),
                Size = new Size(labelWidth, 20),
                Font = new Font("Segoe UI", 10),
                Text = "80%",
                TextAlign = ContentAlignment.MiddleLeft
            };

            y += verticalSpacing;

            // НОВЫЙ: Потребность в прогулке
            Label explorationLabel = new Label
            {
                Text = "🌳 Прогулка",
                Location = new Point(10, y),
                Size = new Size(110, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34)
            };

            explorationPanel = CreateCustomProgressBarPanel(barWidth, 20);
            explorationPanel.Location = new Point(120, y);

            explorationBar = new ProgressBar
            {
                Size = new Size(barWidth, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 50, // Начальное значение
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.LightGray
            };
            explorationBar.Parent = explorationPanel;
            explorationBar.Location = new Point(0, 0);

            explorationPercentLabel = new Label
            {
                Location = new Point(percentLabelX, y),
                Size = new Size(labelWidth, 20),
                Font = new Font("Segoe UI", 10),
                Text = "50%",
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Добавляем все элементы в контейнер статистики
            statsBox.Controls.AddRange(new Control[]
            {
                hungerLabel, hungerPanel, hungerPercentLabel,
                happinessLabel, happinessPanel, happinessPercentLabel,
                healthLabel, healthPanel, healthPercentLabel,
                energyLabel, energyPanel, energyPercentLabel,
                hygieneLabel, hygienePanel, hygienePercentLabel, // Новые
                explorationLabel, explorationPanel, explorationPercentLabel // Новые
            });

            // Центрирование
            panel.SizeChanged += (s, e) =>
            {
                statsBox.Location = new Point(
                    (panel.Width - statsBox.Width) / 2,
                    (panel.Height - statsBox.Height) / 2
                );
            };

            panel.Controls.Add(statsBox);
            return panel;
        }

        // Метод создания кастомной панели для прогресс-бара (с градиентным фоном)
        private Panel CreateCustomProgressBarPanel(int width, int height)
        {
            Panel panel = new Panel
            {
                Size = new Size(width, height),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(1) // Внутренний отступ
            };

            // Рисуем градиентную полосу внутри
            panel.Paint += (s, e) =>
            {
                Graphics graphics = e.Graphics;
                Rectangle rect = new Rectangle(0, 0, width, height);
                // Создаем градиентную кисть (от прозрачного белого к прозрачному серому)
                LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(100, 255, 255, 255), Color.FromArgb(100, 200, 200, 200), 0f);
                graphics.FillRectangle(brush, rect);// Закрашиваем прямоугольник
                brush.Dispose();// Освобождаем ресурсы
            };

            return panel;
        }


        // Метод создания нижней панели с кнопками действий
        private Panel CreateBottomPanel()
        {
            Panel panel = new Panel
            {
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Панель с потоковым расположением для кнопок
            FlowLayoutPanel actionsPanel = new FlowLayoutPanel
            {
                // Горизонтальное расположение
                WrapContents = false,                      // Не переносить на новую строку
                Dock = DockStyle.Fill,                     // Заполняет панель
                Padding = new Padding(40, 20, 40, 20)      // Отступы
            };

            // Массив информации о кнопках
            object[] buttons = new[]
            {
                new { Text = "🍎 Покормить", Handler = new EventHandler(FeedPet), Color = Color.FromArgb(50, 205, 50) },
                new { Text = "🎮 Играть", Handler = new EventHandler(PlayWithPet), Color = Color.FromArgb(255, 140, 0) },
                new { Text = "🛁 Купать", Handler = new EventHandler(BathPet), Color = Color.FromArgb(64, 224, 208) }, // НОВАЯ
                new { Text = "🌳 Гулять", Handler = new EventHandler(WalkPet), Color = Color.FromArgb(34, 139, 34) }, // НОВАЯ
                new { Text = "💤 Спать", Handler = new EventHandler(PutToSleep), Color = Color.FromArgb(30, 144, 255) },
                new { Text = "💊 Лечить", Handler = new EventHandler(HealPet), Color = Color.FromArgb(220, 20, 60) },
                new { Text = "🎯 Мини-игры", Handler = new EventHandler(OpenGames), Color = Color.FromArgb(147, 112, 219) },
                new { Text = "🏆 Достижения", Handler = new EventHandler(OpenAchievements), Color = Color.FromArgb(255, 215, 0) },
                new { Text = "💾 Сохранить", Handler = new EventHandler(SaveGame), Color = Color.FromArgb(105, 105, 105) },
                new { Text = "📂 Загрузить", Handler = new EventHandler(LoadGame), Color = Color.FromArgb(70, 130, 180) }
            };
            // Создаем и добавляем кнопки
            foreach (object btnInfo in buttons)
            {
                Button button = CreateStyledButton(((dynamic)btnInfo).Text, ((dynamic)btnInfo).Color);
                button.Click += ((dynamic)btnInfo).Handler; // Добавляем обработчик 

                if (((dynamic)btnInfo).Text == "💤 Спать")
                {
                    sleepButton = button; // Сохраняем ссылку на кнопку сна 
                }

                actionsPanel.Controls.Add(button); // Добавляем кнопку на панель
            }

            panel.Controls.Add(actionsPanel);
            return panel;
        }

        // Метод купания питомца
        private void BathPet(object sender, EventArgs e)
        {
            if (!pet.IsSleeping) // Проверяем, не спит ли питомец
            {
                if (pet.Energy < 20)
                {
                    MessageBox.Show("Питомец слишком устал для купания!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                pet.TakeBath(); // Вызываем метод купания
                MessageBox.Show($"Вы искупали {pet.Name}!\n+40 к чистоте, +10 к счастью",
                    "Купание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdatePetDisplay();// Обновляем отображение
                CheckAndShowAchievements(); // Проверяем достижения
            }
            else
            {
                MessageBox.Show("Питомец спит! Разбудите его сначала.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Метод прогулки с питомцем
        private void WalkPet(object sender, EventArgs e)
        {
            if (!pet.IsSleeping)
            {
                if (pet.Energy < 30)
                {
                    MessageBox.Show("Питомец слишком устал для прогулки!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Показываем текущую потребность в прогулке
                string walkStatus = pet.GetWalkStatus();

                pet.GoForWalk(); // Вызываем метод прогулки

                MessageBox.Show($"Вы погуляли с {pet.Name}!\n" +
                               $"+50 к потребности в прогулке (сейчас: {pet.Exploration}%)\n" +
                               $"+25 к счастью\n" +
                               $"-25 к энергии\n" +
                               $"\nСтатус: {walkStatus}",
                    "Прогулка", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdatePetDisplay();
                CheckAndShowAchievements();
            }
            else
            {
                MessageBox.Show("Питомец спит! Разбудите его сначала.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Метод создания стилизованной кнопки
        private Button CreateStyledButton(string text, Color baseColor)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(175, 60),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(5), // Внешние отступы
                // Добавляем тень
                BackColor = baseColor
            };

            button.FlatAppearance.BorderSize = 0; // Убираем рамку
            button.MouseEnter += (s, e) => // Эффект при наведении курсора
            {
                button.BackColor = ControlPaint.Light(baseColor, 0.2f); // Осветляем цвет
                button.ForeColor = Color.Black; // Меняем цвет текста
            };
            // Эффект при уходе курсора
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = baseColor;// Возвращаем исходный цвет
                button.ForeColor = Color.White; // Возвращаем белый текст
            };

            return button;
        }

        // Метод загрузки изображений питомца
        private void LoadPetImages()
        {
            petImages = new Dictionary<string, Image>();

            try
            {
                string[] imageFiles = {
                    "happy_pet.png", "normal_pet.png", "sad_pet.png",
                    "sleeping_pet.png", "dead_pet.png"
                };

                bool imagesFound = false;
                // Загружаем каждое изображение из папки Images
                foreach (string fileName in imageFiles)
                {
                    // Path.Combine("Images", fileName): Этот метод объединяет имя папки "Images" и имя файла(fileName)
                    // в одну строку, формируя правильный путь к файлу с учетом особенностей операционной системы
                    string fullPath = Path.Combine("Images", fileName);

                    //File.Exists(fullPath): Это условие проверяет, существует ли физически файл по пути,
                    //указанному в переменной fullPath
                    if (File.Exists(fullPath))
                    {
                        //Image.FromFile(fullPath): Этот метод загружает изображение
                        //из указанного файла в память и создает объект типа Image.
                        petImages[fileName] = Image.FromFile(fullPath);
                        imagesFound = true;
                    }
                }
                // Если ни одно изображение не найдено, выбрасываем исключение
                if (!imagesFound)
                {
                    throw new FileNotFoundException("Файлы изображений не найдены");
                }
            }

            // В случае ошибки показываем предупреждение и создаем временные изображения
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображений: {ex.Message}\nБудут использованы стандартные изображения.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CreateFallbackImages(); // Создаем временные изображения
            }
        }

        // Метод создания временных изображений при отсутствии файлов
        private void CreateFallbackImages()
        {
            petImages["happy_pet.png"] = CreatePetImage(Color.LightGreen, "😊", "СЧАСТЛИВЫЙ");
            petImages["normal_pet.png"] = CreatePetImage(Color.LightYellow, "😐", "НОРМАЛЬНЫЙ");
            petImages["sad_pet.png"] = CreatePetImage(Color.LightPink, "😞", "ГРУСТНЫЙ");
            petImages["sleeping_pet.png"] = CreatePetImage(Color.LightBlue, "💤", "СПИТ");
            petImages["dead_pet.png"] = CreatePetImage(Color.Gray, "💀", "УМЕР");
        }


        // Метод создания простого изображения питомца
        private Image CreatePetImage(Color bgColor, string emoji, string moodText)
        {
            int width = 200;
            int height = 200;

            // Создание экземпляра класса Bitmap: Она создает новый объект в памяти
            Bitmap bitmap = new Bitmap(width, height);
            //ключевой объект, который представляет собой поверхность для рисования
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Настройки сглаживания для лучшего качества
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                // Создаем градиентный фон
                LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0),
                    new Point(width, height),
                    Color.White,
                    bgColor);
                //Нарисовать и закрасить прямоугольник
                graphics.FillRectangle(brush, 0, 0, width, height);
                // Рисуем эмодзи в центре
                Font emojiFont = new Font("Segoe UI Emoji", 48, FontStyle.Bold);
                StringFormat emojiFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString(emoji, emojiFont, Brushes.Black,
                    new RectangleF(0, 0, width, height - 30), emojiFormat);


                // Рисуем текст состояния под эмодзи
                Font textFont = new Font("Arial", 10, FontStyle.Bold);
                SolidBrush textBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0));// Полупрозрачный черный
                graphics.DrawString(moodText, textFont, textBrush,
                    new RectangleF(0, height - 25, width, 20), emojiFormat);


                // Рисуем рамку вокруг изображения
                graphics.DrawRectangle(new Pen(Color.DarkGray, 3), 1, 1, width - 3, height - 3);

                brush.Dispose();// освобождаем место
            }
            return bitmap;
        }


        // Метод инициализации игровых объектов
        private void InitializeGame()
        {
            saveSystem = new SaveSystem();
            lastDayUpdate = DateTime.Now; //Запомнить текущее время.

            DialogResult result = MessageBox.Show("Хотите загрузить сохраненную игру?", "Тамагочи",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Используем деконструкцию кортежа
                (VirtualPet loadedPet, AchievementSystem loadedAchievements) = saveSystem.LoadGame();

                if (loadedPet != null)
                {
                    pet = loadedPet;
                    achievements = loadedAchievements;
                    UpdatePetDisplay();
                    return;
                }
            }
            // Создаем нового питомца
            string name = "Тамагочи";
            try
            {
                name = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите имя вашего питомца:", "Новый питомец", "Тамагочи", -1, -1);

                if (string.IsNullOrEmpty(name))
                    name = "Тамагочи";
            }
            catch
            {
                name = "Тамагочи";
            }

            pet = new VirtualPet(name);
            achievements = new AchievementSystem();
            UpdatePetDisplay();
        }

        // Метод настройки таймера для периодического обновления состояния игры
        private void SetupTimer()
        {
            gameTimer = new System.Timers.Timer(30000); // 30 секунд
            gameTimer.Elapsed += OnTimerElapsed;
            gameTimer.Start();
        }

        // Метод проверки и отображения достижений
        private void CheckAndShowAchievements()
        {
            if (pet != null && achievements != null)
            {
                List<string> unlocked = achievements.CheckAchievements(pet);

                // Словарь для перевода 
                Dictionary<string, string> achievementNamesRu = new Dictionary<string, string>
                {
                    { "First Meal", "Первая еда" },
                    { "Playful Pet", "Игривый питомец" },
                    { "Good Caretaker", "Хороший хозяин" },
                    { "Survivor", "Выживший" },
                    { "Master", "Мастер Тамагочи" },
                    { "Clean Pet", "Чистюля" },
                    { "Explorer", "Исследователь" },
                    { "Personality Master", "Мастер характера" }
                };

                // для перебора элементов коллекции(списка, массива, словаря).
                // Показываем каждое новое достижение
                foreach (string achievementKey in unlocked)
                {
                    string achievementName = achievementNamesRu.ContainsKey(achievementKey)
                        ? achievementNamesRu[achievementKey]
                        : achievementKey;

                    MessageBox.Show($"🎉 Достижение разблокировано: {achievementName}",
                        "Поздравляем!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Обработчик события таймера - вызывается каждые 30 секунд
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {

            // Проверяем, нужно ли вызывать метод в основном потоке (потоке UI)
            //проверяет, вызван ли код из потока, отличного от того,
            //в котором был создан элемент управления 
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnTimerElapsed(sender, e)));
                return;
            }

            if (pet != null)
            {
                pet.UpdateStatus();  // обновляем статус
                // Обновляем возраст питомца каждую минуту
                if ((DateTime.Now - lastDayUpdate).TotalMinutes >= 1)
                {
                    pet.Age++;
                    pet.DaysSurvived++;
                    lastDayUpdate = DateTime.Now;
                }

                UpdatePetDisplay(); // Обновляем отображение

                List<string> unlocked = achievements.CheckAchievements(pet);
                foreach (string achievement in unlocked)
                {
                    MessageBox.Show($"🎉 Достижение разблокировано: {achievement}",
                        "Поздравляем!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                // Проверяем, жив ли питомец
                if (!pet.IsAlive())
                {
                    gameTimer.Stop();
                    MessageBox.Show($"💔 Ваш питомец умер...\nОн прожил {pet.DaysSurvived} дней",
                        "Конец игры", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdatePetDisplay();
                    this.Close();
                }
            }
        }
        // Метод обновления отображения состояния питомца на форме
        private void UpdatePetDisplay()
        {
            if (pet == null)
                return;

            // Обновляем существующие процентные метки(текст)
            if (hungerPercentLabel != null)
                hungerPercentLabel.Text = $"{Math.Max(0, Math.Min(100, pet.Hunger))}%";
            if (happinessPercentLabel != null)
                happinessPercentLabel.Text = $"{Math.Max(0, Math.Min(100, pet.Happiness))}%";
            if (healthPercentLabel != null)
                healthPercentLabel.Text = $"{Math.Max(0, Math.Min(100, pet.Health))}%";
            if (energyPercentLabel != null)
                energyPercentLabel.Text = $"{Math.Max(0, Math.Min(100, pet.Energy))}%";

            // Обновляем метки чистоты и прогулок
            if (hygienePercentLabel != null)
                hygienePercentLabel.Text = $"{Math.Max(0, Math.Min(100, pet.Hygiene))}%";
            if (explorationPercentLabel != null)
                explorationPercentLabel.Text = $"{Math.Max(0, Math.Min(100, pet.Exploration))}%";

            // Обновление прогресс-баров
            if (hungerBar != null)
                hungerBar.Value = Math.Max(0, Math.Min(100, pet.Hunger));
            if (happinessBar != null)
                happinessBar.Value = Math.Max(0, Math.Min(100, pet.Happiness));
            if (healthBar != null)
                healthBar.Value = Math.Max(0, Math.Min(100, pet.Health));
            if (energyBar != null)
                energyBar.Value = Math.Max(0, Math.Min(100, pet.Energy));
            if (hygieneBar != null)
                hygieneBar.Value = Math.Max(0, Math.Min(100, pet.Hygiene));
            if (explorationBar != null)
                explorationBar.Value = Math.Max(0, Math.Min(100, pet.Exploration));

            if (sleepButton != null)
                sleepButton.Text = pet.IsSleeping ? "👀 Разбудить" : "💤 Спать";
            // Обновление статуса с информацией о прогулке
            if (statusLabel != null)
            {
                string walkStatus = pet.NeedsWalk() ? "🔴 Нужна прогулка!" : "🟢 Прогулка не нужна";

                statusLabel.Text = $"✨ Имя: {pet.Name}\n\n" +
                                  $"📅 Возраст: {pet.Age} дней\n" +
                                  $"⏳ Прожито: {pet.DaysSurvived} дней\n\n" +
                                  $"🎭 Характер: {pet.CurrentPersonality}\n" +
                                  $"😊 Настроение: {pet.GetMood()}\n" +
                                  $"❤ Здоровье: {pet.GetHealthStatus()}\n" +
                                  $"🚶 Прогулка: {pet.GetWalkStatus()}\n" +
                                  $"{(pet.IsSleeping ? "💤 Спит" : "👀 Бодрствует")}\n\n" +
                                  $"🍽️ Съедено корма: {pet.MealsEaten}\n" +
                                  $"🎮 Сыграно игр: {pet.GamesPlayed}\n" +
                                  $"🛁 Принято ванн: {pet.BathsTaken}\n" +
                                  $"🌳 Прогулок: {pet.WalksTaken}";
            }

            UpdatePetImage();
        }


        // Метод обновления изображения питомца в зависимости от его состояния
        private void UpdatePetImage()
        {
            try
            {
                //Если у меня НЕТ объекта питомца (pet == null)
                //ИЛИ у меня НЕТ элемента на экране для его отображения (petPicture == null),
                //то закончи 
                if (pet == null || petPicture == null) return;
                // Получаем путь к изображению на основе состояния питомца
                string imageKey = pet.GetImagePath();

                if (petImages.ContainsKey(imageKey))
                {
                    petPicture.Image = petImages[imageKey];
                    petPicture.BackColor = Color.Transparent;
                }
                else
                {
                    SetFallbackBackground(); // цветной фон
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления изображения: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetFallbackBackground();
            }
        }
        // Метод установки цветного фона при отсутствии изображений
        private void SetFallbackBackground()
        {
            if (petPicture == null) return;

            if (!pet.IsAlive())
            {
                petPicture.BackColor = Color.Gray;
            }
            else if (pet.IsSleeping)
            {
                petPicture.BackColor = Color.LightBlue;
            }
            else if (pet.Happiness >= 70)
            {
                petPicture.BackColor = Color.LightGreen;
            }
            else if (pet.Happiness >= 40)
            {
                petPicture.BackColor = Color.LightYellow;
            }
            else
            {
                petPicture.BackColor = Color.LightPink;
            }

            petPicture.Image = null;// Убираем изображение
        }


        // Метод кормления питомца (обработчик кнопки "Покормить")
        private void FeedPet(object sender, EventArgs e)
        {
            if (!pet.IsSleeping)
            {
                pet.Feed();
                UpdatePetDisplay();
                CheckAndShowAchievements(); // проверка достижения
            }
            else
            {
                MessageBox.Show("Питомец спит и не может есть!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Метод игры с питомцем (обработчик кнопки "Играть")
        private void PlayWithPet(object sender, EventArgs e)
        {
            if (!pet.IsSleeping && pet.Energy > 20)
            {
                pet.Play();
                UpdatePetDisplay();
                CheckAndShowAchievements(); //  проверка
            }
            else if (pet.IsSleeping)
            {
                MessageBox.Show("Питомец спит! Разбудите его сначала.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Питомец слишком устал!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Метод укладывания/пробуждения питомца (обработчик кнопки "Спать")
        private void PutToSleep(object sender, EventArgs e)
        {
            if (!pet.IsSleeping)
            {
                pet.Sleep();
                MessageBox.Show("Питомец засыпает... Zzz", "Сон",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                pet.WakeUp();
                MessageBox.Show("Питомец просыпается!", "Пробуждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdatePetDisplay();
            CheckAndShowAchievements(); // Проверка после сна/пробуждения
        }


        // Метод лечения питомца (обработчик кнопки "Лечить")
        private void HealPet(object sender, EventArgs e)
        {
            if (!pet.IsSleeping)
            {
                pet.Heal();
                UpdatePetDisplay();
                CheckAndShowAchievements(); // Немедленная проверка
            }
            else
            {
                MessageBox.Show("Питомец спит и не может лечиться!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Метод открытия формы мини-игр (обработчик кнопки "Мини-игры")
        private void OpenGames(object sender, EventArgs e)
        {
            if (!pet.IsSleeping)
            {
                MiniGamesForm gamesForm = new MiniGamesForm(pet);
                gamesForm.ShowDialog();// Модальное окно мини-игр
                UpdatePetDisplay();
                CheckAndShowAchievements(); // Проверка после мини-игр
            }
            else
            {
                MessageBox.Show("Питомец спит! Разбудите его сначала.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Метод открытия формы достижений (обработчик кнопки "Достижения")
        private void OpenAchievements(object sender, EventArgs e)
        {

            // Вывод количества достижений
            MessageBox.Show($"Количество достижений: {achievements.Achievements.Count}", "Отлаживать");

            AchievementsForm achievementsForm = new AchievementsForm(achievements);
            achievementsForm.ShowDialog();// Модальное окно достижений
        }

        // Метод сохранения игры (обработчик кнопки "Сохранить")
        private void SaveGame(object sender, EventArgs e)
        {
            // Если получилось сохранить игру,
            //То показать окошко с сообщением "Игра успешно сохранена!"
            if (saveSystem.SaveGame(pet, achievements))
            {
                MessageBox.Show("Игра успешно сохранена!", "Сохранение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // Метод загрузки игры (обработчик кнопки "Загрузить")
        private void LoadGame(object sender, EventArgs e)
        {
            // смотрим, есть ли на компьютере сохранение
            (VirtualPet loadedPet, AchievementSystem loadedAchievements) = saveSystem.LoadGame();
            if (loadedPet != null)
            {
                pet = loadedPet;
                achievements = loadedAchievements;
                UpdatePetDisplay();
                MessageBox.Show("Игра загружена!", "Загрузка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // Обработчик закрытия формы - освобождение ресурсов
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameTimer?.Stop(); // Останавливаем таймер
            gameTimer?.Dispose(); // Освобождаем ресурсы таймера

            if (petImages != null) // Освобождаем ресурсы изображений
            {
                foreach (Image image in petImages.Values)
                {
                    image?.Dispose(); //Для каждой картинки: "Освободи память!"
                    //если картинка существует, то освободи
                }
                petImages.Clear();//Очищаем список картинок
            }

            base.OnFormClosing(e);
        }
    }
}