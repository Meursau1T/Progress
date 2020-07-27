using System;
using System.Collections;
using System.IO;

namespace Progress
{
    class Task{
        int Finished;
        int Total;
        String Description;
        public Task(int Total,String Description){
            Finished = 0;
            this.Total = Total;
            this.Description = Description;
        }
        public void PrintDescription(){
            Console.WriteLine(Description);
        }
        public String GetDescription(){
            return Description;
        }
        public (int,int) GetProgress(){
            return (Finished, Total);
        }
        public void DrawProgress(){
            int ConsoleWidth = Console.BufferWidth;            
            String start = "[";
            String end = "]" + Finished.ToString() + "/" + Total.ToString();
            int ProgressBarLength = ConsoleWidth - start.Length - end.Length;
            double rate = (double)Finished / (double)Total;            
            int Black = (int)(ProgressBarLength * rate);
            int White = ProgressBarLength - Black;
            Console.WriteLine("{0}:", Description);
            Console.Write(start);
            for (int i = 0; i < Black; i++){
                Console.Write("#");
            }
            for (int i = 0; i < White; i++){
                Console.Write("-");
            }
            Console.WriteLine(end);            
        }
        public void SetProgress(String Operation){
            if(Operation[0] == '+'){
                int tmp = Int32.Parse(Operation.Substring(1));
                Finished += tmp;
            }else{
                Finished = Int32.Parse(Operation);
            }
        }
    }
    class Tasks{
        ArrayList TaskList;
        public Tasks(){
            TaskList = new ArrayList();
        }
        public void Add(int Total,String Description){
            TaskList.Add(new Task(Total, Description));
        }
        public void DrawAll(){
            int cnt = 0;
            foreach(Task t in TaskList){
                Console.Write("[{0}]",cnt);
                cnt += 1;
                t.DrawProgress();
            }
        }
        public void Print(int Index){
            ((Task)TaskList[Index]).DrawProgress();
        }
        public void SetProgress(int Index,String Operation){
            ((Task)TaskList[Index]).SetProgress(Operation);
        }        
        public void PrintList(){
            for (int i = 0; i < TaskList.Count; i++){
                Console.Write("[{0}]", i);
                ((Task)TaskList[i]).PrintDescription();
            }
        }
        public void Remove(int Index){
            TaskList.RemoveAt(Index);
        }
        public void Write(String URI){
            StreamWriter sw = new StreamWriter(URI);
            for (int i = 0; i < TaskList.Count; i++){
                Task task = (Task)TaskList[i];
                String Description;
                int Total, Finished;
                Description = task.GetDescription();
                (Finished, Total) = task.GetProgress();
                sw.Write("{0} {1} {2}\n", Finished, Total, Description);
            }
            sw.Close();
        }
        public void Read(String URI){
            StreamReader sr = new StreamReader(URI);
            string line;
            int cnt = 0;
            while((line = sr.ReadLine()) != null){
                string[] strs = line.Split(' ');
                int Total;
                String Description,Finished;
                Finished = strs[0];
                Total = Int32.Parse(strs[1]);
                Description = strs[2];
                TaskList.Add(new Task(Total, Description));
                ((Task)TaskList[cnt]).SetProgress(Finished);
                cnt++;
            }
            sr.Close();
        }
        
    }
    class Program
    {
        static String URI = System.AppDomain.CurrentDomain.BaseDirectory + "/users.data";
        static void PrintHelp(){
            Console.WriteLine("Save file:{0}",URI);
            Console.WriteLine("-a [TOTAL] [DESCRIPTION]: 添加一项任务，参数为总进度和名称，不能包含空格");
            Console.WriteLine("-h: 输出帮助信息");
            Console.WriteLine("-u [Index]: 更新某个任务的进度");
            Console.WriteLine("-l: 获取任务列表");
            Console.WriteLine("-rm [Index]: 删除某个任务");
            Console.WriteLine("-p： 输出所有任务的进度，如果跟参数，则输出单个任务的进度");
        }
        static void AddTask(string[] args){
            Tasks list = new Tasks();
            list.Read(URI);
            list.Add(Int32.Parse(args[1]), args[2]);
            list.Write(URI);
        }
        static void UpdateTask(string[] args){
            Tasks list = new Tasks();
            list.Read(URI);
            list.SetProgress(Int32.Parse(args[1]), args[2]);
            list.Write(URI);
        }
        static void PrintTask(string[] args){
            Tasks list = new Tasks();
            list.Read(URI);
            if(args.Length == 1){
                list.DrawAll();
            }else{
                list.Print(Int32.Parse(args[1]));
            }
        }
        static void PrintList(){
            Tasks list = new Tasks();
            list.Read(URI);
            list.PrintList();
        }
        static void Remove(int index){
            Tasks list = new Tasks();
            list.Read(URI);
            list.Remove(index);
            list.Write(URI);
        }
        static void Main(string[] args)
        {
            if(args.Length == 0){
                return;
            }
            
            switch(args[0]){
                case "-h":
                    PrintHelp();
                    break;
                case "-a":
                    AddTask(args);
                    break;
                case "-u":
                    UpdateTask(args);
                    break;
                case "-l":
                    PrintList();
                    break;
                case "-rm":
                    Remove(Int32.Parse(args[1]));
                    break;
                case "-p":
                    PrintTask(args);
                    break;
                default:
                    PrintHelp();
                    break;
            }
        }
    }
}
