using System.Collections.Generic;

namespace nival_testing
{
    /**
     * Класс для накопления информации
     * о различных ошибках.
     */
    class Logger
    {
        private List<string> messages;

        public Logger()
        {
            messages = new List<string>();
        }

        public void AddMessage(string m)
        {
            messages.Add(m);
        }

        public List<string> GetMessages()
        {
            return messages;
        }
    }
}
