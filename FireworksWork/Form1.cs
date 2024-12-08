using Timer = System.Windows.Forms.Timer;

namespace FireworksWork
{
    public partial class Form1 : Form
    {
        private readonly Random _random = new Random(); // ������� ������ ��� ��������� ��������� �����
        private List<Firework> _existingFireworks = new(); // ������ ������������ �����������

        private const int _tickRate = 60; // ������� ���������� (� ��)

        private const int _rgbMin = 0; // ������� � �������� RGB ��� ��������� ������
        private const int _rgbMax = 255;

        private const int _minAdditionalColors = 10; // ������� � �������� ������ � �����������
        private const int _maxAdditionalColors = 30;

        private const int _borderOffsetX = 150; // ������� �� X � Y �� �����
        private const int _borderOffsetY = 125;

        private const int _minLifeTime = 1000; // ������� � �������� ������� ����� ����������
        private const int _maxLifeTime = 5000;

        private const int _minRadius = 200; // ������� � �������� ������� ����������
        private const int _maxRadius = 300;

        private const int _minParticles = 8; // ������� � �������� ������ � ���
        private const int _maxParticles = 24;

        private const int _maxActiveFireworks = 7; // �������� ���������� ����������� �� ������

        private const int _minFireworkSpawnDelay = 700; // ������� � �������� �������� ��� �������� ������ ���������� 
        private const int _maxFireworkSpawnDelay = 1700;

        private int _minDrawX; // ����������� � ������������ ���������� ��� �������� ����������
        private int _minDrawY;
        private int _maxDrawX;
        private int _maxDrawY;
        private int _currentSpawnDelay; // ������� �������� ��� �������� ����������
        private Timer _animationTimer = null!; // ������

        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true); // ������ ����������� ��������� ���������� ��������
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _animationTimer = new Timer(); // �������������� ������. ������ ��� �������� (�������) � ������������� �� ����� ����
            _animationTimer.Interval = _tickRate;
            _animationTimer.Tick += AnimationTimer_Tick;

            Size windowSize = Size; // ����� ������� ���� ���������� � ������������ ����������� � ������������ ���������� ��� �������� ����������
            _minDrawX = _borderOffsetX;
            _minDrawY = _borderOffsetY;
            _maxDrawX = windowSize.Width - _borderOffsetX;
            _maxDrawY = windowSize.Height - _borderOffsetY;

            InitializeFireworks(); // ��������� ������������� �����������. ����� ������ � �������� ������ �� �������
            await Task.Delay(100); // ��������� ��������
            _animationTimer.Start(); // ������ �������
        }

        private void InitializeFireworks()
        {
            int fireworksCountStart = _random.Next(_maxActiveFireworks); // ���������� ����� ����������� �����������
            for (int i = 0; i < fireworksCountStart; i++) {
                _existingFireworks.Add(CreateNewFirework()); // ������� �� � ��������� � ������
            }
        }

        private Firework CreateNewFirework()
        {
            int radius = _random.Next(_minRadius, _maxRadius); // ���������� ������ ��������� ����� ������
            int lifeTime = _random.Next(_minLifeTime, _maxLifeTime);
            int randomX = _random.Next(_minDrawX, _maxDrawX);
            int randomY = _random.Next(_minDrawY, _maxDrawY);
            int particles = _random.Next(_minParticles, _maxParticles);
            int additionalColorsCount = _random.Next(_minAdditionalColors, _maxAdditionalColors);
            List<Color> additionalColors = new List<Color>(additionalColorsCount);
            for (int i = 0; i < additionalColorsCount; i++)
            {
                additionalColors.Add(GenerateColor()); // ���������� �����
            }

            Point centerPosition = new Point(randomX, randomY);
            var firework = new Firework(centerPosition, lifeTime, radius, particles, additionalColors, _tickRate); // ������� ����� ����������
            return firework;
        }

        private Color GenerateColor()
        {
            int r = _random.Next(_rgbMin, _rgbMax); // ���������� RGB ����� ���������� ����� ������
            int g = _random.Next(_rgbMin, _rgbMax);
            int b = _random.Next(_rgbMin, _rgbMax);
            return Color.FromArgb(r, g, b); // ������� ����
        }

        private void AnimationTimer_Tick(object? sender, EventArgs? e)
        {
            if (_existingFireworks.Count < _maxActiveFireworks) {  // �������� �� ����������� �������� ������ ����������
                if (_currentSpawnDelay < 0) // ���� ��� ��������, �� �������
                {
                    _currentSpawnDelay = _random.Next(_minFireworkSpawnDelay, _maxFireworkSpawnDelay); // ���������� ��������� ��������
                    _existingFireworks.Add(CreateNewFirework()); // ������� � ��������� � ������
                }
            }

            _currentSpawnDelay -= _tickRate * (_maxActiveFireworks - _existingFireworks.Count) / 2; // ������ ��� ��������� ��������

            foreach (Firework firework in _existingFireworks) // ���������� �� ���� �����������
            {
                if (!firework.Active) // ���� ��� ���������, �� ����������
                {
                    firework.Activate(this); // �������� �������� ����������
                }

                firework.Tick(_tickRate); // ������ ��� �� ������ ����������
            }

            for (int i = 0; i < _existingFireworks.Count; i++) // ����� ������ � ������� for, �.�. � foreach ������ ������� �������� ���������
            {
                var firework = _existingFireworks[i]; 
                if (firework.LeftLifeTime < 0) // ���� ����� ����� ���������� �����������, �� ������� ���
                {
                    firework.Dispose();
                    _existingFireworks.Remove(firework);
                }
            }
        }
    }
}
