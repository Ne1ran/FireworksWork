using Timer = System.Windows.Forms.Timer;

namespace FireworksWork
{
    public partial class Form1 : Form
    {
        private readonly Random _random = new Random(); // Создаем рандом для генерации случайных чисел
        private List<Firework> _existingFireworks = new(); // Список существующих фейерверков

        private const int _tickRate = 60; // Частота обновления (в мс)

        private const int _rgbMin = 0; // Минимум и максимум RGB для генерации цветов
        private const int _rgbMax = 255;

        private const int _minAdditionalColors = 10; // Минимум и максимум цветов в фейерверках
        private const int _maxAdditionalColors = 30;

        private const int _borderOffsetX = 150; // Отступы по X и Y от краев
        private const int _borderOffsetY = 125;

        private const int _minLifeTime = 1000; // Минимум и максимум времени жизни фейерверка
        private const int _maxLifeTime = 5000;

        private const int _minRadius = 200; // Минимум и максимум радиуса фейерверка
        private const int _maxRadius = 300;

        private const int _minParticles = 8; // Минимум и максимум частиц в нем
        private const int _maxParticles = 24;

        private const int _maxActiveFireworks = 7; // Максимум количество фейерверков на экране

        private const int _minFireworkSpawnDelay = 700; // Минимум и максимум задержки для создания нового фейерверка 
        private const int _maxFireworkSpawnDelay = 1700;

        private int _minDrawX; // Минимальные и максимальные координаты для создания фейерверка
        private int _minDrawY;
        private int _maxDrawX;
        private int _maxDrawY;
        private int _currentSpawnDelay; // Текущая задержка для создания фейерверка
        private Timer _animationTimer = null!; // Таймер

        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true); // Ставим возможность поддержки прозрачных объектов
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _animationTimer = new Timer(); // Инициализируем таймер. Задаем ему интервал (тикрейт) и подписываемся на ивент тика
            _animationTimer.Interval = _tickRate;
            _animationTimer.Tick += AnimationTimer_Tick;

            Size windowSize = Size; // Берем текущее окно приложения и рассчитываем минимальные и максимальные координаты для создания фейерверка
            _minDrawX = _borderOffsetX;
            _minDrawY = _borderOffsetY;
            _maxDrawX = windowSize.Width - _borderOffsetX;
            _maxDrawY = windowSize.Height - _borderOffsetY;

            InitializeFireworks(); // Первичная инициализация фейерверков. Можно убрать и работать только от таймера
            await Task.Delay(100); // Небольшая задержка
            _animationTimer.Start(); // Запуск таймера
        }

        private void InitializeFireworks()
        {
            int fireworksCountStart = _random.Next(_maxActiveFireworks); // Генерируем число изначальных фейерверков
            for (int i = 0; i < fireworksCountStart; i++) {
                _existingFireworks.Add(CreateNewFirework()); // Создаем их и добавляем в список
            }
        }

        private Firework CreateNewFirework()
        {
            int radius = _random.Next(_minRadius, _maxRadius); // Генерируем нужные параметры через рандом
            int lifeTime = _random.Next(_minLifeTime, _maxLifeTime);
            int randomX = _random.Next(_minDrawX, _maxDrawX);
            int randomY = _random.Next(_minDrawY, _maxDrawY);
            int particles = _random.Next(_minParticles, _maxParticles);
            int additionalColorsCount = _random.Next(_minAdditionalColors, _maxAdditionalColors);
            List<Color> additionalColors = new List<Color>(additionalColorsCount);
            for (int i = 0; i < additionalColorsCount; i++)
            {
                additionalColors.Add(GenerateColor()); // Генерируем цвета
            }

            Point centerPosition = new Point(randomX, randomY);
            var firework = new Firework(centerPosition, lifeTime, radius, particles, additionalColors, _tickRate); // Создаем класс фейерверка
            return firework;
        }

        private Color GenerateColor()
        {
            int r = _random.Next(_rgbMin, _rgbMax); // Генерируем RGB цвета фейерверка через рандом
            int g = _random.Next(_rgbMin, _rgbMax);
            int b = _random.Next(_rgbMin, _rgbMax);
            return Color.FromArgb(r, g, b); // Создаем цвет
        }

        private void AnimationTimer_Tick(object? sender, EventArgs? e)
        {
            if (_existingFireworks.Count < _maxActiveFireworks) {  // Проверка на возможность создания нового фейерверка
                if (_currentSpawnDelay < 0) // Если нет задержки, то создаем
                {
                    _currentSpawnDelay = _random.Next(_minFireworkSpawnDelay, _maxFireworkSpawnDelay); // Генерируем следующую задержку
                    _existingFireworks.Add(CreateNewFirework()); // Создаем и добавляем в список
                }
            }

            _currentSpawnDelay -= _tickRate * (_maxActiveFireworks - _existingFireworks.Count) / 2; // Каждый тик уменьшаем задержку

            foreach (Firework firework in _existingFireworks) // Проходимся по всем фейерверкам
            {
                if (!firework.Active) // Если они неактивны, то активируем
                {
                    firework.Activate(this); // Передаем родителя параметром
                }

                firework.Tick(_tickRate); // Делаем тик на каждом фейерверке
            }

            for (int i = 0; i < _existingFireworks.Count; i++) // Важно делать в обычном for, т.к. в foreach нельзя удалять элементы коллекции
            {
                var firework = _existingFireworks[i]; 
                if (firework.LeftLifeTime < 0) // Если время жизни фейерверка закончилось, то удаляем его
                {
                    firework.Dispose();
                    _existingFireworks.Remove(firework);
                }
            }
        }
    }
}
