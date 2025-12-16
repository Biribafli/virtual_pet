using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TamagotchiWindows.Models
{
    // Класс системы сохранения и загрузки игры Тамагочи.
    //Отвечает за сохранение состояния игры в файлы и загрузку из них.
    // Использует JSON формат для сериализации данных.
    public class SaveSystem
    {
        // Путь к папке с сохранениями
        // Обычно это подпапка в директории приложения
        private string saveDirectory = "saves";

        // Конструктор класса SaveSystem.
        // Создает папку для сохранений, если она не существует.
        public SaveSystem()
        {
            // Проверяем, существует ли папка для сохранений
            if (!Directory.Exists(saveDirectory))
            {
                // Если не существует - создаем ее
                Directory.CreateDirectory(saveDirectory);
            }
        }

        // Метод сохранения игры в файл.
        // Объект питомца, который нужно сохранить
        // Система достижений для сохранения
        // Имя слота сохранения (по умолчанию "default")
        // true - если сохранение успешно, false - если произошла ошибка
        public bool SaveGame(VirtualPet pet, AchievementSystem achievements, string slotName = "default")
        {
            try
            {
                // Создаем объект с данными для сохранения
                // Используем анонимный тип для удобства
                object saveData = new
                {
                    Pet = pet,                    // Сохраняем питомца со всеми его свойствами
                    Achievements = achievements.Achievements // Сохраняем словарь достижений
                };

                // Сериализуем объект в JSON строку
                // JsonSerializerOptions настраивает процесс сериализации
                string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions
                {
                    WriteIndented = true,      // Добавляем отступы для читаемости файла
                    IncludeFields = true      // Включаем поля (не только свойства) при сериализации
                });

                // Формируем полный путь к файлу сохранения
                // Path.Combine корректно объединяет части пути для любой ОС
                string filePath = Path.Combine(saveDirectory, $"{slotName}.json");

                // Записываем JSON строку в файл
                // Если файл уже существует - он будет перезаписан
                File.WriteAllText(filePath, jsonString);

                // Возвращаем true, если все прошло успешно
                return true;
            }
            catch (Exception ex)
            {
                // Если произошла ошибка - показываем сообщение пользователю
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Возвращаем false, чтобы вызывающий код знал об ошибке
                return false;
            }
        }

        // Метод загрузки игры из файла.
        // Имя слота сохранения (по умолчанию "default")
        // Кортеж (VirtualPet, AchievementSystem) с загруженными данными или (null, null) если ошибка
        public (VirtualPet, AchievementSystem) LoadGame(string slotName = "default")
        {
            try
            {
                // Формируем полный путь к файлу сохранения
                string filePath = Path.Combine(saveDirectory, $"{slotName}.json");

                // Проверяем, существует ли файл сохранения
                if (!File.Exists(filePath))
                {
                    // Если файл не существует - возвращаем null значения
                    return (null, null);
                }

                // Читаем весь текст из файла сохранения
                string jsonString = File.ReadAllText(filePath);

                // Настраиваем параметры десериализации
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    IncludeFields = true // Включаем поля при десериализации
                };

                // Десериализуем JSON строку в объект типа SaveData
                SaveData saveData = JsonSerializer.Deserialize<SaveData>(jsonString, options);

                // Если питомец был успешно загружен
                if (saveData.Pet != null)
                {
                    // Вызываем метод восстановления личности питомца
                    // Это важно, так как при загрузке нужно восстановить связи между объектами
                    saveData.Pet.RestorePersonality();
                }

                // Создаем новую систему достижений для загрузки данных
                AchievementSystem achievements = new AchievementSystem();

                // Если достижения были сохранены
                if (saveData.Achievements != null)
                {
                    // Копируем загруженные достижения в новую систему
                    achievements.Achievements = saveData.Achievements;

                    // Вызываем метод обновления словаря достижений
                    // Это нужно для совместимости с новыми версиями игры
                    achievements.UpdateAchievementsDictionary();
                }

                // Возвращаем кортеж с загруженными данными
                return (saveData.Pet, achievements);
            }
            catch (Exception ex)
            {
                // Если произошла ошибка - показываем сообщение пользователю
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Возвращаем null значения
                return (null, null);
            }
        }

        // Внутренний класс для хранения данных сохранения.
        // Используется для правильной десериализации JSON.
        private class SaveData
        {
            // Сохраненный питомец со всеми его характеристиками.
            public VirtualPet Pet { get; set; }

            // Словарь достижений (ключ - название достижения, значение - разблокировано ли).
            public Dictionary<string, bool> Achievements { get; set; }
        }
    }
}