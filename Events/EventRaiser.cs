
namespace ApplicationTracker.Events
{
    public static class EventRaiser
    {
        // Action<string>: what to do with a failure report. Printing is the default, and it can be replaced.
        public static Action<string> ReportFailure = message => Console.WriteLine(message);

        public static void Raise<TEventArgs>(EventHandler<TEventArgs> eventHandler, object sender, TEventArgs args)
        {
            if (eventHandler == null) return;                        // nobody is subscribed

            foreach (Delegate subscriber in eventHandler.GetInvocationList())   // one delegate per subscriber
            {
                try
                {
                    ((EventHandler<TEventArgs>)subscriber)(sender, args);
                }
                catch (Exception ex)
                {
                    // One subscriber failed: report it and carry on with the next one
                    ReportFailure($"[Notification failed] {subscriber.Method.Name}: {ex.Message}");
                }
            }
        }
    }
}