using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System;

namespace nival_testing
{

    class CalculationReader
    {
        private string filepath;
        public Logger logger;
        public List<Calculation> calculations;

        public CalculationReader(string filepath)
        {
            this.filepath = filepath;
            logger = new Logger();
            calculations = new List<Calculation>();
        }
        
        public void ParseCalculations()
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(filepath);

                while (reader.Read())
                {
                    if (reader.Name == "folder" && reader.GetAttribute("name") == "calculations")
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "folder" && reader.GetAttribute("name") == "calculation")
                            {
                                ParseCalculation(reader);
                            }
                        }
                    }
                } 
            }
            catch (XmlException e)
            {
                logger.AddMessage("Фатальная ошибка с структуре xml-файла:\n   " + e.Message);
            } 

        }

        private void ParseCalculation(XmlTextReader reader)
        {
            var newCalculation = new Calculation();

            bool uidValid = false;
            bool operandValid = false;
            bool modValid = false;

            while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "str")
                {
                    if (reader.GetAttribute("name") == "uid")
                    {
                        if (UidValidation(reader))
                        {
                            uidValid = true;
                            newCalculation.uid = ParseUid(reader);
                        }
                    }
                    else
                    {
                        if (reader.GetAttribute("name") == "operand")
                        {
                            if (OperandValidation(reader))
                            {
                                operandValid = true;
                                newCalculation.operand = ParseOperand(reader);
                            }
                        }
                    }
                }
                else
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "int")
                    {
                        if (ModValidation(reader))
                        {
                            modValid = true;
                            newCalculation.mod = ParseMod(reader);
                        }
                    }
                }
            }
            if (uidValid && operandValid && modValid)
                calculations.Add(newCalculation);
        }

        private string ParseUid(XmlTextReader reader)
        {
            return reader.GetAttribute("value");
        }

        private Operand ParseOperand(XmlTextReader reader)
        {
            Operand result = Operand.add;
            switch (reader.GetAttribute("value"))
            {
                case "subtract":
                    result = Operand.subtract;
                    break;
                case "multiply":
                    result = Operand.multiply;
                    break;
                case "divide":
                    result = Operand.divide;
                    break;
            }
            return result;
        }

        private int ParseMod(XmlTextReader reader)
        {
            return int.Parse(reader.GetAttribute("value"));
        }

        private bool UidValidation(XmlTextReader reader)
        {
            bool validity = false;

            if (reader.GetAttribute("value") != null)
            {
                validity = true;
            }

            return validity;
        }

        private bool OperandValidation(XmlTextReader reader)
        {
            bool validity = false;

            if (reader.GetAttribute("value") != null)
            {
                switch (reader.GetAttribute("value"))
                {
                    default:
                        logger.AddMessage("Неверный атрибут name тэга str, должен быть <str name=\"operand\" value=\" ??? \"/>" +
                            " где ??? - строка определющая оператор, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");
                        break;
                    case "add":
                        validity = true;
                        break;
                    case "subtract":
                        validity = true;
                        break;
                    case "multiply":
                        validity = true;
                        break;
                    case "divide":
                        validity = true;
                        break;
                }
            }

            return validity;
        }

        private bool ModValidation(XmlTextReader reader)
        {
            bool validity = false;

            if (reader.GetAttribute("value") != null)
            {
                bool isDigit = int.TryParse(reader.GetAttribute("value"), out int result);
                if (isDigit)
                {
                    validity = true;
                }
            }

            return validity;
        }
    }
}
