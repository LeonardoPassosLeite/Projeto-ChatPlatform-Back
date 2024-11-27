namespace ChatPlatform.Services
{
    public class MessageBuilder
    {
        public static object CreateSystemMessage(string text)
        {
            return CreateChatMessage("system", "Sistema", text);
        }

        public static object CreateChatMessage(string type, string userName, string text)
        {
            return new
            {
                type,
                userName,
                text,
                timestamp = DateTime.Now.ToString("HH:mm:ss")
            };
        }
    }
}