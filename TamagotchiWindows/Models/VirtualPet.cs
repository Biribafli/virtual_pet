using System;
using System.Collections.Generic;
using System.Linq;

namespace TamagotchiWindows.Models
{
    // Основной класс виртуального питомца.
    // Содержит все свойства, методы и логику поведения питомца.
    // Реализует систему характеристик, личности и взаимодействий.
    public class VirtualPet
    {
        // ==================== ОСНОВНЫЕ ХАРАКТЕРИСТИКИ ====================

        //Имя питомца, задается при создании
        public string Name { get; set; }

        // Уровень голода (0-100). 0 - умирает от голода, 100 - сытый
        public int Hunger { get; set; } = 80;

        // Уровень счастья (0-100). Влияет на настроение и здоровье
        public int Happiness { get; set; } = 80;

        // Уровень здоровья (0-100). При 0 питомец умирает
        public int Health { get; set; } = 100;

        //Уровень энергии (0-100). Нужен для активных действий
        public int Energy { get; set; } = 80;

        //Уровень чистоты (0-100). Влияет на здоровье
        public int Hygiene { get; set; } = 80;

        //Потребность в прогулке (0-100). 100 - не хочет гулять, 0 - очень хочет
        public int Exploration { get; set; } = 100; // НАЧИНАЕМ С 100 (нет потребности)

        // ==================== СТАТИСТИКА И СОСТОЯНИЯ ====================

        // Возраст питомца в игровых днях
        public int Age { get; set; } = 0;

        // Сколько дней питомец прожил (для достижений)
        public int DaysSurvived { get; set; } = 0;

        // Спит ли питомец в данный момент
        public bool IsSleeping { get; set; } = false;

        //Счетчик съеденных кормов
        public int MealsEaten { get; set; } = 0;

        //Счетчик сыгранных игр
        public int GamesPlayed { get; set; } = 0;

        // Счетчик принятых ванн
        public int BathsTaken { get; set; } = 0;

        // Счетчик прогулок
        public int WalksTaken { get; set; } = 0;

        // ==================== СИСТЕМА ЛИЧНОСТИ ====================

        // Словарь черт личности.
        // Ключ - название черты на английском, Значение - уровень развития (0-100)
        public Dictionary<string, int> PersonalityTraits { get; set; }

        //  Текущая доминирующая личность на русском языке 
        public string CurrentPersonality { get; set; }

        //  
        // Конструктор питомца. Создает нового питомца с заданным именем.
        //  
        // Имя питомца
        public VirtualPet(string name)
        {
            Name = name;
            InitializePersonality();  // Инициализирует черты личности
            UpdatePersonalityType();  // Определяет текущий тип личности
        }

        //  
        // Инициализация системы личности.
        // Создает словарь с начальными значениями для всех черт.
        // Все черты начинаются с 0.
        //  
        public void InitializePersonality()
        {
            PersonalityTraits = new Dictionary<string, int>
            {
                { "Lazy", 0 },      // Ленивый
                { "Active", 0 },    // Активный
                { "Smart", 0 },     // Умный
                { "Playful", 0 },   // Игривый
                { "Clean", 0 },     // Чистюля
                { "Curious", 0 },   // Любопытный
                { "Moody", 0 }      // Капризный
            };
        }

        //  
        // Восстановление личности после загрузки из сохранения.
        // Если черты личности не были загружены, инициализирует их заново.
        //  
        public void RestorePersonality()
        {
            if (PersonalityTraits == null || PersonalityTraits.Count == 0)
            {
                InitializePersonality();
            }
            UpdatePersonalityType();  // Обновляет тип личности после восстановления
        }

        //  
        // Определение текущего типа личности на основе развитых черт.
        // Находит черту с максимальным значением и переводит ее в русское название.
        //  
        public void UpdatePersonalityType()
        {
            // Проверка на наличие черт личности
            if (PersonalityTraits == null || PersonalityTraits.Count == 0)
            {
                CurrentPersonality = "Обычный";
                return;
            }

            int maxTraitValue = 0;
            string dominantTrait = "Normal";

            // Перебор всех черт для поиска максимальной
            foreach (KeyValuePair<string, int> trait in PersonalityTraits)
            {
                if (trait.Value > maxTraitValue)
                {
                    maxTraitValue = trait.Value;
                    dominantTrait = trait.Key;
                }
            }

            // Преобразование английского названия в русское с помощью switch выражения
            CurrentPersonality = dominantTrait switch
            {
                "Lazy" => "Ленивый",
                "Active" => "Активный",
                "Smart" => "Умный",
                "Playful" => "Игривый",
                "Clean" => "Чистюля",
                "Curious" => "Любопытный",
                "Moody" => "Капризный",
                _ => "Обычный"  // По умолчанию
            };
        }


        // Основной метод обновления статуса питомца.
        // Вызывается периодически (например, каждые 30 секунд).
        // Обновляет все характеристики с учетом состояния и личности.

        public void UpdateStatus()
        {
            // ===== ОБНОВЛЕНИЕ ПРИ СНЕ =====
            if (IsSleeping)
            {
                Energy = Math.Min(100, Energy + 10);      // Энергия восстанавливается
                Hunger = Math.Max(0, Hunger - 1);         // Голод медленно растет
                Happiness = Math.Max(0, Happiness - 1);   // Счастье медленно падает
                Hygiene = Math.Max(0, Hygiene - 1);       // Чистота медленно падает
                // Во сне потребность в прогулке УМЕНЬШАЕТСЯ (отдыхает)
                Exploration = Math.Max(0, Exploration - 1);

                // Развитие черты "Ленивый" во время сна
                if (PersonalityTraits != null && PersonalityTraits.ContainsKey("Lazy"))
                    PersonalityTraits["Lazy"] = Math.Min(100, PersonalityTraits["Lazy"] + 1);
            }
            // ===== ОБНОВЛЕНИЕ ПРИ БОДРСТВОВАНИИ =====
            else
            {
                Hunger = Math.Max(0, Hunger - 2);         // Голод растет быстрее
                Happiness = Math.Max(0, Happiness - 1);   // Счастье падает
                Energy = Math.Max(0, Energy - 2);         // Энергия тратится
                Hygiene = Math.Max(0, Hygiene - 2);       // Чистота падает быстрее
                // Потребность в прогулке УВЕЛИЧИВАЕТСЯ когда бодрствует
                Exploration = Math.Max(0, Exploration - 2);

                // Развитие черты "Капризный" при низкой чистоте
                if (Hygiene < 30 && PersonalityTraits != null && PersonalityTraits.ContainsKey("Moody"))
                {
                    PersonalityTraits["Moody"] = Math.Min(100, PersonalityTraits["Moody"] + 1);
                }
            }

            // Применение эффектов личности на изменение характеристик
            ApplyPersonalityEffects();

            // ===== ВЛИЯНИЕ НА ЗДОРОВЬЕ =====
            // Здоровье ухудшается при низких показателях других характеристик
            if (Hunger < 20) Health = Math.Max(0, Health - 3);
            if (Happiness < 20) Health = Math.Max(0, Health - 2);
            if (Energy < 10) Health = Math.Max(0, Health - 1);
            if (Hygiene < 20) Health = Math.Max(0, Health - 2);

            // Если потребность в прогулке низкая (<20), питомец скучает
            if (Exploration < 20)
            {
                Happiness = Math.Max(0, Happiness - 2);
            }

            // Восстановление здоровья при хороших условиях
            if (Hunger > 80 && Happiness > 80 && Hygiene > 60 && Exploration > 50)
                Health = Math.Min(100, Health + 1);

            // Гарантируем, что все значения в пределах 0-100
            ClampValues();

            // Обновляем тип личности после всех изменений
            UpdatePersonalityType();
        }


        // Ограничение всех характеристик в диапазоне 0-100.
        // Предотвращает выход значений за допустимые пределы.

        private void ClampValues()
        {
            Hunger = Math.Max(0, Math.Min(100, Hunger));
            Happiness = Math.Max(0, Math.Min(100, Happiness));
            Health = Math.Max(0, Math.Min(100, Health));
            Energy = Math.Max(0, Math.Min(100, Energy));
            Hygiene = Math.Max(0, Math.Min(100, Hygiene));
            Exploration = Math.Max(0, Math.Min(100, Exploration));
        }


        // Применение эффектов текущей личности на изменение характеристик.
        // Каждый тип личности по-разному влияет на расход характеристик.

        private void ApplyPersonalityEffects()
        {
            if (PersonalityTraits == null) return;

            // Ленивый медленнее тратит энергию и голодает
            if (CurrentPersonality == "Ленивый")
            {
                Energy = Math.Max(0, Energy - 1);      // Медленнее тратит энергию
                Hunger = Math.Max(0, Hunger - 1);      // Медленнее голодает
                // Ленивые медленнее хотят гулять
                Exploration = Math.Max(0, Exploration - 1);
            }
            // Активный быстрее тратит энергию
            else if (CurrentPersonality == "Активный")
            {
                Energy = Math.Max(0, Energy - 3);      // Быстрее тратит энергию
                Happiness = Math.Max(0, Happiness - 2); // Быстрее теряет настроение
                // Активные быстрее хотят гулять
                Exploration = Math.Max(0, Exploration - 3);
            }
            // Капризный быстрее теряет настроение
            else if (CurrentPersonality == "Капризный")
            {
                Happiness = Math.Max(0, Happiness - 3); // Быстро теряет настроение
            }
            // Любопытный быстрее хочет гулять
            else if (CurrentPersonality == "Любопытный")
            {
                Exploration = Math.Max(0, Exploration - 4); // Быстро растет потребность в прогулке
            }
        }

        // ==================== МЕТОДЫ ВЗАИМОДЕЙСТВИЯ ====================


        // Покормить питомца.
        // Уменьшает голод, немного увеличивает счастье.

        public void Feed()
        {
            if (!IsSleeping)
            {
                Hunger = Math.Min(100, Hunger + 30);      // +30 к сытости
                Happiness = Math.Min(100, Happiness + 5); // +5 к счастью
                MealsEaten++;                             // Увеличиваем счетчик

                // Развитие черты "Ленивый" при переедании
                if (Hunger > 90 && PersonalityTraits != null && PersonalityTraits.ContainsKey("Lazy"))
                {
                    PersonalityTraits["Lazy"] = Math.Min(100, PersonalityTraits["Lazy"] + 1);
                }
            }
        }


        // Поиграть с питомцем.
        // Требует энергии, сильно увеличивает счастье.

        public void Play()
        {
            if (!IsSleeping && Energy > 20)
            {
                Happiness = Math.Min(100, Happiness + 20);  // +20 к счастью
                Energy = Math.Max(0, Energy - 15);          // -15 энергии
                Hygiene = Math.Max(0, Hygiene - 5);         // -5 чистоты
                // Игра немного уменьшает потребность в прогулке
                Exploration = Math.Min(100, Exploration + 10);
                GamesPlayed++;                              // Увеличиваем счетчик

                // Развитие черт "Активный" и "Игривый"
                if (PersonalityTraits != null)
                {
                    if (PersonalityTraits.ContainsKey("Active"))
                        PersonalityTraits["Active"] = Math.Min(100, PersonalityTraits["Active"] + 2);
                    if (PersonalityTraits.ContainsKey("Playful"))
                        PersonalityTraits["Playful"] = Math.Min(100, PersonalityTraits["Playful"] + 2);
                }
            }
        }


        // Искупать питомца.
        // Сильно увеличивает чистоту, немного увеличивает счастье.

        public void TakeBath()
        {
            if (!IsSleeping)
            {
                Hygiene = Math.Min(100, Hygiene + 40);      // +40 к чистоте
                Happiness = Math.Min(100, Happiness + 10);  // +10 к счастью
                Energy = Math.Max(0, Energy - 10);          // -10 энергии
                BathsTaken++;                               // Увеличиваем счетчик

                // Развитие черты "Чистюля"
                if (PersonalityTraits != null && PersonalityTraits.ContainsKey("Clean"))
                {
                    PersonalityTraits["Clean"] = Math.Min(100, PersonalityTraits["Clean"] + 3);
                }

                // Развитие черты "Капризный" если часто купают
                if (BathsTaken > 3 && Hygiene > 90 && PersonalityTraits != null && PersonalityTraits.ContainsKey("Moody"))
                {
                    PersonalityTraits["Moody"] = Math.Min(100, PersonalityTraits["Moody"] + 1);
                }
            }
        }


        // Погулять с питомцем.
        // Сильно уменьшает потребность в прогулке, увеличивает счастье.
        // Требует много энергии.

        public void GoForWalk()
        {
            if (!IsSleeping && Energy > 30)
            {
                Exploration = Math.Min(100, Exploration + 50);  // +50 к потребности в прогулке
                Happiness = Math.Min(100, Happiness + 25);      // +25 к счастью
                Energy = Math.Max(0, Energy - 25);              // -25 энергии
                Hunger = Math.Max(0, Hunger - 10);              // -10 сытости
                Hygiene = Math.Max(0, Hygiene - 15);            // -15 чистоты
                WalksTaken++;                                   // Увеличиваем счетчик

                // Развитие черт "Активный" и "Любопытный"
                if (PersonalityTraits != null)
                {
                    if (PersonalityTraits.ContainsKey("Active"))
                        PersonalityTraits["Active"] = Math.Min(100, PersonalityTraits["Active"] + 3);
                    if (PersonalityTraits.ContainsKey("Curious"))
                        PersonalityTraits["Curious"] = Math.Min(100, PersonalityTraits["Curious"] + 3);
                }
            }
        }


        // Уложить питомца спать.
        // Восстанавливает энергию, меняет состояние.

        public void Sleep()
        {
            if (!IsSleeping)
            {
                IsSleeping = true;
                Energy = Math.Min(100, Energy + 20);  // +20 энергии

                // Развитие черты "Ленивый" при сне
                if (PersonalityTraits != null && PersonalityTraits.ContainsKey("Lazy"))
                {
                    PersonalityTraits["Lazy"] = Math.Min(100, PersonalityTraits["Lazy"] + 2);
                }
            }
        }


        // Разбудить питомца.
        // Меняет состояние сна на бодрствование.

        public void WakeUp()
        {
            if (IsSleeping)
            {
                IsSleeping = false;
            }
        }


        // Полечить питомца.
        // Увеличивает здоровье, требует энергии.

        public void Heal()
        {
            if (!IsSleeping)
            {
                Health = Math.Min(100, Health + 30);  // +30 здоровья
                Energy = Math.Max(0, Energy - 10);    // -10 энергии
            }
        }

        // ==================== МЕТОДЫ ПОЛУЧЕНИЯ СТАТУСА ====================


        // Проверка, нужна ли прогулка питомцу.
        //  true если потребность в прогулке меньше 20 
        public bool NeedsWalk()
        {
            return Exploration < 20;
        }


        // Получить текстовый статус потребности в прогулке.

        //  Текстовое описание желания гулять 
        public string GetWalkStatus()
        {
            if (Exploration >= 80) return "Не хочет гулять";
            if (Exploration >= 50) return "Можно погулять";
            if (Exploration >= 20) return "Хочет погулять";
            return "Очень хочет гулять!";
        }


        // Получить текущее настроение питомца.
        // Зависит от уровня счастья и состояния сна.

        //  Текстовое описание настроения с эмодзи 
        public string GetMood()
        {
            if (IsSleeping) return "💤 Спит";
            if (Happiness >= 70) return "😊 Счастливый";
            if (Happiness >= 40) return "😐 Спокойный";
            return "😞 Грустный";
        }


        // Получить статус здоровья питомца.

        //  Текстовое описание состояния здоровья 
        public string GetHealthStatus()
        {
            if (Health >= 70) return "Отличное";
            if (Health >= 40) return "Хорошее";
            if (Health >= 20) return "Плохое";
            return "Критическое";
        }


        // Получить информацию о личности питомца.
        // Показывает текущий тип и развитые черты.

        // Текст с информацией о личности
        public string GetPersonalityInfo()
        {
            if (PersonalityTraits == null || PersonalityTraits.Count == 0)
                return "Характер: Обычный";

            // LINQ запрос для получения черт со значением больше 20
            IEnumerable<string> activeTraits = PersonalityTraits
                .Where(t => t.Value > 20)                      // Фильтр: черты > 20
                .Select(t => $"{t.Key}: {t.Value}");          // Преобразование в строку

            return $"Характер: {CurrentPersonality}\n" +
                   $"Черты: {string.Join(", ", activeTraits)}";
        }


        // Получить путь к изображению питомца в зависимости от состояния.
        // Определяет какое изображение показывать на основе настроения и здоровья.

        //Имя файла изображения
        public string GetImagePath()
        {
            if (!IsAlive()) return "dead_pet.png";        // Мертв
            if (IsSleeping) return "sleeping_pet.png";    // Спит
            if (Happiness >= 70) return "happy_pet.png";  // Счастливый
            if (Happiness >= 40) return "normal_pet.png"; // Нормальный
            return "sad_pet.png";                         // Грустный
        }


        // Проверка, жив ли питомец.

        // true если здоровье больше 0
        public bool IsAlive() => Health > 0;
    }
}