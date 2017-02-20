using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Construct_Truth_Table
{
	class Program
	{
		
		internal class DataGenerator{
			
			private List<string[]> expressions = new List<string[]>();
			
			private string[] vars;
						
			private Constructor constructor;
			
			
			public DataGenerator(string[] exprs, string[] vars){
								
				foreach (string expr in exprs)
				{
					this.expressions.Add(expr.Split(null));
					
				}
				
				this.vars = vars;
				
				this.init();
				
			}
			
			
			
			private void init(){
				
				this.constructor = new Constructor(this.vars);
				
			}
			
			public void printTruthTable(){
				
				this.printTitles();
				
				this.printValues();
				
			}
			
			private void printTitles(){
				
				// write the vars
				foreach (string variable in this.vars)
				{
					Console.Write("{0} | ", variable);
				}
				
				// write the expression
				foreach (string[] expression in this.expressions)
				{
					Console.Write("{0} | ", String.Join(" ",expression));
				}
				
				Console.WriteLine();
				
			}
			
			private void printValues(){
				
				Analyzer analyzer;
				
				foreach (var dic in this.constructor.getTable())
				{
					
					// write variable's value
					foreach (string variable in this.vars)
					{
						
						Console.Write("{0} | ", dic[variable] == 1 ? "T" : "F");
						
					}
				
					
					// write the calculated final values
					foreach (string[] expression in this.expressions)
					{
						analyzer = new Analyzer(expression,dic);
						
						Console.Write("{0} | ", analyzer.analyze() == "true" ? "T":"F");

					}
					
					Console.WriteLine();
				}
				
			}
			
		}
		
		
		internal class Analyzer
		{
			protected List<string> operators = new List<string>()
			{
			   "implies","and", "or", "xor"
			};

			protected List<string> booleans = new List<string>()
			{
				"true","false"
			};

			protected List<string> expression;

			protected Dictionary<string,int> option;

			public Analyzer(string[] expr, Dictionary<string, int> option)
			{
				this.expression = expr.ToList();

				this.option = option;

				this.init();

			}

			protected void init()
			{

				var newExpression = new List<string>(this.expression);

				for (int i = 0; i < this.expression.Count; i++)
				{
					if (this.option.Keys.Contains(this.expression[i]))
					{
						newExpression[i] = this.option[this.expression[i]] == 1 ? "true" : "false";
					}
				}

				this.expression = newExpression;

			}

			public string analyze()
			{
				int[] enclosedSubExpr = innerClosureBracket(this.expression);

				if (!isSubExpr(enclosedSubExpr))
				{
					return this.evaluate(this.expression);
				}
				
				int startIndex = enclosedSubExpr[0];
				
				int endIndex = enclosedSubExpr[1];
				
				int count = endIndex - startIndex;
				
				this.expression.Insert(startIndex,this.evaluate(this.expression.GetRange(startIndex,count)));

				this.expression.RemoveRange(startIndex + 1, count + 1);
				
				return this.analyze();
				
			}
			
			protected string evaluate(List<string>expr){
				
				for (int i = 0; i < expr.Count; i++)
				{
					if (this.isOperator(expr[i]))
					{
						string val = this.resultOf(expr,expr[i],i);
						
						return val;
					}
				}
			
				
				return "null";
				
			}
			
			protected bool isSubExpr(int[] enclosedSubExpr){
				
				return enclosedSubExpr[1] != 0;
				
			}
			
			
			protected int[] innerClosureBracket(List<string> expr){
				
				int startIndex = 0; int lastIndex = 0;
				
				for (int i = 0; i < expr.Count; i++)
				{
					if (expr[i] == "(")
					{
						startIndex = i;
						
					}
					
					if (expr[i] == ")")
					{
						lastIndex = i;
						
						break;
					}
				}
				
				return new int[]{startIndex,lastIndex};
			}
			
			protected bool isOperator(string str){
				
				return this.operators.Contains(str);
				
			}
			
			protected string resultOf(List<string> expr, string op, int index){
				
				switch (op)
				{
					case "implies":
						return (new ImplicationOperator(expr,index)).evaluate();
					
					case "and":
						return (new LogicalAddOpertor(expr,index)).evaluate();
					
					default:
						return "null";
				}
				
			}

		}
		
		internal abstract class BaseOperator {
			
			protected List<string> expr;
			
			protected int index;
			
			public BaseOperator(List<string> expr, int index){
				
				this.expr = expr;
				
				this.index = index;
				
			}
			
			protected bool valOf(string str){
				
				if (str == "true")
				{
					return true;
				}
				
				return false;
			}
			
			abstract public string evaluate();
			
		}
		
		internal class ImplicationOperator : BaseOperator{
			
			public ImplicationOperator(List<string> expr,int index) : base(expr,index){
				
				//...
				
			}
		
			public override string evaluate(){
				
				if (valOf(this.expr[this.index - 1]) && !valOf(this.expr[this.index + 1]))
				{
					
					return "false";
				}
				
				return "true";
			}
			
		}
		
		internal class LogicalAddOpertor : BaseOperator{
			
			public LogicalAddOpertor(List<string> expr, int index) : base(expr,index){
				
				//...
				
			}
			
			public override string evaluate(){
				
				if (valOf(this.expr[this.index - 1]) && valOf(this.expr[this.index + 1]))
				{
					return "true";
				}
				
				return "false";
				
			}
			
		}
		
		internal class LogicalOrOperator : BaseOperator{
			
			public LogicalOrOperator(List<string> expr, int index) : base(expr, index){
				
				//...
				
			}
			
			public override string evaluate(){
				
				return "";
				
			}
			
		}
		
		
		internal class Constructor
		{
		
			private List<string> vars;

			private List<Dictionary<string,int>> table;

			public Constructor(string[] vars)
			{
				this.vars = vars.ToList();

				this.table = new List<Dictionary<string, int>>();

				this.build();

			}

			protected void build()
			{
				foreach (string c in this.vars)
				{   
					if (this.table.Any())
					{
						List<Dictionary<string,int>> tmp = new List<Dictionary<string, int>>();

						foreach (Dictionary<string,int> dic in this.table)
						{

							var copiedDic = new Dictionary<string, int>(dic);

							copiedDic.Add(c, 0);


							tmp.Add(copiedDic);

							dic.Add(c, 1);
						}

						this.table.AddRange(tmp);

					}
					   else
					{
						Dictionary<string, int> dic = new Dictionary<string, int>()
						{
							{c , 1 }
						};

						this.table.Add(dic);

						Dictionary<string, int> dic2 = new Dictionary<string, int>()
						{
							{c ,0 }
						};

						this.table.Add(dic2);
					}

				}
			}
			
			public List<Dictionary<string,int>> getTable(){
				
				return this.table;
				
			}

			public void printCount()
			{
				Console.WriteLine("There are {0} combinations", this.table.Count());
			}

			public void printDic()
			{
				foreach (Dictionary<string,int> dic in this.table)
				{
					Console.WriteLine("New Set");

					foreach (KeyValuePair<string,int> pair in dic)
					{
						Console.WriteLine("key => {0}, value => {1}", pair.Key, pair.Value);
					}
				}
			}
		}

		static void Main(string[] args)
		{
		    DataGenerator generator = new DataGenerator(
											new string[]{
												"p implies q", "p and q", "( p implies q ) and ( p and q )"
											}, 
											new string[]{
												"p","q"
											});
		
			generator.printTruthTable();
									
		}
	}
}
