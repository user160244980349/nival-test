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
                    if (reader.NodeType != XmlNodeType.Element)
                        continue;

                    if (reader.Name != "folder")
                        logger.AddMessage("Непредвиденный элемент <" + reader.Name + " ... >, ожидался <folder name=\"calculations\">, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

                    if (reader.GetAttribute("name") != "calculations")
                        logger.AddMessage("Непредвиденный элемент <" + reader.Name + " name=\"" + reader.GetAttribute("name") + "\", ожидался <folder name=\"calculations\">, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

                    while (reader.Read())
                    {
                        if (reader.NodeType != XmlNodeType.Element)
                            continue;

                        if (reader.Name != "folder")
                            logger.AddMessage("Непредвиденный элемент <" + reader.Name + " ... >, ожидался <folder name=\"calculation\">, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

                        if (reader.GetAttribute("name") != "calculation")
                            logger.AddMessage("Непредвиденный элемент <" + reader.Name + " name=\"" + reader.GetAttribute("name") + "\", ожидался <folder name=\"calculation\">, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

                        ParseCalculation(reader);
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

            bool uidFound = false;
            bool operandFound = false;
            bool modFound = false;

            while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType != XmlNodeType.Element)
                    continue;

                if (reader.Name == "str")
                {
                    if (reader.GetAttribute("name") == null)
                        logger.AddMessage("Пропущен атрибут name, <str name=\" ??? \" ... />, где ??? - uid или operand, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

                    if (reader.GetAttribute("name") == "uid")
                    {
                        uidFound = true;
                        if (UidValidation(reader))
                        {
                            uidValid = true;
                            newCalculation.uid = ParseUid(reader);
                        }
                    }

                    if (reader.GetAttribute("name") == "operand")
                    {
                        operandFound = true;
                        if (OperandValidation(reader))
                        {
                            operandValid = true;
                            newCalculation.operand = ParseOperand(reader);
                        }
                    }
                }

                if (reader.Name == "int")
                {
                    if (reader.GetAttribute("name") == null)
                        logger.AddMessage("Пропущен атрибут name, <int name=\"mod\" ... />, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

                    if (reader.GetAttribute("name") == "mod")
                    {
                        modFound = true;
                        if (ModValidation(reader))
                        {
                            modValid = true;
                            newCalculation.mod = ParseMod(reader);
                        }
                    }
                }
            }

            if (!uidFound)
                logger.AddMessage("Пропущен <str name=\"uid\" value=\" ??? \"/>, где ??? - строка идентификатор, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");
            if (!operandFound)
                logger.AddMessage("Пропущен <str name=\"operand\" value=\" ??? \"/>, где ??? - строка, определющая оператор, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");
            if (!modFound)
                logger.AddMessage("Пропущен <int name=\"mod\" value=\" ??? \"/>, где ??? - целое число, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

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
                validity = true;
            else
                logger.AddMessage("Пропущен атрибут value, <str name=\"uid\" value=\" ??? \"/>, где ??? - строка идентификатор, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

            return validity;
        }

        private bool OperandValidation(XmlTextReader reader)
        {
            bool validity = false;

            if (reader.GetAttribute("value") == null)
            {
                logger.AddMessage("Пропущен атрибут value, <str name=\"operand\" value=\" ??? \"/>, где ??? - строка, определющая оператор, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");
                return validity;
            }

            switch (reader.GetAttribute("value"))
            {
                default:
                    logger.AddMessage("Неверный атрибут value, <str name=\"operand\" value=\" ??? \"/>, где ??? - строка, определющая оператор, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");
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

            return validity;
        }

        private bool ModValidation(XmlTextReader reader)
        {
            bool validity = false;

            if (reader.GetAttribute("value") == null)
            {
                logger.AddMessage("Пропущен атрибут value, <int name=\"mod\" value=\" ??? \"/>, где ??? - целое число, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");
                return validity;
            }

            bool isDigit = int.TryParse(reader.GetAttribute("value"), out int result);

            if (isDigit)
                validity = true;
            else
                logger.AddMessage("value не является числом, <int name=\"mod\" value=\" ??? \"/>, где ??? - целое число, строка " + reader.LineNumber + ", позиция " + reader.LinePosition + ".");

            return validity;
        }
    }
}
