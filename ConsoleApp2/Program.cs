
using ConsoleApp2;
using System.Runtime.InteropServices;

//var cal = new Calculate();
//var m = new Maths();
//Calculate.Operation del = m.sum;


//int result = cal.Execute(5, 2, del);
//Console.WriteLine(  result);
var x = new TestService();
TraceContext.Set("1");
x.RunSequential();