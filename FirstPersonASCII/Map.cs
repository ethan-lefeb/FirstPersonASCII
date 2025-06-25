public class Map
{
    public int Width { get; }
    public int Height { get; }

    private char[] _mapData;

    public Map(int width, int height)
    {
        Width = width;
        Height = height;
        _mapData = new char[width * height];
        GenerateDefault();
    }

    public void GenerateDefault()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    _mapData[y * Width + x] = '#'; // Border walls
                else
                    _mapData[y * Width + x] = '.'; // Floor
            }
        }
        for (int y = 4; y < 10; y++)
            _mapData[y * Width + 5] = '#';
        for (int x = 8; x < 12; x++)
            _mapData[7 * Width + x] = '#';
    }

    public void GenerateRandom()
    {
        Random rand = new Random();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    _mapData[y * Width + x] = '#';
                else
                    _mapData[y * Width + x] = rand.NextDouble() < 0.2 ? '#' : '.';
            }
        }
    }

    public bool IsWall(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return true; // Treat out-of-bounds as wall
        return _mapData[y * Width + x] == '#';
    }

    public char this[int x, int y] => _mapData[y * Width + x];

    public void DrawToBuffer(char[] screen, int screenWidth, int offsetX, int offsetY)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                screen[(y + offsetY) * screenWidth + (x + offsetX)] = _mapData[y * Width + x];
            }
        }
    }

    public void Set(int x, int y, char value)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            _mapData[y * Width + x] = value;
    }
}
