using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
   
    public class Calculate
    {
        public delegate int Operation(int a, int b);
        [Log]
        public int Execute(int a,int b,Operation op)
        {
            return op(a,b);
        }
    }
    public class Maths
    {
        [Log]
        public int mul(int a ,int b)
        {
            return a * b;
        }

        public int sub (int a, int b)
        {
            return a - b;
        }
        [Log]
        public int sum (int a, int b)
        {
            return a + b;
        }
        public int div(int a , int b)
        {
            return a / b; 
        }
        
    }
}
