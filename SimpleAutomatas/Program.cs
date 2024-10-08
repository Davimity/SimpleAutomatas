using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAutomatas.Classes;

namespace SimpleAutomatas {
    internal static class Program {

       public static void Main(string[] args) {
            Console.Write("""
                              ========================================
                              =========== SIMPLE AUTOMATAS ===========
                              ========================================
                              
                              Introduce the file path to the automata: 
                              """);

            var path = Console.ReadLine() ?? "";

            var automata = Automata.ReadFromFile(path);

            Console.Write($"""
                               
                               Read automata:
                               
                               {automata}
                               
                               """);

            while (true) {
                Console.Write("Input to check: ");
                var input = Console.ReadLine() ?? "";

                Console.Write($"""
                               
                               The automata {(automata.Accepts(input) ? "accepts" : "does not accept")} the input
                               
                               Trace: {automata.GetTrace()}
                               
                               Do you want to check another input? (y/n): 
                               """);

                var response = Console.ReadLine() ?? "";

                if (!response.ToLower().Equals("y")) break;
            }
       }
    }
}
