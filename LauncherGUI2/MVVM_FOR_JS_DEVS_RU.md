# MVVM в LauncherGUI2: подробный гайд для разработчика с опытом JS/HTML

## 1. Коротко: что тебе непривычно после JS

В вебе ты обычно делаешь так:
- вешаешь `onclick`,
- вызываешь функцию напрямую,
- руками меняешь DOM/состояние.

В этом проекте (Avalonia + MVVM) иначе:
- UI (XAML) **не вызывает методы напрямую** в большинстве случаев,
- UI говорит: "выполни команду",
- команда лежит во ViewModel,
- ViewModel меняет данные,
- UI сам обновляется через Binding.

Это и есть главный сдвиг мышления.

---

## 2. Архитектура в этом проекте

### `Model`
- Обычные структуры данных.
- Пример: `AppEntry` — одно приложение из `appsettings.json`.

Файлы:
- `Models/AppEntry.cs`
- `Models/LauncherConfig.cs`

### `ViewModel`
- Логика интерфейса и состояние, но без прямой работы с контролами.
- Здесь команды, коллекции, флаги видимости.

Файлы:
- `ViewModels/MainWindowViewModel.cs`
- `ViewModels/AppCardViewModel.cs`
- `ViewModels/RelayCommand.cs`
- `ViewModels/ViewModelBase.cs`

### `View`
- Разметка XAML и стили.
- Отображает данные из ViewModel.

Файлы:
- `Views/MainWindow.axaml`
- `Views/MainWindow.axaml.cs` (минимальный code-behind, только "оконные" вещи)

### `Service`
- Работа с внешними источниками (файл конфигурации, иконки и т.д.).

Файл:
- `Services/AppConfigService.cs`

---

## 3. Как данные попадают в интерфейс

1. `Program.cs` запускает Avalonia-приложение.
2. `App.axaml.cs` создает:
   - `AppConfigService`,
   - `MainWindowViewModel`.
3. `MainWindow` получает `DataContext = viewModel`.
4. В XAML все `{Binding ...}` читаются из этого `DataContext`.

Аналог в вебе:
- как будто ты передал компоненту большой `state` объект и шаблон читает поля из него.

---

## 4. Что такое Binding (на примерах проекта)

### Пример 1: коллекция карточек
```xml
<ItemsControl ItemsSource="{Binding InstalledApplications}" />
```
Это аналог:
```js
installedApplications.map(renderCard)
```

### Пример 2: показать/скрыть блок
```xml
<ScrollViewer IsVisible="{Binding IsSecuritySectionVisible}" />
```
Аналог:
```js
if (isSecuritySectionVisible) { showBlock() } else { hideBlock() }
```

### Пример 3: динамический класс
```xml
Classes.Active="{Binding IsApplicationsSectionSelected}"
```
Аналог:
```js
button.classList.toggle("active", isApplicationsSectionSelected)
```

---

## 5. Почему команда, а не прямой вызов функции

В XAML нельзя удобно писать "вызови метод VM и передай параметр" как в обычном JS `onclick`.
Поэтому используется `ICommand`.

`RelayCommand` — это обертка:
- `Execute` = что сделать,
- `CanExecute` = можно ли делать сейчас.

Плюсы:
- UI не знает детали логики,
- легко тестировать ViewModel,
- легко отключать кнопки через `CanExecute`,
- один и тот же command можно повесить на кнопку, меню, хоткей.

---

## 6. Как работает переключение секций

Кнопки меню:
```xml
Command="{Binding SwitchSectionCommand}"
CommandParameter="applications"
```

Во ViewModel:
- `SwitchSectionCommand` вызывает `SwitchSection`.
- `SwitchSection` меняет `_selectedSection`.
- вызывается `RaisePropertyChanged(...)`.

Дальше `IsVisible`-привязки обновляются, и интерфейс показывает нужный раздел.

Это аналог централизованного `setState({ currentSection })`.

---

## 7. Как работает установка приложения в этом проекте

### Состояния из JSON/модели
- `available` — приложение доступно для установки.
- `installed` — приложение уже установлено.

### На экране:
- главная секция берет `InstalledApplications`,
- секция установки берет `CatalogApplications` (неустановленные).

### Сценарий:
1. Нажали "Установить" -> `InstallApplicationCommand`.
2. Во ViewModel выставляется `IsInstalling=true`.
3. Карточка получает overlay с индикатором.
4. Когда установка завершена:
   - `Installed=true`,
   - карточка убирается из каталога,
   - карточка добавляется в установленные.

---

## 8. Что такое `INotifyPropertyChanged` и зачем он нужен

Когда во ViewModel меняется свойство, UI об этом не узнает "сам".
Нужно явно сообщить:
```csharp
RaisePropertyChanged(nameof(SomeProperty));
```

Это аналог:
- "триггернуть перерендер компонента" в веб-фреймворках.

`ViewModelBase` реализует эту механику один раз для всех ViewModel.

---

## 9. Зачем нужен `MainWindow.axaml.cs`, если есть MVVM

MVVM не запрещает code-behind, он просит держать там только UI-специфику.

В этом проекте в `MainWindow.axaml.cs` оставлены:
- перетаскивание окна,
- сворачивание/максимизация/закрытие.

Бизнес-логика (переключение разделов, установка, запуск, фильтрация) — во ViewModel.

---

## 10. Соответствие "JS-подход -> MVVM-подход"

1. `onclick="fn()"` -> `Command="{Binding SomeCommand}"`
2. `classList.toggle(...)` -> `Classes.SomeClass="{Binding BoolFlag}"`
3. `if (...) render block` -> `IsVisible="{Binding BoolFlag}"`
4. `array.map(...)` -> `ItemsControl ItemsSource + DataTemplate`
5. `state update` -> изменить свойство VM + `RaisePropertyChanged`

---

## 11. Как читать этот проект пошагово (рекомендованный маршрут)

1. Открой `App.axaml.cs` — увидишь, где создается VM.
2. Открой `MainWindowViewModel.cs` — поймешь, какие данные и команды есть у экрана.
3. Открой `MainWindow.axaml` — найди, где эти свойства/команды привязаны.
4. Пройдись по `AppCardViewModel.cs` — это состояние одной карточки.
5. Посмотри `RelayCommand.cs` — как команда превращает метод в ICommand.

---

## 12. Мини-шпаргалка по терминам

- `DataContext`: источник данных для Binding (обычно ViewModel).
- `Binding`: связь свойства UI со свойством VM.
- `Command`: объект действия для кнопки/элемента UI.
- `DataTemplate`: шаблон одного элемента коллекции.
- `ItemsControl`: рендер списка элементов.
- `INotifyPropertyChanged`: уведомление UI об изменении свойств.

---

## 13. Если хочешь “думать как в JS”, но писать MVVM

Практическая формула:
1. В VM создай `state` (свойства).
2. В VM создай "handler" как `RelayCommand`.
3. В XAML привяжи кнопку к команде.
4. В XAML привяжи UI к `state`.
5. При изменении `state` вызывай `RaisePropertyChanged`.

Это почти тот же цикл, что в вебе, только связи декларативные и типобезопасные.
