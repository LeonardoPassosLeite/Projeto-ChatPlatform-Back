namespace ChatPlatform.Entities
{
    public class ChatMessage
    {
        public string Type { get; set; } = string.Empty; // Tipo da mensagem (e.g., "system", "message")
        public string UserName { get; set; } = string.Empty; // Nome do usuário que enviou a mensagem
        public string Text { get; set; } = string.Empty; // Texto da mensagem
        public string Timestamp { get; set; } = DateTime.Now.ToString("HH:mm:ss"); // Horário da mensagem
    }
}
