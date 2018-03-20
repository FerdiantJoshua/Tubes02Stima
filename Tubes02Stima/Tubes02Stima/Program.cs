using System;
using System.Collections.Generic;
using System.IO;

namespace C3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
        static List<string> readFile(){
            List<string> list_of_input = new List<string>();
            using (var reader = new StreamReader("test.txt")){
                string line;
                while ((line = reader.ReadLine()) != null){
                    list_of_input.Add(line);
                }
            }
            return list_of_input;
        }
        static List<List<string>> parse(List<string> list_of_input){
            List<List<string>> each_line = new List<List<string>>();
            foreach(var line in list_of_input){
                string[] lines = line.Split(", ");
                List<string> temp = new List<string>();
                for(int i = 0; i < lines.Length; i++) {
                    temp.Add(lines[i]);
                }
                each_line.Add(temp);
            }
            return each_line;
        }
        static List<string> DFS(List<List<string>> each_line){
            List<string> jawaban = new List<string>();
            // for belum selesai
            return jawaban;
        }
    }
}
