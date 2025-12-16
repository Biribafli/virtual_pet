using System.Collections.Generic;

namespace TamagotchiWindows.Models
{
    // Система достижений для игры Тамагочи.
    // Отслеживает прогресс игрока и разблокирует достижения при выполнении условий.
    // Использует словарь для хранения состояния достижений.
    public class AchievementSystem
    {
        // Словарь достижений, где:
        // - Ключ: уникальное имя достижения на английском (для программирования)
        // - Значение: разблокировано ли достижение (true/false)
        public Dictionary<string, bool> Achievements { get; set; }

        // Конструктор системы достижений.
        // Инициализирует словарь всеми достижениями в заблокированном состоянии.
        public AchievementSystem()
        {
            // Инициализируем все достижения, включая новые
            Achievements = new Dictionary<string, bool>
            {
                // Старые достижения (базовые)
                {"First Meal", false},        // Первое кормление
                {"Playful Pet", false},       // Игровой питомец
                {"Good Caretaker", false},    // Хороший хозяин
                {"Survivor", false},          // Выживший
                {"Master", false},            // Мастер игры
                
                // Новые достижения (добавлены в обновлении)
                {"Clean Pet", false},         // Первая ванна
                {"Explorer", false},          // Первая прогулка
                {"Personality Master", false} // Мастер характера
            };
        }

        // Основной метод проверки достижений.
        // Проверяет состояние питомца и разблокирует достижения при выполнении условий.
        //Объект питомца для проверки статистики
        // Список названий только что разблокированных достижений (на русском)
        public List<string> CheckAchievements(VirtualPet pet)
        {
            // Список для хранения названий только что разблокированных достижений
            List<string> unlocked = new List<string>();

            // Словарь для перевода технических названий на русский для отображения
            Dictionary<string, string> achievementNamesRu = new Dictionary<string, string>
            {
                {"First Meal", "Первая еда"},
                {"Playful Pet", "Игривый питомец"},
                {"Good Caretaker", "Хороший хозяин"},
                {"Survivor", "Выживший"},
                {"Master", "Мастер Тамагочи"},
                {"Clean Pet", "Чистюля"},
                {"Explorer", "Исследователь"},
                {"Personality Master", "Мастер характера"}
            };

            // ==================== ПРОВЕРКА БАЗОВЫХ ДОСТИЖЕНИЙ ====================

            // Проверяем каждое достижение безопасно, с проверкой существования ключа

            // Достижение "Первая еда" - покормить питомца хотя бы 1 раз
            //содержится ли указанный ключ в словаре
            if (Achievements.ContainsKey("First Meal") && pet.MealsEaten >= 1 && !Achievements["First Meal"])
            {
                Achievements["First Meal"] = true;                    // Разблокируем
                unlocked.Add(achievementNamesRu["First Meal"]);      // Добавляем русское название
            }

            // Достижение "Игривый питомец" - сыграть с питомцем 5 раз
            if (Achievements.ContainsKey("Playful Pet") && pet.GamesPlayed >= 5 && !Achievements["Playful Pet"])
            {
                Achievements["Playful Pet"] = true;
                unlocked.Add(achievementNamesRu["Playful Pet"]);
            }

            // Достижение "Хороший хозяин" - прожить с питомцем 3 дня
            if (Achievements.ContainsKey("Good Caretaker") && pet.DaysSurvived >= 3 && !Achievements["Good Caretaker"])
            {
                Achievements["Good Caretaker"] = true;
                unlocked.Add(achievementNamesRu["Good Caretaker"]);
            }

            // Достижение "Выживший" - прожить с питомцем 7 дней
            if (Achievements.ContainsKey("Survivor") && pet.DaysSurvived >= 7 && !Achievements["Survivor"])
            {
                Achievements["Survivor"] = true;
                unlocked.Add(achievementNamesRu["Survivor"]);
            }

            // ==================== ПРОВЕРКА НОВЫХ ДОСТИЖЕНИЙ ====================

            // Достижение "Чистюля" - искупать питомца хотя бы 1 раз
            if (Achievements.ContainsKey("Clean Pet") && pet.BathsTaken >= 1 && !Achievements["Clean Pet"])
            {
                Achievements["Clean Pet"] = true;
                unlocked.Add(achievementNamesRu["Clean Pet"]);
            }

            // Достижение "Исследователь" - погулять с питомцем хотя бы 1 раз
            if (Achievements.ContainsKey("Explorer") && pet.WalksTaken >= 1 && !Achievements["Explorer"])
            {
                Achievements["Explorer"] = true;
                unlocked.Add(achievementNamesRu["Explorer"]);
            }

            // Достижение "Мастер характера" - развить любую черту личности до 80+
            if (pet.PersonalityTraits != null && Achievements.ContainsKey("Personality Master"))
            {
                bool hasMaxTrait = false;  // Флаг наличия максимально развитой черты

                // Перебираем все черты личности питомца
                foreach (KeyValuePair<string, int> trait in pet.PersonalityTraits)
                {
                    // Если черта развита до 80 или больше и достижение еще не разблокировано
                    if (trait.Value >= 80 && !Achievements["Personality Master"])
                    {
                        hasMaxTrait = true;  // Нашли развитую черту
                        break;               // Прерываем цикл - достаточно одной черты
                    }
                }

                // Если найдена развитая черта и достижение еще не разблокировано
                if (hasMaxTrait && !Achievements["Personality Master"])
                {
                    Achievements["Personality Master"] = true;
                    unlocked.Add(achievementNamesRu["Personality Master"]);
                }
            }

            // ==================== СЕКРЕТНОЕ ДОСТИЖЕНИЕ ====================

            // Достижение "Мастер Тамагочи" - получить ВСЕ остальные достижения
            // Это самое сложное и финальное достижение
            if (Achievements.ContainsKey("Master"))
            {
                // Проверяем все базовые достижения
                bool allBasic = Achievements.ContainsKey("First Meal") && Achievements["First Meal"] &&
                               Achievements.ContainsKey("Playful Pet") && Achievements["Playful Pet"] &&
                               Achievements.ContainsKey("Good Caretaker") && Achievements["Good Caretaker"] &&
                               Achievements.ContainsKey("Survivor") && Achievements["Survivor"];

                // Проверяем все новые достижения
                bool allNew = Achievements.ContainsKey("Clean Pet") && Achievements["Clean Pet"] &&
                             Achievements.ContainsKey("Explorer") && Achievements["Explorer"];

                // Если получены ВСЕ достижения (базовые + новые) и "Мастер" еще не разблокирован
                if (allBasic && allNew && !Achievements["Master"])
                {
                    Achievements["Master"] = true;
                    unlocked.Add(achievementNamesRu["Master"]);
                }
            }

            // Возвращаем список только что разблокированных достижений
            return unlocked;
        }

        // Метод для обновления словаря достижений при загрузке старых сохранений.
        // Обеспечивает совместимость между разными версиями игры.
        // Если в старом сохранении нет новых достижений - добавляет их.
        public void UpdateAchievementsDictionary()
        {
            // Стандартный набор достижений для всех версий игры
            Dictionary<string, bool> defaultAchievements = new Dictionary<string, bool>
            {
                {"First Meal", false},
                {"Playful Pet", false},
                {"Good Caretaker", false},
                {"Survivor", false},
                {"Master", false},
                {"Clean Pet", false},
                {"Explorer", false},
                {"Personality Master", false}
            };

            // Перебираем стандартный набор достижений
            foreach (KeyValuePair<string, bool> achievement in defaultAchievements)
            {
                // Если в загруженном словаре нет этого достижения
                if (!Achievements.ContainsKey(achievement.Key))
                {
                    // Добавляем недостающее достижение с дефолтным значением (false)
                    Achievements[achievement.Key] = achievement.Value;
                }
            }
        }
    }
}