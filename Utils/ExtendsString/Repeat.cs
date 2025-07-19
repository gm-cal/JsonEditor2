namespace Utils{
    public static partial class ExtendsString{
        public static string Repeat(this string str, int count){
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));
            return string.Concat(Enumerable.Repeat(str, count));
        }
    }
}