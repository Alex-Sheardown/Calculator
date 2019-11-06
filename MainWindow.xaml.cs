using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace calculator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public static string evaluatePostfix(string exp)
		{
			// create a stack  
			Stack<double> stack = new Stack<double>();



			// Scan all characters one by one  
			for (int i = 0; i < exp.Length; i++)
			{
				char c = exp[i];

				if (c == ' ')
				{
					continue;
				}

				// If the scanned character is an   
				// operand (number here),extract  
				// the number. Push it to the stack.  
				else if (char.IsDigit(c))
				{
					int keepTrack = 0;
					string n = "";

					// extract the characters and  
					// store it in num  
					while (char.IsDigit(c) || c == '.')
					{


						if(c == '.')
						{
							keepTrack++;
							if(keepTrack > 1)
							{
								return "Syntax Error";
							}
						}

						n += c;
						i++;
						try
						{
							c = exp[i];
						}
						catch
						{
							return "Syntax Error";
						}
					}
					i--;

					// push the number in stack 
					try
					{
						stack.Push(Convert.ToDouble(n));
					}
					catch
					{
						return "Syntax Error";
					}
				}
				else if (stack.Count  == 1) {

					double val1 = stack.Pop();
					if (c == '-')
					{
						
						stack.Push(-val1);
					}

					if (c == '*')
					{
						
						stack.Push(val1);
					}

				}
				// If the scanned character is  
				// an operator, pop two elements  
				// from stack apply the operator

				else
				{
					double val1 = 0;
					double val2 = 0;
					try
					{
						val1 = stack.Pop();
						val2 = stack.Pop();
					}
					catch
					{
						return "Syntax Error";
					}
					

					switch (c)
					{
						case '+':
							stack.Push(val2 + val1);
							break;

						case '-':
							stack.Push(val2 - val1);
							break;

						case '/':

							if(val1 == 0)
							{
								return "Undefined";
							}

							stack.Push(val2 / val1);
							break;

						case '*':
							stack.Push(val2 * val1);
							break;

						case '^':
							stack.Push(Math.Pow(val2, val1));
							break;
					}
				}
			}
			try
			{
				double hold = stack.Pop();
				if(hold >= double.MaxValue)
				{
					return "Calculation is to big";
				}
				double answer = Math.Round(hold, 2);
				return hold.ToString();
			}
			catch(Exception e)
			{
				return "Error";
			}
		}

		public static string InfixToPostfix(string exp)
		{
			// initializing empty String for result
			String result = String.Empty;

			// initializing empty stack
			Stack<char> stack = new Stack<char>();
			

			for (int i = 0; i < exp.Length; ++i)
			{
				char c = exp[i];

				// If the scanned character is an operand, add it to output.
				if (Char.IsLetterOrDigit(c) && i + 1 < exp.Length)
				{
					char d = exp[i + 1];
					if (Char.IsLetterOrDigit(d))
					{
						result += c;
					}
					else if (d == '.')
					{
						result += c;
					}
					else
					{
						result += c + " ";
					}

				}
				
				else if (c == '.' )
				{

					if (i - 1 >= 0 && i + 1 < exp.Length)
					{
						char b = exp[i - 1];
						char d = exp[i + 1];
						if (!Char.IsLetterOrDigit(d) && !Char.IsLetterOrDigit(b))
						{
							return "Syntax Error";
						}
					}
					
					//if{

						if (i - 1 > 0)
						{

							char d = exp[i - 1];
							if (!Char.IsLetterOrDigit(d))
							{
								result += 0;
							}

						}

						else
						{
							//result += 0;
						}

						if (i + 1 < exp.Length)
						{

							char d = exp[i + 1];
							if (Char.IsLetterOrDigit(d))
							{
								result += c;
							}
							else
							{
								result += c + "0 ";
								//exp.Insert(i,".0");
							}
						}
					//}
				}

				else if (Char.IsLetterOrDigit(c))
				{

					result += c + " ";

				}
				
				// If the scanned character is an '(', push it to the stack.
				else if (c == '(')
				{
					if((i - 1) >= 0){
						char d = exp[i - 1];
						
						if (Char.IsLetterOrDigit(d) || d == ' ' || d == '.') {
							while (stack.Count != 0 && Prec('*') <= Prec(stack.Peek()))
								result += stack.Pop() + " ";
							stack.Push('*');
						}
					}
					stack.Push(c);
				
				}
				//  If the scanned character is an ')', pop and output from the stack 
				// until an '(' is encountered.
				else if (c == ')')
				{
					while (stack.Count != 0 && stack.Peek() != '(')
						result += stack.Pop() + " ";

					if (stack.Count != 0 && stack.Peek() != '(')
						return "Invalid Expression"; // invalid expression                
					else
					{
						try
						{
							stack.Pop();
						}
						catch
						{
							return "Invalid Expression";
						}

						if (i + 1 < exp.Length)
						{
							
							char d = exp[i + 1];
							if (Char.IsLetterOrDigit(d) || d == '(')
							{

								
								while (stack.Count != 0 && Prec('*') <= Prec(stack.Peek()))
									result += stack.Pop() + " ";
								stack.Push('*');
								
							}
						}
						//stack.Pop();
					}
				}
				else // an operator is encountered
				{
					while (stack.Count != 0 && Prec(c) <= Prec(stack.Peek()))
						result += stack.Pop() + " ";
					stack.Push(c);
				}

			}

			// pop all the operators from the stack
			while (stack.Count != 0)
				result += stack.Pop();

			return result;
		}

		// A utility function to return precedence of a given operator
		// Higher returned value means higher precedence
		public static int Prec(char ch)
		{
			switch (ch)
			{
				case '+':
				case '-':
					return 1;

				case '*':
				case '/':
					return 2;

				case '^':
					return 3;
			}
			return -1;
		}

		private void Button_Click_E(object sender, RoutedEventArgs e)
		{
			bool check1 = string.Equals(calculatorBox.Text, "Error");
			bool check2 = string.Equals(calculatorBox.Text, "Undefined");
			bool check3 = string.Equals(calculatorBox.Text, "Calculation is to big");
			bool check4 = string.Equals(calculatorBox.Text, "Syntax Error");

			if (check1 || check2 || check3 || check4)
			{
				calculatorBox.Text = "";
			}
			
			
				
			
			else
			{
				string hold = calculatorBox.Text;
				hold = hold.Replace(" ", String.Empty); ;
				calculatorBox.Text = hold;
				hold = InfixToPostfix(hold);
				calculatorBox.Text = hold;
				calculatorBox.Text = evaluatePostfix(calculatorBox.Text);
				//calculatorBox.Text = answer.ToString();
			}
		}


		private void addToTextBox(char value)
		{
			bool check1 = string.Equals(calculatorBox.Text, "Error");
			bool check2 = string.Equals(calculatorBox.Text, "Undefined");
			bool check3 = string.Equals(calculatorBox.Text, "Calculation is to big");
			bool check4 = string.Equals(calculatorBox.Text, "Syntax Error");

			if (check1 || check2 || check3 || check4)
			{
				calculatorBox.Text = "";
			}

			if (calculatorBox.Text.Length < 25)
			{
				calculatorBox.Text += value;
			}
		}

		private void Button_Click_Back(object sender, RoutedEventArgs e)
		{
			String str = calculatorBox.Text;
			if (!String.IsNullOrEmpty(str))
			{
				calculatorBox.Text = str.TrimEnd(str[str.Length - 1]);
			}
		}

		private void Button_Click_C(object sender, RoutedEventArgs e)
		{
			calculatorBox.Text = "";
		}

		

		//All the numbers and other characters
		private void Button_Click_0(object sender, RoutedEventArgs e)
		{
			addToTextBox('0');
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			addToTextBox('1');
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			addToTextBox('2');
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			addToTextBox('3');
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			addToTextBox('4');
		}

		private void Button_Click_5(object sender, RoutedEventArgs e)
		{
			addToTextBox('5');
		}

		private void Button_Click_6(object sender, RoutedEventArgs e)
		{
			addToTextBox('6');
		}

		private void Button_Click_7(object sender, RoutedEventArgs e)
		{
			addToTextBox('7');
		}

		private void Button_Click_8(object sender, RoutedEventArgs e)
		{
			addToTextBox('8');
		}

		private void Button_Click_9(object sender, RoutedEventArgs e)
		{
			addToTextBox('9');
		}

		private void Button_Click_P(object sender, RoutedEventArgs e)
		{
			addToTextBox('^');
		}

		private void Button_Click_D(object sender, RoutedEventArgs e)
		{
			addToTextBox('/');
		}

		private void Button_Click_M(object sender, RoutedEventArgs e)
		{
			addToTextBox('*');
		}

		private void Button_Click_S(object sender, RoutedEventArgs e)
		{
			addToTextBox('-');
		}

		private void Button_Click_A(object sender, RoutedEventArgs e)
		{
			addToTextBox('+');
		}

		private void Button_Click_Dot(object sender, RoutedEventArgs e)
		{
			addToTextBox('.');
		}

		private void Button_Click_LB(object sender, RoutedEventArgs e)
		{
			addToTextBox('(');
		}

		private void Button_Click_RB(object sender, RoutedEventArgs e)
		{
			addToTextBox(')');
		}
	}
}
