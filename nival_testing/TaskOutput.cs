using System.Collections.Generic;

namespace nival_testing
{
    /**
     * Класс контейнер для передачи 
     * результатов работы потока 
     * обработки файла в основной 
     * поток программы.
     */
    class TaskOutput
    {
        public string filepath;
        public int result;
        public int completed;
        public List<string> logs;
    }
}
