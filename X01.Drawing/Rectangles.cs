namespace X01
{
    public sealed class Rectangles
    {
        private readonly List<Rectangle> _rects = new();
        public void Append(params Rectangle[] rects)
        {
            _rects.AddRange(rects);
        }
        public List<Rectangle> Union()
        {
            Rectangle firstRect;
            foreach (Rectangle x in _rects.Skip(1))
            {
                // firstRect.
            }
            return (_rects);
        }
    }
}
