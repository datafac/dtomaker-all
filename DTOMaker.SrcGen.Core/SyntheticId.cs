namespace DTOMaker.SrcGen.Core
{
    internal readonly struct SyntheticId
    {
        private readonly int _id;
        public int Id => _id;
        public SyntheticId(int id) => _id = id & 0x0FFFFFFF;
        public SyntheticId Add(int id) => new SyntheticId((_id * 1031 + id) & 0x0FFFFFFF);
    }
}