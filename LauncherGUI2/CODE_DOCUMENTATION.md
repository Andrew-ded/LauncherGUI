# LauncherGUI2: Документация по коду

## 1. Назначение проекта
`LauncherGUI2` — desktop-приложение на Avalonia для отображения и запуска списка приложений из `appsettings.json`.

## 2. Структура проекта
- `Program.cs`: точка входа и конфигурация `AppBuilder`.
- `App.axaml` / `App.axaml.cs`: глобальная тема и инициализация главного окна.
- `Models/`: модели данных (`LauncherConfig`, `AppEntry`).
- `Services/AppConfigService.cs`: загрузка конфига и подготовка иконок.
- `ViewModels/`: бизнес-логика окна и команд.
- `Views/MainWindow.axaml`: разметка интерфейса и стили.
- `Views/MainWindow.axaml.cs`: UI-обработчики (кнопки окна, модальный overlay).

## 3. Поток запуска приложения
1. `Program.Main` вызывает `BuildAvaloniaApp().StartWithClassicDesktopLifetime(args)`.
2. В `App.OnFrameworkInitializationCompleted` создается `AppConfigService`.
3. `AppConfigService.Load()` читает `appsettings.json` и собирает `LauncherConfig`.
4. На основе конфига создается `MainWindowViewModel`.
5. `MainWindow` получает `DataContext` и отображает UI.

## 4. Конфигурация (`appsettings.json`)
Файл должен содержать массив приложений:
- `Name`: имя;
- `Version`: версия;
- `ExecutablePath`: путь к EXE;
- `IconPath`: путь к иконке (опционально, в текущей версии используется системная иконка EXE).

## 5. ViewModel-слой
### `MainWindowViewModel`
- Хранит список `Applications`.
- Предоставляет команды:
  - `LaunchApplicationCommand`;
  - `DeleteApplicationCommand`;
  - `SwitchSectionCommand`.
- Управляет видимостью разделов:
  - `applications`;
  - `security`;
  - `settings`.

### `AppCardViewModel`
Обертка над `AppEntry` для привязки карточки в UI.

### `RelayCommand`
Базовая реализация `ICommand` для XAML-привязок.

## 6. UI и поведение
### Разделы интерфейса
- **Приложения**: рабочие карточки с запуском/удалением.
- **Безопасность**: те же карточки, визуально приглушены, запуск блокируется.
- **Настройки**: секция-заглушка под будущие настройки.

### Модальное окно отсутствия прав
Отрисовано как in-app overlay (`NoPermissionOverlay`) внутри `MainWindow`, а не отдельным системным `Window`.
Это исключает появление системной рамки вокруг модалки.

## 7. Загрузка иконок
`AppConfigService`:
1. Берет `ExecutablePath`.
2. Извлекает иконку через `Icon.ExtractAssociatedIcon`.
3. Конвертирует `System.Drawing.Bitmap` в PNG-поток.
4. Создает `Avalonia.Media.Imaging.Bitmap` и сохраняет в `AppEntry.IconApp`.

## 8. Известные ограничения
- Использование `System.Drawing` и `Icon.*` ориентировано на Windows.
- В проекте возможны предупреждения анализатора (`CA1416`) о платформенной специфике.
- `NormalizeIconPath` подготовлен как утилита, но сейчас не используется.

## 9. Как расширять проект
- Добавить реальные настройки в секцию `settings`.
- Разделить `security` на отдельную коллекцию приложений с политиками доступа.
- Добавить персистентное удаление/изменение приложений обратно в `appsettings.json`.
- Перенести логику модальных окон в отдельный сервис диалогов.

## 10. Быстрая проверка после изменений
```powershell
dotnet build -o .\build-check
```
Если `build-check` не нужен, папку можно удалить вручную.
