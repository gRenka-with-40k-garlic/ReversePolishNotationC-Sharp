
using System;
using System.Collections.Generic;
using System.Numerics;


// Унарный минус по типу -(8+2) не поддерживается 
// div и mod реализованы как / и % соответсвенно


namespace RPN
{
    public class ReversePolishNotationCalc
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Введите данные:");
            string input = Console.ReadLine();

            ReversePolishNotationCalc expression = new ReversePolishNotationCalc();

            Console.WriteLine("Обратная польская запись:");
            string[] rpnExpression = expression.ToReversePolishNotation(input);
            foreach (string i in rpnExpression)
                Console.Write(i + " ");

            Console.WriteLine("\nРезультат:");
            Console.WriteLine(expression.result(input));
        }


        public ReversePolishNotationCalc()
        {
            operators = new List<string>(standart_operators);

        }
        private List<string> operators;
        private List<string> standart_operators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/", "^", "%" });

        private IEnumerable <string> Separate(string input) //IEnumerable предоставляет перечислитель, который поддерживает простой перебор элементов в введеной строке
        {
            int id = 0; //position
            while (id < input.Length)
            {
                string s = string.Empty + input[id];//String.Empty Поле. Представляет пустую строку. Это поле доступно только для чтения.
                if (!standart_operators.Contains(input[id].ToString())) //.Contains возвращает значение, указывающее, встречается ли указанная подстрока внутри этой строки.
                {
                    if (Char.IsDigit(input[id])) // Char.IsDigit это буквально Символ.ЯвляетсяЦифрой (true/false)
                        for (int i = id + 1; i < input.Length && (Char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++) 
                            s += input[i];
                    else if (Char.IsLetter(input[id])) //Char.IsLetter Значение true, если c является буквой; в противном случае — значение false.
                        for (int i = id + 1; i < input.Length && (Char.IsLetter(input[i]) || Char.IsDigit(input[i])); i++) 
                            s += input[i];
                }
                yield return s; //yield return нужен IEnumerable
                id += s.Length;
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
                case "%":
                    return 2;
                case "^":
                    return 3;
                default:
                    return 4;
            }
        }

        public string[] ToReversePolishNotation(string input)
        {
            List<string> SeparatedOperatorsList = new List<string>();
            Stack<string> stack = new Stack<string>(); //последним пришел - первым вышел
            foreach (string c in Separate(input))
            {
                if (operators.Contains(c)) ////.Contains это метод поиска конкретных значений в списке (операторов)
                {
                    if (stack.Count > 0 && !c.Equals("(")) //.Equals выполняет сравнение ссылок на объекты, возвращая булево значение
                    {
                        if (c.Equals(")"))
                        {
                            string s = stack.Pop(); //pop удаляет и возвращает верхний элемент стека
                            while (s != "(")
                            {
                                SeparatedOperatorsList.Add(s); //Добавляет объект в конец списка/листа
                                s = stack.Pop();
                            }
                        }
                        else if (GetPriority(c) > GetPriority(stack.Peek())) //.peek возвращает верхний элемент стека, не удаляя его 
                            stack.Push(c); //Добавление элемента в стек 
                        else
                        {
                            while (stack.Count > 0 && GetPriority(c) <= GetPriority(stack.Peek()))
                                SeparatedOperatorsList.Add(stack.Pop());
                            stack.Push(c);
                        }
                    }
                    else
                        stack.Push(c);
                }
                else
                    SeparatedOperatorsList.Add(c);
            }
            if (stack.Count > 0)
                foreach (string c in stack)
                    SeparatedOperatorsList.Add(c);

            return SeparatedOperatorsList.ToArray(); //ToArray Копирует элементы списка в новый массив.
        }
        public BigInteger result(string input) // BigInteger представляет произвольно большое целое число со знаком.
        {
            Stack<string> stack = new Stack<string>();
            Queue<string> queue = new Queue<string>(ToReversePolishNotation(input)); //первым поступил — первым обслужен
            string str = queue.Dequeue(); //.Dequeue удаляет и возвращает объект из начала очереди 
            while (queue.Count >= 0)
            {
                if (!operators.Contains(str))
                {
                    stack.Push(str);
                    str = queue.Dequeue();
                }
                else
                {
                    BigInteger sum = 0;
                    try
                    {
                        switch (str)
                        {

                            case "+":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());//.Parse тут преобразует строковое представление числа из стака в его эквивалент типа BigInteger.
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    sum = a + b;//Add(BigInteger, BigInteger) Складывает два значения BigInteger и возвращает результат.
                                    break;
                                }
                            case "-":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    sum = b - a; //BigInteger.Subtract(BigInteger, BigInteger)	Вычитает одно значение BigInteger из другого и возвращает результат.
                                    break;
                                }
                            case "*":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    sum = b * a; // можно еще вроде BigInteger.Multiply(b, a) 
                                    break;
                                }
                            case "/":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    //sum = b / a;	 
                                    sum = BigInteger.Divide(b, a); //Делит указанное значение BigInteger на другое указанное значение BigInteger, используя целочисленное деление.
                                    break;
                                }
                            case "^":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    sum = BigInteger.Pow(b, (int)a); // .Pow возводит в степень 
                                    break;
                                }
                            case "%":
                                {
                                    BigInteger a = BigInteger.Parse(stack.Pop());
                                    BigInteger b = BigInteger.Parse(stack.Pop());
                                    sum = b % a; 
                                    break;
                                }
                                // вообще у BigInt очень много методов, логарифмы там...
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    stack.Push(sum.ToString());
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