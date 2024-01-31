
using System;
using System.Collections.Generic;
using System.Numerics;

// я мучалась над реализациями, и переписывала порядка 20 раз, читая ошибки чуть ли не на индийских форумах
// на этом мои полномочия все 
// это лучшая реализация не считая ту что могла в другие системы счисления, но у нее была проблема 
// а еще унарный минус не поддерживается держувкурсе 


namespace RPNtry51
{
    public class RPN
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Введите данные:");
            string input = Console.ReadLine();

            RPN expression = new RPN();

            Console.WriteLine("Обратная польская запись:");
            string[] rpnExpression = expression.ToRPN(input);
            foreach (string token in rpnExpression)
                Console.Write(token + " ");

            Console.WriteLine("\nРезультат:");
            Console.WriteLine(expression.result(input));
        }


        public RPN()
        {
            operators = new List<string>(standart_operators);

        }
        private List<string> operators;
        private List<string> standart_operators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/", "^" });

        private IEnumerable <string> Separate(string input) //с IEnumerable, метод Separate разбивает введенное пользователем выражение на отдельные элементы и возвращает их в виде последовательности строк
        {
            int pos = 0;
            while (pos < input.Length)
            {
                string s = string.Empty + input[pos];
                if (!standart_operators.Contains(input[pos].ToString())) //.Contains это метод поиска конкретных значений в коллекции/листе/массиве
                {
                    if (Char.IsDigit(input[pos])) // Char.IsDigit это буквально Символ.ЯвляетсяЦифрой
                        for (int i = pos + 1; i < input.Length &&
                            (Char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++)
                            s += input[i];
                    else if (Char.IsLetter(input[pos])) //Char.IsLetter буквально Символ.ЯвляетсяБуквой
                        for (int i = pos + 1; i < input.Length &&
                            (Char.IsLetter(input[i]) || Char.IsDigit(input[i])); i++)
                            s += input[i];
                }
                yield return s;
                pos += s.Length;
            }
        }


        private byte GetPriority(string s)
        {
            switch (s)
            {
                case "(":
                case ")":
                    return 0;
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                    return 3;
                default:
                    return 4;
            }
        }

        public string[] ToRPN(string input)
        {
            List<string> outputSeparated = new List<string>();
            Stack<string> stack = new Stack<string>();
            foreach (string c in Separate(input))
            {
                if (operators.Contains(c))
                {
                    if (stack.Count > 0 && !c.Equals("(")) //.Equals выполняет сравнение ссылок на объекты, возвращая булево значение
                    {
                        if (c.Equals(")"))
                        {
                            string s = stack.Pop();
                            while (s != "(")
                            {
                                outputSeparated.Add(s); //add и pop вроде уже прошли,думаю мне не надо помечать,понимаю ли я что это такое?
                                s = stack.Pop();
                            }
                        }
                        else if (GetPriority(c) > GetPriority(stack.Peek())) //.peek возвращает верхний элемент стека, не удаляя его 
                            stack.Push(c); //push тоже прошли
                        else
                        {
                            while (stack.Count > 0 && GetPriority(c) <= GetPriority(stack.Peek()))
                                outputSeparated.Add(stack.Pop());
                            stack.Push(c);
                        }
                    }
                    else
                        stack.Push(c);
                }
                else
                    outputSeparated.Add(c);
            }
            if (stack.Count > 0)
                foreach (string c in stack)
                    outputSeparated.Add(c);

            return outputSeparated.ToArray();
        }
        public BigInteger result(string input)
        {
            Stack<string> stack = new Stack<string>();
            Queue<string> queue = new Queue<string>(ToRPN(input)); //мы ведь проходили очередь? ну это почти стек но не стек там дааа...
            string str = queue.Dequeue(); //.Dequeue удаляет объект из начала очереди и возвращает его
            while (queue.Count >= 0)
            {
                if (!operators.Contains(str))
                {
                    stack.Push(str);
                    str = queue.Dequeue();
                }
                else
                {
                    BigInteger summ = 0;
                    try
                    {
                        switch (str)
                        {

                            case "+":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    summ = a + b;
                                    break;
                                }
                            case "-":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    summ = b - a;
                                    break;
                                }
                            case "*":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    summ = b * a;
                                    break;
                                }
                            case "/":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    summ = b / a;
                                    break;
                                }
                            case "^":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    summ = BigInteger.Pow(b, (int)a);
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    stack.Push(summ.ToString());
                    if (queue.Count > 0)
                        str = queue.Dequeue();
                    else
                        break;
                }

            }
            return BigInteger.Parse(stack.Pop());
        }
    }
    
}