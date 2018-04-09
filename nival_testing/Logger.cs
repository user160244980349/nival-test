using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nival_testing
{
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
