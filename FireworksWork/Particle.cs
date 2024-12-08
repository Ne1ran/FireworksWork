namespace FireworksWork
{
    public class Particle // Класс частицы
    {
        private const int _startOpacity = 255; // Начальная непрозрачность

        private double _speed; // Параметры для частицы + кисть
        private double _direction;
        private int _size;
        private SolidBrush _brush;
        private float _xPos;
        private float _yPos;

        public float Opacity { get; private set; } // Поле для прозрачности

        public Particle(float x, float y, double speed, double direction, int size, Color color)
        { // Конструктор
            _speed = speed;
            _direction = direction;
            _size = size;
            _brush = new SolidBrush(color); // Создаем кисть по цвету
            _xPos = x;
            _yPos = y;
            Opacity = _startOpacity;
        }

        public void Move(float opacityChangePercent)
        {
            _xPos += (float) (Math.Cos(_direction) * _speed); // Делаем движение по X и Y
            _yPos += (float) (Math.Sin(_direction) * _speed);
            Opacity = _startOpacity * opacityChangePercent; // Уменьшаем прозрачность для исчезновения частицы
            _brush.Color = Color.FromArgb(Math.Max(0, (int) Opacity), _brush.Color); // Меняен цвет на более прозрачный
        }

        public void Draw(Graphics graphics)
        {
            graphics.FillEllipse(_brush, _xPos - _size / 2f, _yPos - _size / 2f, _size, _size); // Отрисовка частицы
        }
    }
}
