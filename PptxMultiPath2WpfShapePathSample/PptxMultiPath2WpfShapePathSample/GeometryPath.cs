namespace PptxMultiPath2WpfShapePathSample
{
    public readonly struct GeometryPath
    {
        public GeometryPath(string path, FillMode fillMode = FillMode.Norm, bool isStroke = true)
        {
            Path = path;
            IsStroke = isStroke;
            FillMode = fillMode;
            IsFilled = fillMode is not FillMode.None;
        }

        /// <summary>
        /// 是否填充
        /// </summary>
        public bool IsFilled { get; }

        /// <summary>
        /// 是否有边框
        /// </summary>
        public bool IsStroke { get; }

        public FillMode FillMode { get; }

        /// <summary>
        /// Geometry的Path
        /// </summary>
        public string Path { get; }
    }

    public enum FillMode
    {
        /// <summary>
        ///Darken Path Fill
        /// </summary>
        Darken,

        /// <summary>
        /// Darken Path Fill Less
        /// </summary>
        DarkenLess,

        /// <summary>
        /// Lighten Path Fill
        /// </summary>
        Lighten,

        /// <summary>
        /// Lighten Path Fill Less
        /// </summary>
        LightenLess,

        /// <summary>
        /// None Path Fill
        /// </summary>
        None,

        /// <summary>
        /// Normal Path Fill
        /// </summary>
        Norm
    }
}
