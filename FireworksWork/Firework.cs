namespace FireworksWork
{
    public class Firework // Класс фейерверка
    {
        private readonly Random _random = new Random(); // Также нужен рандом

        private const int _minParticleSize = 3; // Минимум и максимум размеры частиц
        private const int _maxParticleSize = 12;
        private const int _roundDegrees = 360; // Углы в окружности

        private Point _centerPosition; // Параметры для фейерверка
        private int _startLifeTime;
        private int _radius;
        private PictureBox _fireworkPictureBox = null!; // Отдельный PictureBox под него
        private Graphics _graphics = null!; // Графика от PictureBox

        private List<Particle> _particles = new(); // Частицы фейерверка

        public bool Active { get; private set; } // Публичные поля фейеверка для проверок 

        public int LeftLifeTime { get; private set; }
        public Point CenterPosition { get => _centerPosition; set => _centerPosition = value; }
        public int Radius { get => _radius; set => _radius = value; }

        public Firework(Point centerPosition, int lifeTime, int radius, int particlesCount, List<Color> additionalColors, int tickRate)
        { // Конструктор
            CenterPosition = centerPosition;
            _startLifeTime = lifeTime;
            Radius = radius;
            var particleDegree = (double) _roundDegrees / particlesCount; // Расчет поворота для каждой частицы
            for (int i = 0; i < particlesCount; i++) 
            { // Создание частиц
                double direction = particleDegree * i * (Math.PI / 180); // Расчет траектории и перевод в радианы 
                int particleSize = _random.Next(_minParticleSize, _maxParticleSize);
                Color particleColor = RandomizeColor(additionalColors); // Выбираем рандомом цвет
                double speed = (double) (Radius / 3) / _startLifeTime * tickRate; // Расчет скорости
                float localParticleCenterPosition = radius / 2f; // Расчет локальной позиции центра 
                _particles.Add(new Particle(localParticleCenterPosition, localParticleCenterPosition, speed, direction, particleSize, particleColor)); // Создаем частицу
            }

            LeftLifeTime = _startLifeTime;
            Active = false;
        }

        public void Activate(Control parent) // Активация фейерверка
        {
            _fireworkPictureBox = new PictureBox(); // Создаем PictureBox под него
            int posX = CenterPosition.X - Radius / 2; // Расчет позиций для него
            int posY = CenterPosition.Y - Radius / 2;
            _fireworkPictureBox.Parent = parent; // Устанавливаем родителя (Form1)
            _fireworkPictureBox.SetBounds(posX, posY, Radius, Radius); // Устанавливаем размеры
            _graphics = _fireworkPictureBox.CreateGraphics(); // Создаем графику
            _fireworkPictureBox.BackColor = Color.Transparent; // Ставим прозрачный цвет фоном
            _fireworkPictureBox.Refresh();
            Active = true;
        }

        public void Tick(int tickRate) // Тик фейерверка
        {
            if (!Active) { // Если выключен, то ничего не делаем
                return;
            }

            LeftLifeTime -= tickRate; // Уменьшаем время жизни
            for (int i = 0; i < _particles.Count; i++) // Делаем логику для частиц. Обязательно в for
            {
                var particle = _particles[i]; 
                float opacityChange = (float) LeftLifeTime / _startLifeTime; // Отношение оставшегося времени жизни к изначальному для изменения прозрачности
                particle.Move(opacityChange); // Двигаем частицу
                particle.Draw(_graphics); // Отрисовываем
                if (particle.Opacity <= 0)
                {
                    _particles.Remove(particle); // Удаляем, если частица ушла в прозрачность
                }
            }
        }

        public void Dispose() { // Удаление фейерверка
            _fireworkPictureBox.Refresh(); // Чистим PictureBox
            _fireworkPictureBox.Dispose(); // Удаляем
        }

        private Color RandomizeColor(List<Color> colors) // Выбираем рандомный цвет из предложенных
        {
            int colorsCount = colors.Count;
            int choosenColorIndex = _random.Next(colorsCount - 1); // -1 чтобы выбрать существующий индекс
            return colors[choosenColorIndex];
        }
    }
}
