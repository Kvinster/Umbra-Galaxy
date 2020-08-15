namespace STP.Config {
    public sealed class SingleDialogText : BaseDialogText {
        public override string Text { get; }

        public SingleDialogText(string text) {
            Text = text;
        }
    }
}
