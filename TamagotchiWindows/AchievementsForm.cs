using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TamagotchiWindows.Models;

namespace TamagotchiWindows
{
    //Форма для отображения достижений игрока.
    // Показывает список всех достижений с их статусом (разблокировано/заблокировано).
    // Имеет интуитивно понятный интерфейс с визуальными индикаторами состояния.
    public partial class AchievementsForm : Form
    {
        // Ссылка на систему достижений, переданная из главной формы
        private AchievementSystem achievements;

        // Конструктор формы достижений.
        //Объект системы достижений для отображения
        public AchievementsForm(AchievementSystem achievements)
        {
            this.achievements = achievements;  // Сохраняем переданную систему достижений
            InitializeComponents();            // Инициализируем элементы управления
            this.KeyPreview = true;           // Разрешаем форму обрабатывать нажатия клавиш

            // Обработчик нажатия клавиши Escape для закрытия формы
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    this.Close();  // Закрываем форму при нажатии Escape
            };
        }

        // Метод динамического создания всех элементов управления формы.
        // Создает интерфейс "на лету" без использования дизайнера.
        private void InitializeComponents()
        {
            // ============= НАСТРОЙКА ОСНОВНЫХ СВОЙСТВ ФОРМЫ =============

            this.Size = new Size(550, 650);                      // Фиксированный размер формы
            this.Text = "🏆 Достижения";                         // Заголовок окна с эмодзи
            this.StartPosition = FormStartPosition.CenterParent; // Центрирование относительно родительской формы
            this.BackColor = Color.FromArgb(250, 250, 255);      // Светло-синий фон формы
            this.FormBorderStyle = FormBorderStyle.FixedDialog;  // Фиксированные границы (нельзя менять размер)
            this.MaximizeBox = false;                            // Отключаем кнопку развертывания
            this.MinimizeBox = false;                            // Отключаем кнопку сворачивания

            // ============= СОЗДАНИЕ ОСНОВНОЙ СТРУКТУРЫ =============

            // Табличная панель для организации макета в 3 строки
            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,    // Заполняет всю форму
                RowCount = 3,             // 3 строки: заголовок, список, кнопка
                ColumnCount = 1           // 1 колонка
            };

            // Настройка высоты строк:
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 80f));   // Строка 0: заголовок (80px)
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // Строка 1: список (все оставшееся место)
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60f));   // Строка 2: кнопка закрытия (60px)

            // ============= ЗАГОЛОВОК ФОРМЫ =============

            // Панель для заголовка (верхняя часть формы)
            Panel headerPanel = new Panel
            {
                BackColor = Color.FromArgb(70, 130, 180),  // Темно-синий цвет фона
                Dock = DockStyle.Fill                      // Заполняет свою ячейку таблицы
            };

            // Текст заголовка
            Label titleLabel = new Label
            {
                Text = "🏆 ВАШИ ДОСТИЖЕНИЯ",              // Текст с эмодзи трофея
                Font = new Font("Segoe UI", 16, FontStyle.Bold),  // Крупный жирный шрифт
                ForeColor = Color.White,                  // Белый текст на синем фоне
                TextAlign = ContentAlignment.MiddleCenter, // Выравнивание по центру
                Dock = DockStyle.Fill                     // Заполняет всю панель заголовка
            };

            headerPanel.Controls.Add(titleLabel);         // Добавляем текст на панель заголовка
            mainTable.Controls.Add(headerPanel, 0, 0);    // Помещаем панель в первую строку таблицы

            // ============= ОБЛАСТЬ С ПРОКРУТКОЙ ДЛЯ СПИСКА ДОСТИЖЕНИЙ =============

            // Панель с прокруткой для длинного списка достижений
            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,                    // Заполняет свою ячейку таблицы
                AutoScroll = true,                       // Включаем автоматическую прокрутку
                Padding = new Padding(20, 10, 20, 10)    // Внутренние отступы: слева/справа 20px, сверху/снизу 10px
            };

            // Панель с потоковым расположением для карточек достижений
            FlowLayoutPanel achievementsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,    // Элементы располагаются сверху вниз
                WrapContents = false,                    // Не переносить элементы на новую строку
                AutoSize = true,                        // Автоматический размер по содержимому
                AutoSizeMode = AutoSizeMode.GrowAndShrink, // Может расти и уменьшаться
                Width = 380                             // Фиксированная ширина для центрирования
            };

            // ============= СЛОВАРИ ДЛЯ ТЕКСТОВОГО СОДЕРЖАНИЯ =============

            // Словарь для перевода технических названий достижений на русский
            // Ключ: английское имя (из системы достижений), Значение: русский перевод
            Dictionary<string, string> achievementNamesRu = new Dictionary<string, string>
            {
               { "First Meal", "Первая еда" },
                { "Playful Pet", "Игривый питомец" },
                { "Good Caretaker", "Хороший хозяин" },
                { "Survivor", "Выживший" },
                { "Master", "Мастер Тамагочи" },
                {"Clean Pet", "Чистюля"},
                {"Explorer", "Исследователь"},
                {"Personality Master", "Мастер характера"}
            };

            // Словарь с описаниями условий получения каждого достижения
            // Показывает игрокам, что нужно сделать для разблокировки
            Dictionary<string, string> achievementDescriptions = new Dictionary<string, string>
            {
                { "First Meal", "Покормите питомца впервые" },
                { "Playful Pet", "Сыграйте с питомцем 5 раз" },
                { "Good Caretaker", "Проживите с питомцем 3 дня" },
                { "Survivor", "Проживите с питомцем 7 дней" },
                { "Master", "Получите все достижения" },
                {"Clean Pet", "Искупай питомца"},
                {"Explorer", "Погуляй с питомцем"},
                {"Personality Master", "Развить черту характера до значения 80 или выше"}
            };

            // ============= ПРОВЕРКА ДАННЫХ ПЕРЕД ОТОБРАЖЕНИЕМ =============

            // Проверяем, что система достижений была корректно передана
            if (achievements == null || achievements.Achievements == null)
            {
                // Если данные отсутствуют - показываем сообщение об ошибке
                Label errorLabel = new Label
                {
                    Text = "Достижения не загружены",           // Текст ошибки
                    Font = new Font("Segoe UI", 12),           // Средний размер шрифта
                    ForeColor = Color.Red,                     // Красный цвет для привлечения внимания
                    AutoSize = true                           // Автоматический размер под текст
                };
                achievementsPanel.Controls.Add(errorLabel);    // Добавляем сообщение об ошибке
            }
            else
            {
                // ============= СОЗДАНИЕ КАРТОЧЕК ДЛЯ КАЖДОГО ДОСТИЖЕНИЯ =============

                // Перебираем все достижения из словаря системы достижений
                foreach (KeyValuePair<string, bool> achievement in achievements.Achievements)
                {
                    // Создаем панель (карточку) для одного достижения
                    Panel achievementPanel = new Panel
                    {
                        // Цвет фона зависит от статуса достижения:
                        // - Разблокировано: светло-зеленый (Color.FromArgb(240, 255, 240))
                        // - Заблокировано: светло-серый (Color.FromArgb(245, 245, 245))
                        BackColor = achievement.Value ?
                            Color.FromArgb(240, 255, 240) :
                            Color.FromArgb(245, 245, 245),

                        BorderStyle = BorderStyle.FixedSingle,     // Рамка вокруг карточки
                        Margin = new Padding(0, 0, 0, 10),         // Отступ снизу 10px между карточками
                        Padding = new Padding(15, 10, 15, 10),     // Внутренние отступы
                        Width = 380,                               // Фиксированная ширина
                        Height = 80,                               // Фиксированная высота
                        Tag = achievement.Key                      // Сохраняем ключ достижения в свойстве Tag
                    };

                    // ===== ИКОНКА СОСТОЯНИЯ =====
                    Label iconLabel = new Label
                    {
                        Text = achievement.Value ? "✅" : "🔒",    // Эмодзи: галочка или замок
                        Font = new Font("Segoe UI Emoji", 15),    // Шрифт с поддержкой эмодзи
                        Location = new Point(15, 20),             // Позиция: слева, по центру по вертикали
                        AutoSize = true                          // Автоматический размер под эмодзи
                    };

                    // ===== НАЗВАНИЕ ДОСТИЖЕНИЯ =====
                    Label nameLabel = new Label
                    {
                        // Получаем русское название или используем английское как запасной вариант
                        Text = achievementNamesRu.ContainsKey(achievement.Key) ?
                               achievementNamesRu[achievement.Key] : achievement.Key,

                        Font = new Font("Segoe UI", 12, FontStyle.Bold),  // Жирный шрифт для названия
                        Location = new Point(60, 10),                     // Позиция: справа от иконки
                        AutoSize = true,                                 // Автоматический размер под текст

                        // Цвет текста зависит от статуса:
                        // - Разблокировано: темно-зеленый (Color.FromArgb(0, 100, 0))
                        // - Заблокировано: серый (Color.FromArgb(100, 100, 100))
                        ForeColor = achievement.Value ?
                            Color.FromArgb(0, 100, 0) :
                            Color.FromArgb(100, 100, 100)
                    };

                    // ===== ОПИСАНИЕ УСЛОВИЯ =====
                    Label descLabel = new Label
                    {
                        // Получаем описание или сообщение об отсутствии
                        Text = achievementDescriptions.ContainsKey(achievement.Key) ?
                               achievementDescriptions[achievement.Key] : "Описание отсутствует",

                        Font = new Font("Segoe UI", 9),          // Мелкий шрифт для описания
                        Location = new Point(60, 32),            // Позиция: под названием
                        AutoSize = true,                         // Автоматический размер
                        ForeColor = Color.FromArgb(120, 120, 120) // Серый цвет для второстепенного текста
                    };

                    // ===== ТЕКСТОВЫЙ СТАТУС =====
                    Label statusLabel = new Label
                    {
                        Text = achievement.Value ? "РАЗБЛОКИРОВАНО" : "ЗАБЛОКИРОВАНО",  // Текст статуса
                        Font = new Font("Segoe UI", 9, FontStyle.Italic),             // Курсивный шрифт
                        Location = new Point(60, 52),                                 // Позиция: под описанием
                        AutoSize = true,                                              // Автоматический размер

                        // Цвет текста статуса:
                        // - Разблокировано: зеленый
                        // - Заблокировано: серый
                        ForeColor = achievement.Value ?
                            Color.Green :
                            Color.Gray
                    };

                    // ===== ДОБАВЛЕНИЕ ЭЛЕМЕНТОВ НА КАРТОЧКУ =====

                    // Добавляем все созданные элементы управления на панель достижения
                    achievementPanel.Controls.Add(iconLabel);     // Иконка состояния
                    achievementPanel.Controls.Add(nameLabel);     // Название достижения
                    achievementPanel.Controls.Add(descLabel);     // Описание условия
                    achievementPanel.Controls.Add(statusLabel);   // Текстовый статус

                    // Добавляем готовую карточку в панель с потоковым расположением
                    achievementsPanel.Controls.Add(achievementPanel);
                }
            }

            // ============= ЦЕНТРИРОВАНИЕ ПАНЕЛИ С ДОСТИЖЕНИЯМИ =============

            // Обработчик события изменения размера для динамического центрирования
            scrollPanel.SizeChanged += (s, e) =>
            {
                Panel panel = (Panel)s;  // Приводим отправителя к типу Panel

                // Если ширина области прокрутки больше ширины панели с достижениями
                if (panel.Width > achievementsPanel.Width)
                {
                    // Центрируем панель по горизонтали
                    achievementsPanel.Location = new Point(
                        (panel.Width - achievementsPanel.Width) / 2,  // Расчет отступа слева
                        0                                            // Отступ сверху = 0
                    );
                }
                else
                {
                    // Если панель шире области - прижимаем к левому краю
                    achievementsPanel.Location = new Point(0, 0);
                }
            };

            // Добавляем панель с достижениями в область прокрутки
            scrollPanel.Controls.Add(achievementsPanel);

            // Добавляем область прокрутки во вторую строку основной таблицы
            mainTable.Controls.Add(scrollPanel, 0, 1);

            // ============= ПАНЕЛЬ С КНОПКОЙ ЗАКРЫТИЯ =============

            // Панель для кнопки закрытия (нижняя часть формы)
            Panel closePanel = new Panel
            {
                Dock = DockStyle.Fill,           // Заполняет свою ячейку таблицы
                BackColor = Color.Transparent    // Прозрачный фон
            };

            // Кнопка для закрытия формы

            // //
            Button closeButton = new Button
            {
                Text = "Закрыть (Esc)",          // Текст с подсказкой про клавишу Escape
                Font = new Font("Segoe UI", 10, FontStyle.Bold),  // Средний жирный шрифт
                BackColor = Color.FromArgb(70, 130, 180),         // Синий фон как у заголовка
                ForeColor = Color.White,                           // Белый текст
                FlatStyle = FlatStyle.Flat,                       // Плоский стиль (без 3D-эффекта)
                Cursor = Cursors.Hand,                            // Курсор "рука" при наведении
                Width = 140,                                       // Фиксированная ширина
                Height = 40                                        // Фиксированная высота
            };

            closeButton.FlatAppearance.BorderSize = 0;  // Убираем рамку у плоской кнопки
            closeButton.Click += (s, e) => this.Close(); // Обработчик клика - закрытие формы

            // Обработчик для центрирования кнопки при изменении размера панели
            closePanel.SizeChanged += (s, e) =>
            {
                Panel panel = (Panel)s;  // Приводим отправителя к типу Panel

                // Центрируем кнопку по горизонтали и вертикали
                closeButton.Location = new Point(
                    (panel.Width - closeButton.Width) / 2,   // Центр по горизонтали
                    (panel.Height - closeButton.Height) / 2  // Центр по вертикали
                );
            };

            closePanel.Controls.Add(closeButton);           // Добавляем кнопку на панель
            mainTable.Controls.Add(closePanel, 0, 2);       // Добавляем панель в третью строку таблицы

            // ============= ФИНАЛЬНЫЙ ЭТАП =============

            // Добавляем основную таблицу с всеми элементами на форму
            this.Controls.Add(mainTable);
        }
    }
}