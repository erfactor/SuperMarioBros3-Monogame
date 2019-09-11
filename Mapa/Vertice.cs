namespace SuperMarioBros
{
    public class Vertex
    {
        public Vector2 position;
        public Mode action;
        public Vertex Top = null;
        public Vertex Right = null;
        public Vertex Bot = null;
        public Vertex Left = null;

        public Vertex(Vector2 position, Mode action = Mode.nothing)
        {
            this.position = position;
            this.action = action;
        }
        public void SwitchAction()
        {
            if (action == Mode.nothing) return;
            else MainView.SwitchMode(action);
        }
    }

}
