namespace Sushi.Mediakiwi.Framework
{
    public class ModuleExecutionResult
    {
        /// <summary>
        /// Was the execute SuccessFull ?
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The output to be shown in the Wim Notification Area.
        /// If IsSuccess is TRUE, this will output in the notification,
        /// else it will output in the error
        /// </summary>
        public string WimNotificationOutput { get; set; }

    }
}
