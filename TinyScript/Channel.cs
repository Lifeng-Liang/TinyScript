namespace TinyScript
{
    public class Channel
    {
        public virtual void Print(object msg)
        {
            Print(msg.ToString());
        }

        public virtual void Print(string message, params object[] args)
        {
            System.Console.WriteLine(message, args);
        }
    }
}
