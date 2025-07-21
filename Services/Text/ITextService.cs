namespace Services{
    public interface ITextService{
        void ModifySelection(string[] input, bool indent, out string[] output);
    }
}
